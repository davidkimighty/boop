using System.Collections.Generic;
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
            if (!_byId.TryGetValue(id, out IUIElement element)) return;

            if (bringToFront)
            {
                // resort view order
            }

            element.Show();
        }

        public void Hide(EntityId id)
        {
            if (!_byId.TryGetValue(id, out IUIElement element)) return;

            element.Hide();
            // resort view order
        }
    }
}
