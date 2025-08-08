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
            _uiEventChannel.RegisterUIRequested += HandleRegister;
            _uiEventChannel.UnregisterUIRequested += HandleUnregister;
            _uiEventChannel.ShowUIRequested += HandleShow;
            _uiEventChannel.HideUIRequested += HandleHide;
        }

        private void OnDestroy()
        {
            _uiEventChannel.RegisterUIRequested -= HandleRegister;
            _uiEventChannel.UnregisterUIRequested -= HandleUnregister;
            _uiEventChannel.ShowUIRequested -= HandleShow;
            _uiEventChannel.HideUIRequested -= HandleHide;
        }

        private Guid HandleRegister(IView view)
        {
            return _uiRegistry.Register(view);
        }

        private void HandleUnregister(IView view)
        {
            _uiRegistry.Unregister(view);
        }

        private void HandleShow(Type type, Guid? id)
        {
            if (id.HasValue)
                _uiRegistry.ShowView(id.Value);
            else
                _uiRegistry.ShowView(type);
        }

        private void HandleHide(Type type, Guid? id)
        {
            if (id.HasValue)
                _uiRegistry.HideView(id.Value);
            else
                _uiRegistry.HideView(type);
        }
    }
}
