using System;
using UnityEngine;

namespace boop
{
    public interface IUIElement
    {
        event Action<EntityId> OnShow;
        event Action<EntityId> OnHide;

        void Show();
        void Hide();
    }
}
