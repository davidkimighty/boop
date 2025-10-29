public class CountModel
{
    private int _count;

    public int Count => _count;

    public CountModel(int startCount)
    {
        _count = startCount;
    }

    public void Count0To9()
    {
        _count = (_count + 1) % 10;
    }
}
