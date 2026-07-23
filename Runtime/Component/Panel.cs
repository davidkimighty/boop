using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public abstract class Panel : UIBehaviour, IUIElement
    {
        public event Action<EntityId> OnShow;
        public event Action<EntityId> OnHide;

        [SerializeField] protected Canvas _canvas;

        public virtual void Show() => _ = ShowAsync();

        public virtual void Hide() => _ = HideAsync();

        public virtual void SetActive(bool active) => _canvas.enabled = active;

#pragma warning disable CS1998
        public virtual async Awaitable ShowAsync(CancellationToken token = default)
        {
            _canvas.enabled = true;
            OnShow?.Invoke(GetEntityId());
        }

        public virtual async Awaitable HideAsync(CancellationToken token = default)
        {
            _canvas.enabled = false;
            OnHide?.Invoke(GetEntityId());
        }
#pragma warning restore CS1998
    }
}
