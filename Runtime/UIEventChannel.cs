using System;
using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "UIEventChannel", menuName = "boop/EventChannel/UIEventChannel")]
    public class UIEventChannel : ScriptableObject
    {
        public event Func<IView, Guid> RegisterUIRequested;
        public event Action<IView> UnregisterUIRequested;
        public event Action<Type, Guid?> ShowUIRequested;
        public event Action<Type, Guid?> HideUIRequested;

        public Guid RequestRegisterUI(IView uiElement)
        {
            return RegisterUIRequested != null ? RegisterUIRequested.Invoke(uiElement) : Guid.Empty;
        }

        public void RequestUnregisterUI(IView uiElement)
        {
            UnregisterUIRequested?.Invoke(uiElement);
        }

        public void RequestShowUI<T>(Guid? id = null) where T : IView
        {
            ShowUIRequested?.Invoke(typeof(T), id);
        }

        public void RequestHideUI<T>(Guid? id = null) where T : IView
        {
            HideUIRequested?.Invoke(typeof(T), id);
        }
    }
}
