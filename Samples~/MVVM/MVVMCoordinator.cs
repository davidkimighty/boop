using boop;
using UnityEngine;

public class MVVMCoordinator : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private UIEventChannel _uiEventChannel;
    [SerializeField] private CountPanel _panel;

    private void Start()
    {
        _panel.Initialize(new CountViewModel(new CountModel(0)));
        _uiManager.Register("CountPanel", _panel);
    }
}
