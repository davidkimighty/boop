using UnityEngine;

namespace boop
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIEventChannel _eventChannel;

        private UIRegistry _uiRegistry;

        private void Awake()
        {
            _uiRegistry = new UIRegistry();
            _uiRegistry.OnShowView += _eventChannel.RaiseOnShowView;
            _uiRegistry.OnHideView += _eventChannel.RaiseOnHideView;
        }

        public void Register(string id, IElement element)
        {
            _uiRegistry.Register(id, element);
        }

        public void Unregister(string id)
        {
            _uiRegistry.Unregister(id);
        }

        public void ShowView(string id, bool bringToFront = true)
        {
            _uiRegistry.ShowView(id, bringToFront);
        }

        public void HideView(string id)
        {
            _uiRegistry.HideView(id);
        }
    }
}
