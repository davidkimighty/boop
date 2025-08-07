namespace boop
{
    public interface IUIElement
    {
        void Initialize(ViewModel viewModel);
        void Show();
        void Hide();
    }
}
