using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace boop
{
    public class ScrollViewer : UIBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public event Action OnBeginScroll = null;
        public event Action OnEndScroll = null;
        public event Action<GameObject, int> OnFocusElement = null;
        public event Action<GameObject, int> OnUnfocusElement = null;

        [SerializeField] private List<GameObject> _elements = new();
        [SerializeField] private Transform _contentHolder = null;
        [SerializeField] private Scrollbar _scrollbar = null;
        [SerializeField, Range(0.05f, 1)] private float _snapTime = 0.15f;
        [SerializeField, Range(0.05f, 1)] private float _scrollStopSpeed = 0.3f;
        [SerializeField] private bool _scrollWhenRelease = true;

        private float[] _anchorPoints = null;
        private float _targetAnchorPoint = 0f;

        private float _scrollbarValue = 0f;
        private float _scrollVel = 0f;
        private bool _dragging = false;
        private bool _isScrolling = false;
        private int _focusedIndex = -1;

        public List<GameObject> Elements => _elements;

        protected override void Awake()
        {
            RebuildAnchorPoints();
        }

        private void Update()
        {
            if (_anchorPoints == null) return;

            if (_dragging || (_scrollWhenRelease && GetScrollSpeed() > _scrollStopSpeed))
            {
                SetScrolling(true);
                _targetAnchorPoint = _anchorPoints[NearestAnchorIndex()];
            }
            else if (!HasSettled(_targetAnchorPoint))
            {
                SetScrolling(true);
                _scrollbar.value = Mathf.SmoothDamp(_scrollbar.value, _targetAnchorPoint, ref _scrollVel, _snapTime, Mathf.Infinity, Time.deltaTime);
            }
            else
            {
                _scrollVel = 0f;
                SetScrolling(false);
            }

            _scrollbarValue = _scrollbar.value;

            int index = NearestAnchorIndex();
            if (index == _focusedIndex) return;

            if (_focusedIndex >= 0)
                OnUnfocusElement?.Invoke(_elements[_focusedIndex], _focusedIndex);

            _focusedIndex = index;
            OnFocusElement?.Invoke(_elements[index], index);
        }

        public void OnBeginDrag(PointerEventData eventData) => _dragging = true;

        public void OnEndDrag(PointerEventData eventData) => _dragging = false;

        public void SetFocus(int index)
        {
            if (_anchorPoints == null || index < 0 || index >= _anchorPoints.Length) return;

            _targetAnchorPoint = _anchorPoints[index];
        }

        public void Add(GameObject element)
        {
            if (element == null || _elements.Contains(element)) return;

            if (_contentHolder != null)
                element.transform.SetParent(_contentHolder, false);

            _elements.Add(element);
            RebuildAnchorPoints();
        }

        public bool Remove(GameObject element)
        {
            int index = _elements.IndexOf(element);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _elements.Count) return;

            _elements.RemoveAt(index);
            RebuildAnchorPoints();

            if (_focusedIndex >= _elements.Count)
                _focusedIndex = _elements.Count - 1;
        }

        private void RebuildAnchorPoints()
        {
            int count = _elements.Count;
            if (count == 0)
            {
                _anchorPoints = null;
                _focusedIndex = -1;
                return;
            }

            _anchorPoints = new float[count];
            if (count == 1)
            {
                _anchorPoints[0] = 0f;
                _targetAnchorPoint = 0f;
                return;
            }

            float subdivisionDist = 1f / (count - 1);
            for (int i = 0; i < count; i++)
                _anchorPoints[i] = subdivisionDist * i;
        }

        private void SetScrolling(bool scrolling)
        {
            if (scrolling == _isScrolling) return;

            _isScrolling = scrolling;
            if (scrolling)
                OnBeginScroll?.Invoke();
            else
                OnEndScroll?.Invoke();
        }

        private int NearestAnchorIndex()
        {
            int nearest = 0;
            float best = Mathf.Abs(_scrollbar.value - _anchorPoints[0]);
            for (int i = 1; i < _anchorPoints.Length; i++)
            {
                float dist = Mathf.Abs(_scrollbar.value - _anchorPoints[i]);
                if (dist < best)
                {
                    best = dist;
                    nearest = i;
                }
            }
            return nearest;
        }

        private float GetScrollSpeed()
        {
            return Mathf.Abs(_scrollbarValue - _scrollbar.value) / Time.deltaTime;
        }

        private bool HasSettled(float anchorPoint)
        {
            return Mathf.Abs(_scrollbar.value - anchorPoint) <= 0.001f;
        }
    }
}
