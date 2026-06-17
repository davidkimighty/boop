using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public class Button : Interactable, IPointerClickHandler, ISubmitHandler
    {
        public event Action OnClick;

        [SerializeField] protected float _submitTransitionDelay = 0.3f;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                !IsActive() || !IsInteractable()) return;

            OnClick?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            PerformTransition(State.Pressed, IsSelected());
            _ = PerformDelayedTransitionAsync(_currentState, _submitTransitionDelay);

            OnClick?.Invoke();
        }
    }
}
