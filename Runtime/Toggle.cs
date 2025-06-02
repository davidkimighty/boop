using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public class Toggle : Interactable, IPointerClickHandler, ISubmitHandler
    {
        public event Action<bool> OnClick;

        protected bool _isToggled;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                !IsActive() || !IsInteractable()) return;

            _isToggled = !_isToggled;
            _currentState = GetCurrentState(eventData);
            PerformStateTransition(_currentState, false);

            OnClick?.Invoke(_isToggled);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            _isToggled = !_isToggled;
            _currentState = GetCurrentState(eventData);
            PerformStateTransition(_currentState, false);

            OnClick?.Invoke(_isToggled);
        }

        protected override void PerformStateTransition(State state, bool instant)
        {
            if (state != State.Disabled && _isToggled)
                ToggledTransition(instant);

            base.PerformStateTransition(state, instant);
        }

        protected override void NormalTransition(bool instant)
        {
            
        }

        protected override void HoveredTransition(bool instant)
        {

        }

        protected override void PressedTransition(bool instant)
        {
            
        }

        protected override void DisabledTransition(bool instant)
        {
            
        }

        protected void ToggledTransition(bool instant)
        {

        }
    }
}