using boop;
using UnityEngine;

public class CountBootstrapper : MonoBehaviour
{
    [SerializeField] private CountPanel _panel1;
    [SerializeField] private CountPanel _panel2;
    [SerializeField] private CountPanel _panel3;
    [SerializeField] private Toggle _toggleButton1;
    [SerializeField] private Toggle _toggleButton2;
    [SerializeField] private Toggle _toggleButton3;

    private void Start()
    {
        _panel1.Initialize(new CountViewModel(new CountModel()));
        _panel2.Initialize(new CountViewModel(new CountModel()));
        _panel3.Initialize(new CountViewModel(new CountModel()));

        _toggleButton1.OnClick += (toggle) => HandleToggle(toggle, _panel1);
        _toggleButton2.OnClick += (toggle) => HandleToggle(toggle, _panel2);
        _toggleButton3.OnClick += (toggle) => HandleToggle(toggle, _panel3);
    }

    private void HandleToggle(bool toggle, CountPanel panel)
    {
        if (toggle)
            panel.Show();
        else
            panel.Hide();
    }
}
