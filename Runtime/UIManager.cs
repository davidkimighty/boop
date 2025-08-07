using System;
using UnityEngine;

namespace boop
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIEventChannel _uiEventChannel;

        private UIRegistry _uiRegistry;

        private void Awake()
        {
            _uiRegistry = new UIRegistry();
            _uiEventChannel.OnRequestRegisterUI += HandleRegister;
            _uiEventChannel.OnRequestUnregisterUI += HandleUnregister;
            _uiEventChannel.OnRequstShowUI += HandleShow;
            _uiEventChannel.OnRequestHideUI += HandleHide;
        }

        private void OnDestroy()
        {
            _uiEventChannel.OnRequestRegisterUI -= HandleRegister;
            _uiEventChannel.OnRequestUnregisterUI -= HandleUnregister;
            _uiEventChannel.OnRequstShowUI -= HandleShow;
            _uiEventChannel.OnRequestHideUI -= HandleHide;
        }

        private void HandleRegister(IUIElement uiElement)
        {
            _uiRegistry.Register(uiElement);
        }

        private void HandleUnregister(IUIElement uiElement)
        {
            _uiRegistry.Unregister(uiElement);
        }

        private void HandleShow(Type type)
        {
            _uiRegistry.Show(type);
        }

        private void HandleHide(Type type)
        {
            _uiRegistry.Hide(type);
        }
    }
}
