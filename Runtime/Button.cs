using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public class Button : Interactable, IPointerClickHandler, ISubmitHandler
    {
        public event Action OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                !IsActive() || !IsInteractable()) return;

            OnClick?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            PerformStateTransition(State.Pressed, false);
            StartCoroutine(PerformDelayedStateTransition(_currentState, false));

            OnClick?.Invoke();
        }

        protected virtual IEnumerator PerformDelayedStateTransition(State state, bool instant)
        {
            float elapsedTime = 0f;
            float delay = 0.1f; // get delay time from current state

            while (elapsedTime < delay)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            PerformStateTransition(state, instant);
        }
    }
}
