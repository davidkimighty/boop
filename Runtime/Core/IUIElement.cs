using System;
using System.Threading;
using UnityEngine;

namespace boop
{
    public interface IUIElement
    {
        event Action<EntityId> OnShow;
        event Action<EntityId> OnHide;

        void Show();
        void Hide();
        void SetActive(bool active);
        Awaitable ShowAsync(CancellationToken token = default);
        Awaitable HideAsync(CancellationToken token = default);
    }
}
