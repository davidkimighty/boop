using System;
using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "UIEventChannel", menuName = "boop/EventChannel/UIEventChannel")]
    public class UIEventChannel : ScriptableObject
    {
        public event Action<IUIElement> OnRequestRegisterUI;
        public event Action<IUIElement> OnRequestUnregisterUI;
        public event Action<Type> OnRequstShowUI;
        public event Action<Type> OnRequestHideUI;

        public void RequestRegisterUI(IUIElement uiElement)
        {
            OnRequestRegisterUI?.Invoke(uiElement);
        }

        public void RequestUnregisterUI(IUIElement uiElement)
        {
            OnRequestUnregisterUI?.Invoke(uiElement);
        }

        public void RequestShowUI<T>() where T : IUIElement
        {
            OnRequstShowUI?.Invoke(typeof(T));
        }

        public void RequestHideUI<T>() where T : IUIElement
        {
            OnRequestHideUI?.Invoke(typeof(T));
        }
    }
}
