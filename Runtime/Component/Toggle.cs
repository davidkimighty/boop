using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public class Toggle : Interactable, IPointerClickHandler, ISubmitHandler
    {
        public event Action<Toggle, bool> OnClick;

        [SerializeField] protected bool _isToggled;
        [SerializeField] protected float _submitTransitionDelay = 0.3f;

        protected override bool IsSelected() => _isToggled;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                !IsActive() || !IsInteractable()) return;

            _isToggled = !_isToggled;
            _currentState = GetCurrentState(eventData);
            PerformTransition(_currentState, IsSelected());
            OnClick?.Invoke(this, _isToggled);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            _isToggled = !_isToggled;
            _currentState = GetCurrentState(eventData);
            PerformTransition(State.Pressed, IsSelected());
            _ = PerformDelayedTransitionAsync(_currentState, _submitTransitionDelay);
            OnClick?.Invoke(this, _isToggled);
        }

        public void SetToggle(bool on, bool notify = false, bool instant = false)
        {
            _isToggled = on;
            _currentState = State.Normal;
            PerformTransition(_currentState, IsSelected(), instant);
            if (notify) OnClick?.Invoke(this, _isToggled);
        }
    }
}
