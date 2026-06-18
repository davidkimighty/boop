using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace boop
{
    public class ColorTransition : StateTransition
    {
        [Serializable]
        public struct ColorSetting
        {
            public Graphic Graphic;
            public ColorTransitionConfig Config;
        }

        private struct Data
        {
            public Graphic Graphic;
            public Color Start;
            public Color Target;
            public float Duration;
            public AnimationCurve Curve;
            public bool Active;
        }

        [SerializeField] private ColorSetting[] _settings;

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
                    Graphic graphic = _settings[i].Graphic;
                    ColorTransitionConfig config = _settings[i].Config;
                    if (graphic == null || config == null) continue;
                    if (!TryGetStateColor(config, selected, state, out StateTarget<Color> target)) continue;

                    graphic.color = target.Value;
                    graphic.gameObject.SetActive(target.Value.a > 0f);
                }
                return;
            }

            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _ = FadeAsync(state, selected, _cts.Token);
        }

        private async Awaitable FadeAsync(Interactable.State state, bool selected, CancellationToken token)
        {
            int count = _settings.Length;
            float maxDuration = 0f;
            for (int i = 0; i < count; ++i)
            {
                Graphic graphic = _settings[i].Graphic;
                ColorTransitionConfig config = _settings[i].Config;
                if (graphic == null || config == null)
                {
                    _data[i] = default;
                    continue;
                }

                bool found = TryGetStateColor(config, selected, state, out StateTarget<Color> target);
                if (found && target.Value.a > 0f && !graphic.gameObject.activeSelf)
                    graphic.gameObject.SetActive(true);

                bool active = found && graphic.gameObject.activeSelf;
                _data[i] = new Data
                {
                    Graphic = graphic,
                    Start = graphic.color,
                    Target = target.Value,
                    Duration = target.Duration,
                    Curve = target.Curve,
                    Active = active,
                };
                if (active && target.Duration > maxDuration)
                    maxDuration = target.Duration;
            }

            float elapsed = 0f;
            try
            {
                while (elapsed < maxDuration)
                {
                    ApplyColor(elapsed);
                    await Awaitable.NextFrameAsync(token);
                    elapsed += Time.unscaledDeltaTime;
                }

                for (int i = 0; i < _data.Length; ++i)
                {
                    Data data = _data[i];
                    if (!data.Active) continue;

                    data.Graphic.color = data.Target;
                    if (data.Target.a <= 0f)
                        data.Graphic.gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ApplyColor(float elapsed)
        {
            for (int i = 0; i < _data.Length; ++i)
            {
                ref Data data = ref _data[i];
                if (!data.Active) continue;

                float n = data.Duration <= 0f ? 1f : Mathf.Clamp01(elapsed / data.Duration);
                float t = data.Curve != null && data.Curve.length > 0 ? data.Curve.Evaluate(n) : n;
                data.Graphic.color = Color.LerpUnclamped(data.Start, data.Target, t);
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

        private bool TryGetStateColor(ColorTransitionConfig config, bool selected, Interactable.State state, out StateTarget<Color> result)
        {
            if (FindStateColor(selected ? config.Selected : config.Normal, state, out result))
                return true;

            if (selected && FindStateColor(config.Normal, state, out result))
                return true;

            if (FindStateColor(config.Normal, Interactable.State.Normal, out result))
                return true;

            result = default;
            return false;
        }

        private bool FindStateColor(StateTarget<Color>[] states, Interactable.State state, out StateTarget<Color> result)
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
    }
}
