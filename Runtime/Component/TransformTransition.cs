using System;
using System.Threading;
using UnityEngine;

namespace boop
{
    public class TransformTransition : StateTransition
    {
        [Serializable]
        public struct TransformSetting
        {
            public Transform Source;
            public TransformTransitionConfig Config;
        }

        private struct Data
        {
            public Transform Source;
            public TransformChannel Channels;
            public Vector3 StartPosition;
            public Quaternion StartRotation;
            public Vector3 StartScale;
            public Vector3 TargetPosition;
            public Quaternion TargetRotation;
            public Vector3 TargetScale;
            public float Duration;
            public AnimationCurve Curve;
            public bool Active;
        }

        [SerializeField] private TransformSetting[] _settings;

        private Data[] _data;
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            InitDataIfNeeded();
        }

        private void OnDisable()
        {
            CancelTransition();
        }

        public override void Transition(Interactable.State state, bool selected, bool instant = false)
        {
            if (_settings == null || _settings.Length == 0) return;

            InitDataIfNeeded();
            CancelTransition();

            if (instant)
            {
                for (int i = 0; i < _settings.Length; ++i)
                {
                    Transform source = _settings[i].Source;
                    TransformTransitionConfig config = _settings[i].Config;
                    if (source == null || config == null) continue;
                    if (!TryGetStateTransform(config, selected, state, out StateTarget<TransformValue> target)) continue;

                    if ((target.Value.Channels & TransformChannel.Position) != 0)
                        source.localPosition = source is RectTransform rect ? ApplyRectPivot(rect, target.Value.Position) : target.Value.Position;
                    if ((target.Value.Channels & TransformChannel.Rotation) != 0)
                        source.localRotation = Quaternion.Euler(target.Value.Rotation);
                    if ((target.Value.Channels & TransformChannel.Scale) != 0)
                        source.localScale = target.Value.Scale;
                }
                return;
            }

            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _ = MoveAsync(state, selected, _cts.Token);
        }

        private async Awaitable MoveAsync(Interactable.State state, bool selected, CancellationToken token)
        {
            int count = _settings.Length;
            float maxDuration = 0f;
            for (int i = 0; i < count; ++i)
            {
                Transform source = _settings[i].Source;
                TransformTransitionConfig config = _settings[i].Config;
                if (source == null || config == null)
                {
                    _data[i] = default;
                    continue;
                }

                bool found = TryGetStateTransform(config, selected, state, out StateTarget<TransformValue> target);
                TransformValue value = target.Value;
                _data[i] = new Data
                {
                    Source = source,
                    Channels = value.Channels,
                    StartPosition = source.localPosition,
                    StartRotation = source.localRotation,
                    StartScale = source.localScale,
                    TargetPosition = source is RectTransform rect ? ApplyRectPivot(rect, value.Position) : value.Position,
                    TargetRotation = Quaternion.Euler(value.Rotation),
                    TargetScale = value.Scale,
                    Duration = target.Duration,
                    Curve = target.Curve,
                    Active = found && value.Channels != TransformChannel.None,
                };
                if (_data[i].Active && target.Duration > maxDuration)
                    maxDuration = target.Duration;
            }

            float elapsed = 0f;
            try
            {
                while (elapsed < maxDuration)
                {
                    ApplyTransform(elapsed);
                    await Awaitable.NextFrameAsync(token);
                    elapsed += Time.unscaledDeltaTime;
                }

                for (int i = 0; i < _data.Length; ++i)
                {
                    Data data = _data[i];
                    if (!data.Active) continue;

                    if ((data.Channels & TransformChannel.Position) != 0)
                        data.Source.localPosition = data.TargetPosition;
                    if ((data.Channels & TransformChannel.Rotation) != 0)
                        data.Source.localRotation = data.TargetRotation;
                    if ((data.Channels & TransformChannel.Scale) != 0)
                        data.Source.localScale = data.TargetScale;
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ApplyTransform(float elapsed)
        {
            for (int i = 0; i < _data.Length; ++i)
            {
                ref Data data = ref _data[i];
                if (!data.Active) continue;

                float n = data.Duration <= 0f ? 1f : Mathf.Clamp01(elapsed / data.Duration);
                float t = data.Curve != null && data.Curve.length > 0 ? data.Curve.Evaluate(n) : n;

                if ((data.Channels & TransformChannel.Position) != 0)
                    data.Source.localPosition = Vector3.LerpUnclamped(data.StartPosition, data.TargetPosition, t);
                if ((data.Channels & TransformChannel.Rotation) != 0)
                    data.Source.localRotation = Quaternion.SlerpUnclamped(data.StartRotation, data.TargetRotation, t);
                if ((data.Channels & TransformChannel.Scale) != 0)
                    data.Source.localScale = Vector3.LerpUnclamped(data.StartScale, data.TargetScale, t);
            }
        }

        private void InitDataIfNeeded()
        {
            if (_settings == null) return;

            int count = _settings.Length;
            if (_data == null || _data.Length != count)
                _data = new Data[count];
        }

        private void CancelTransition()
        {
            if (_cts == null) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private bool TryGetStateTransform(TransformTransitionConfig config, bool selected, Interactable.State state, out StateTarget<TransformValue> result)
        {
            if (FindStateTransform(selected ? config.Selected : config.Normal, state, out result))
                return true;

            if (selected && FindStateTransform(config.Normal, state, out result))
                return true;

            if (FindStateTransform(config.Normal, Interactable.State.Normal, out result))
                return true;

            result = default;
            return false;
        }

        private bool FindStateTransform(StateTarget<TransformValue>[] states, Interactable.State state, out StateTarget<TransformValue> result)
        {
            if (states != null)
            {
                for (int i = 0; i < states.Length; ++i)
                {
                    if (states[i].State != state) continue;
                    result = states[i];
                    return true;
                }
            }
            result = default;
            return false;
        }

        private Vector3 ApplyRectPivot(RectTransform rect, Vector3 pos) => pos - (Vector3)rect.rect.center;
    }
}
