using boop;

public class CountViewModel : ViewModel
{
    private CountModel _countModel;
    private int _count = 0;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            RaiseDataChange();
        }
    }

    public CountViewModel(CountModel countModel)
    {
        _countModel = countModel;
        _count = _countModel.Count;
    }

    public void Count0To9()
    {
        Count = (_count + 1) % 10;
        _countModel.Count = Count;
    }
}
