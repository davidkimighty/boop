using UnityEngine;

namespace boop
{
    public interface IPanel
    {
        Canvas Canvas { get; }

        void Show();
        void Hide();
    }
}
