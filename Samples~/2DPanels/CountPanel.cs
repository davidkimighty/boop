using boop;
using System;
using UnityEngine;
using UnityEngine.UI;
using Button = boop.Button;

public class CountPanel : Panel
{
    public event Action OnCountClick;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _countButton;
    [SerializeField] private Text _countText;

    private CountViewModel _viewModel;

    public override void Initialize(IViewModel viewModel)
    {
        _viewModel = viewModel as CountViewModel;
        _viewModel.OnCountUpdated += HandleUpdateCount;
        Hide();

        _countButton.OnClick += () => OnCountClick?.Invoke();
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
        _countText.text = _viewModel.Count;
    }
}
