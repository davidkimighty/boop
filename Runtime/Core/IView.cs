using UnityEngine;

namespace boop
{
    public interface IView
    {
        Canvas Canvas { get; }

        void Initialize(IViewModel viewModel);
        void Show();
        void Hide();
    }
}
