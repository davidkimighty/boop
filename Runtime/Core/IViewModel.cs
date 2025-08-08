using System;

namespace boop
{
    public interface IViewModel
    {
        event Action OnDataChange;
    }
}
