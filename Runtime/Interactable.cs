using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace boop
{
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Interactable : UIBehaviour, IMoveHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler,
        IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public enum State
        {
            Normal,
            Hovered,
            Pressed,
            Selected,
            Disabled
        }

        public event Action<Interactable> OnStateNormal;
        public event Action<Interactable> OnStateHovered;
        public event Action<Interactable> OnStatePressed;
        public event Action<Interactable> OnStateSelected;
        public event Action<Interactable> OnStateDisabled;

        private static List<Interactable> s_interactables = new();

        [SerializeField] protected bool _isSelectable;
        protected bool _isInteractable = true;
        protected bool _groupAllowInteraction = true;
        protected bool _isPointerDown;
        protected bool _isPointerInside;
        protected bool _hasSelection;
        protected State _currentState;

        protected override void OnEnable()
        {
            s_interactables.Add(this);

            State state = State.Normal;
            if (_hasSelection)
                state = State.Hovered;
            _currentState = state;
            PerformStateTransition(_currentState, true);
        }

        protected override void OnDisable()
        {
            s_interactables.Remove(this);
            ClearState();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            PerformStateTransition(_currentState, true);
        }
#endif

        public virtual void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    Navigate(eventData, FindInteractable(transform.rotation * Vector3.right));
                    break;
                case MoveDirection.Up:
                    Navigate(eventData, FindInteractable(transform.rotation * Vector3.up));
                    break;
                case MoveDirection.Left:
                    Navigate(eventData, FindInteractable(transform.rotation * Vector3.left));
                    break;
                case MoveDirection.Down:
                    Navigate(eventData, FindInteractable(transform.rotation * Vector3.down));
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (IsInteractable())
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);

            _isPointerDown = true;
            EvaluateAndTransition(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPointerDown = false;
            EvaluateAndTransition(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;
            EvaluateAndTransition(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerInside = false;
            EvaluateAndTransition(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _hasSelection = true;
            EvaluateAndTransition(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _hasSelection = false;
            EvaluateAndTransition(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public void OnSubmit(BaseEventData eventData)
        {
            
        }

        public void Rebuild(CanvasUpdate executing)
        {
            
        }

        public void LayoutComplete()
        {
            
        }

        public void GraphicUpdateComplete()
        {
            
        }

        public virtual bool IsInteractable()
        {
            return _isInteractable && _groupAllowInteraction;
        }

        public virtual void Select()
        {
            if (EventSystem.current.alreadySelecting) return;

            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public Interactable FindInteractable(Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;
            Interactable bestPick = null;

            for (int i = 0; i < s_interactables.Count; ++i)
            {
                Interactable interactable = s_interactables[i];
                if (interactable == this || interactable == null || !interactable.IsInteractable()) continue;

                RectTransform currentRect = interactable.transform as RectTransform;
                Vector3 center = currentRect != null ? (Vector3)currentRect.rect.center : Vector3.zero;
                Vector3 toNext = interactable.transform.TransformPoint(center) - pos;

                float dot = Vector3.Dot(dir, toNext);
                if (dot <= 0) continue;

                float score = dot / toNext.sqrMagnitude;
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = interactable;
                }
            }
            return bestPick;

            Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
            {
                if (rect == null)
                    return Vector3.zero;
                if (dir != Vector2.zero)
                    dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
                dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
                return dir;
            }
        }

        protected virtual bool IsHovered(BaseEventData eventData)
        {
            if (!IsActive() || IsPressed()) return false;

            bool selected = _hasSelection;
            if (eventData is PointerEventData && eventData is PointerEventData pointerData)
            {
                selected |= (_isPointerDown && !_isPointerInside && pointerData.pointerPress == gameObject)
                    || (!_isPointerDown && _isPointerInside && pointerData.pointerPress == gameObject)
                    || (!_isPointerDown && _isPointerInside && pointerData.pointerPress == null);
            }
            else
                selected |= _isPointerInside;
            return selected;
        }

        protected virtual bool IsPressed()
        {
            return IsActive() && _isPointerInside && _isPointerDown;
        }

        protected void UpdateCurrentState(BaseEventData eventData)
        {
            if (!IsInteractable())
                _currentState = State.Disabled;
            else if (IsPressed())
                _currentState = State.Pressed;
            else if (IsHovered(eventData))
                _currentState = State.Hovered;
            else
                _currentState = State.Normal;
        }

        protected virtual void PerformStateTransition(State state, bool instant)
        {
            switch (state)
            {
                case State.Normal:
                    NormalTransition(instant);
                    break;
                case State.Hovered:
                    HoveredTransition(instant);
                    break;
                case State.Pressed:
                    PressedTransition(instant);
                    break;
                case State.Disabled:
                    DisabledTransition(instant);
                    break;
                default: NormalTransition(instant);
                    break;
            }
        }

        protected virtual void ClearState()
        {
            _currentState = State.Normal;
            _isPointerDown = false;
            _isPointerInside = false;
            _hasSelection = false;

            NormalTransition(true);
        }

        protected virtual void NormalTransition(bool instant)
        {
            OnStateNormal?.Invoke(this);
            Debug.Log($"Current State: Normal");
        }

        protected virtual void HoveredTransition(bool instant)
        {
            OnStateHovered?.Invoke(this);
            Debug.Log($"Current State: Hovered");
        }

        protected virtual void PressedTransition(bool instant)
        {
            OnStatePressed?.Invoke(this);
            Debug.Log($"Current State: Pressed");
        }

        protected virtual void DisabledTransition(bool instant)
        {
            OnStateDisabled?.Invoke(this);
            Debug.Log($"Current State: Disabled");
        }

        private void EvaluateAndTransition(BaseEventData eventData)
        {
            if (!IsActive()) return;

            UpdateCurrentState(eventData);
            PerformStateTransition(_currentState, false);
        }

        private void Navigate(AxisEventData eventData, Interactable interactable)
        {
            if (interactable != null && interactable.IsActive())
                eventData.selectedObject = interactable.gameObject;
        }
    }
}