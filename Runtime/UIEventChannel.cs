using System;
using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "UIEventChannel", menuName = "boop/EventChannel/UIEventChannel")]
    public class UIEventChannel : ScriptableObject
    {
        public event Action<IView> OnShowView;
        public event Action<IView> OnHideView;

        public void RaiseOnShowView(IView view) => OnShowView?.Invoke(view);

        public void RaiseOnHideView(IView view) => OnHideView?.Invoke(view);
    }
}
