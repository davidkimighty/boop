using boop;
using System;

public class CountViewModel : ViewModel
{
    public event Action OnCountUpdated;

    private CountModel _countModel;
    private string _count;

    public string Count
    {
        get => _count;
        set => SetProperty(ref _count, value, OnCountUpdated);
    }

    public CountViewModel(CountModel countModel)
    {
        _countModel = countModel;
        UpdateCount();
    }

    public void Count0To9()
    {
        _countModel.Count0To9();
        UpdateCount();
    }

    private void UpdateCount()
    {
        Count = _countModel.Count.ToString();
    }
}
