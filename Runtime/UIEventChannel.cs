using System;
using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "UIEventChannel", menuName = "boop/EventChannel/UIEventChannel")]
    public class UIEventChannel : ScriptableObject
    {
        public event Action<IPanel> OnShowPanel;
        public event Action<IPanel> OnHidePanel;

        public void RaiseShowPanel(IPanel panel) => OnShowPanel?.Invoke(panel);

        public void RaiseHidePanel(IPanel panel) => OnHidePanel?.Invoke(panel);
    }
}
