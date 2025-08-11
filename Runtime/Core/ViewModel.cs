using System;
using System.Collections.Generic;

namespace boop
{
    public abstract class ViewModel : IViewModel, IDisposable
    {
        public event Action OnDataChange;

        public virtual void Dispose() { }

        protected bool SetProperty<T>(ref T field, T value, Action propertyEvent = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            propertyEvent?.Invoke();
            OnDataChange?.Invoke();
            return true;
        }

        protected bool SetProperty<T>(ref T field, T value, Action<T> propertyEvent = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            propertyEvent?.Invoke(value);
            OnDataChange?.Invoke();
            return true;
        }
    }

    public interface IViewModel
    {
        event Action OnDataChange;
    }
}
