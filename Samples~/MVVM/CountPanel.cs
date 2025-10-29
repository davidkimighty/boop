using boop;
using UnityEngine;
using UnityEngine.UI;
using Button = boop.Button;

public class CountPanel : Panel
{
    [SerializeField] private Button _countButton;
    [SerializeField] private Text _countText;

    private CountViewModel _viewModel;

    public override void Initialize(IViewModel viewModel)
    {
        _viewModel = viewModel as CountViewModel;
        if (viewModel == null) return;

        _countButton.OnClick += () => _viewModel.Count0To9();

        _viewModel.OnDataChange += Bind;

        Bind();
    }

    public override void Show()
    {
        Bind();
        base.Show();
    }

    private void Bind(string propertyName = null)
    {
        _countText.text = _viewModel.Count;
    }
}
