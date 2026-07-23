using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace boop
{
    public class UIRegistry
    {
        private Dictionary<EntityId, IUIElement> _byId = new();

        public void Register(EntityId id, IUIElement ui)
        {
            if (!_byId.ContainsKey(id))
                _byId[id] = ui;
        }

        public void Unregister(EntityId id)
        {
            _byId.Remove(id);
        }

        public void Show(EntityId id, bool bringToFront = true)
        {
            _ = ShowAsync(id, bringToFront);
        }

        public void Hide(EntityId id)
        {
            _ = HideAsync(id);
        }

        public async Awaitable ShowAsync(EntityId id, bool bringToFront = true, CancellationToken token = default)
        {
            if (!_byId.TryGetValue(id, out IUIElement element)) return;

            if (bringToFront)
            {
                // resort view order
            }

            await element.ShowAsync(token);
        }

        public async Awaitable HideAsync(EntityId id, CancellationToken token = default)
        {
            if (!_byId.TryGetValue(id, out IUIElement element)) return;

            await element.HideAsync(token);
            // resort view order
        }
    }
}
