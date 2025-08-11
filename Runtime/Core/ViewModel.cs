using System;
using System.Collections.Generic;

namespace boop
{
    public abstract class ViewModel : IViewModel, IDisposable
    {
        public virtual void Dispose() { }

        protected bool SetProperty<T>(ref T field, T value, Action actionEvent = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            actionEvent?.Invoke();
            return true;
        }
    }

    public interface IViewModel { }
}
