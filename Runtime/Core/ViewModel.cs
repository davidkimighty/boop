using System;
using System.Collections.Generic;

namespace boop
{
    public abstract class ViewModel : IViewModel, IDisposable
    {
        public event Action OnDataChange;

        public virtual void Dispose()
        {
            OnDataChange = null;
        }

        protected bool SetProperty<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnDataChange?.Invoke();
            return true;
        }
    }

    public interface IViewModel
    {
        event Action OnDataChange;
    }
}
