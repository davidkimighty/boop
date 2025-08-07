using boop;
using UnityEngine;
using UnityEngine.UI;
using Button = boop.Button;

public class CountPanel : Panel
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _countButton;
    [SerializeField] private Text _countText;

    private CountViewModel _viewModel;

    private void Start()
    {
        Hide();
    }

    public override void Initialize(ViewModel viewModel)
    {
        _viewModel = viewModel as CountViewModel;
        _viewModel.OnDataChange += HandleUpdateCount;
        _countButton.OnClick += HandleCount;
    }

    public override void Show()
    {
        _canvas.enabled = true;
    }

    public override void Hide()
    {
        _canvas.enabled = false;
    }

    private void HandleUpdateCount()
    {
        _countText.text = _viewModel.Count.ToString();
    }

    private void HandleCount()
    {
        _viewModel.Count0To9();
    }
}
