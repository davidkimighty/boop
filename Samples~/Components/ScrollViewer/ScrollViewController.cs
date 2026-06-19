using System;
using System.Collections.Generic;
using System.Threading;
using boop;
using UnityEngine;

namespace boop_sample
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private ScrollViewer _scrollViewer;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Vector3 _scrollTargetScale = new(0.7f, 0.7f, 0.7f);
        [SerializeField] private AnimationCurve _scrollScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _scaleDuration = 0.2f;

        private int _index;
        private readonly Dictionary<GameObject, CancellationTokenSource> _scaleCts = new();

        private void Awake()
        {
            _nextButton.OnClick += () => ScrollTo(+1);
            _previousButton.OnClick += () => ScrollTo(-1);
            _scrollViewer.OnFocusElement += HandleFocus;
            _scrollViewer.OnUnfocusElement += HandleUnfocus;

            foreach (GameObject element in _scrollViewer.Elements)
                if (element != null)
                    element.transform.localScale = _scrollTargetScale;
        }

        private void OnDestroy()
        {
            foreach (CancellationTokenSource cts in _scaleCts.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _scaleCts.Clear();
        }

        private void ScrollTo(int increment)
        {
            int count = _scrollViewer.Elements.Count;
            _index = ((_index + increment) % count + count) % count;
            _scrollViewer.SetFocus(_index);
        }

        private void HandleFocus(GameObject element, int index)
        {
            _index = index;
            AnimateScale(element, Vector3.one);
        }

        private void HandleUnfocus(GameObject element, int index)
        {
            AnimateScale(element, _scrollTargetScale);
        }

        private async void AnimateScale(GameObject element, Vector3 target)
        {
            if (element == null) return;

            if (_scaleCts.TryGetValue(element, out CancellationTokenSource previous))
            {
                previous.Cancel();
                previous.Dispose();
            }

            CancellationTokenSource cts = new();
            _scaleCts[element] = cts;

            try
            {
                await ScaleAsync(element.transform, target, cts.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (_scaleCts.TryGetValue(element, out CancellationTokenSource current) && current == cts)
                {
                    _scaleCts.Remove(element);
                    cts.Dispose();
                }
            }
        }

        private async Awaitable ScaleAsync(Transform target, Vector3 to, CancellationToken token)
        {
            Vector3 from = target.localScale;
            float elapsed = 0f;
            while (elapsed < _scaleDuration)
            {
                elapsed += Time.deltaTime;
                float curve = _scrollScaleCurve.Evaluate(Mathf.Clamp01(elapsed / _scaleDuration));
                target.localScale = Vector3.LerpUnclamped(from, to, curve);
                await Awaitable.NextFrameAsync(token);
            }
            target.localScale = to;
        }
    }
}
