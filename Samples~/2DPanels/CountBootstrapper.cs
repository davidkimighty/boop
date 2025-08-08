using boop;
using UnityEngine;

public class CountBootstrapper : MonoBehaviour
{
    [SerializeField] private UIEventChannel _uiEventChannel;
    [SerializeField] private CountPanel _panel;
    [SerializeField] private Toggle _toggleButton;

    private void Start()
    {
        var viewModel = new CountViewModel(new CountModel());
        _panel.Initialize(viewModel);
        _panel.OnCountClick += () => viewModel.Count0To9();
        _uiEventChannel.RequestRegisterUI(_panel);

        _toggleButton.OnClick += HandleToggle;
    }
    
    private void HandleToggle(bool toggle)
    {
        if (toggle)
            _uiEventChannel.RequestShowUI<CountPanel>();
        else
            _uiEventChannel.RequestHideUI<CountPanel>();
    }
}
