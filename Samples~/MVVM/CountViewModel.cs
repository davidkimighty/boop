using boop;

public class CountViewModel : ViewModel
{
    private CountModel _countModel;

    public string Count => _countModel.Count.ToString();

    public CountViewModel(CountModel countModel)
    {
        _countModel = countModel;
    }

    public void Count0To9()
    {
        _countModel.Count0To9();
        InvokeDataChange(nameof(Count));
    }
}
