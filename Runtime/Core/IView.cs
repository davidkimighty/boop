using System;

namespace boop
{
    public interface IView
    {
        Guid Id { get; }

        void Initialize(IViewModel viewModel);
        void Show();
        void Hide();

        internal void AssignId(Guid id);
    }
}
