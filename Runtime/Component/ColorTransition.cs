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
            public StateColorConfig Config;
        }

        private struct Fade
        {
            public Graphic Graphic;
            public Color Start;
            public Color Target;
            public float Duration;
            public AnimationCurve Curve;
            public bool Active;
        }

        [SerializeField] private ColorSetting[] _settings;

        private Fade[] _fades;
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            if (_settings == null) return;

            int count = _settings.Length;
            if (_fades == null || _fades.Length != count)
                _fades = new Fade[count];
        }

        private void OnDisable()
        {
            CancelTransition();
        }

        public override void Transition(Interactable.State state, bool selected, bool instant = false)
        {
            if (_settings == null || _settings.Length == 0) return;

            CancelTransition();

            if (instant)
            {
                for (int i = 0; i < _settings.Length; ++i)
                {
                    Graphic graphic = _settings[i].Graphic;
                    StateColorConfig config = _settings[i].Config;
                    if (graphic == null || config == null) continue;
                    if (!TryGetStateColor(config, selected, state, out StateColor stateColor)) continue;

                    graphic.color = stateColor.Color;
                    graphic.gameObject.SetActive(stateColor.Color.a > 0f);
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
                StateColorConfig config = _settings[i].Config;
                if (graphic == null || config == null) continue;

                bool found = TryGetStateColor(config, selected, state, out StateColor stateColor);
                if (found && stateColor.Color.a > 0f && !graphic.gameObject.activeSelf)
                    graphic.gameObject.SetActive(true);

                bool active = found && graphic.gameObject.activeSelf;
                _fades[i] = new Fade
                {
                    Graphic = graphic,
                    Start = graphic.color,
                    Target = stateColor.Color,
                    Duration = stateColor.Duration,
                    Curve = stateColor.Curve,
                    Active = active,
                };
                if (active && stateColor.Duration > maxDuration)
                    maxDuration = stateColor.Duration;
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

                for (int i = 0; i < _fades.Length; ++i)
                {
                    Fade fade = _fades[i];
                    if (!fade.Active) continue;

                    fade.Graphic.color = fade.Target;
                    if (fade.Target.a <= 0f)
                        fade.Graphic.gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ApplyColor(float elapsed)
        {
            for (int i = 0; i < _fades.Length; ++i)
            {
                ref Fade fade = ref _fades[i];
                if (!fade.Active) continue;

                float n = fade.Duration <= 0f ? 1f : Mathf.Clamp01(elapsed / fade.Duration);
                float t = fade.Curve != null && fade.Curve.length > 0 ? fade.Curve.Evaluate(n) : n;
                fade.Graphic.color = Color.LerpUnclamped(fade.Start, fade.Target, t);
            }
        }

        private void CancelTransition()
        {
            if (_cts == null) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private bool TryGetStateColor(StateColorConfig config, bool selected, Interactable.State state, out StateColor result)
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

        private bool FindStateColor(StateColor[] states, Interactable.State state, out StateColor result)
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
