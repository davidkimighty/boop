using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public abstract class Panel : UIBehaviour, IUIElement
    {
        public event Action<EntityId> OnShow;
        public event Action<EntityId> OnHide;

        [SerializeField] protected Canvas _canvas;

        public Canvas Canvas => _canvas;

        public virtual void Show()
        {
            Canvas.enabled = true;
            OnShow?.Invoke(GetEntityId());
        }

        public virtual void Hide()
        {
            Canvas.enabled = false;
            OnHide?.Invoke(GetEntityId());
        }
    }
}
