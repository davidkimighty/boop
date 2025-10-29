using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace boop
{
    public abstract class ViewModel : IViewModel
    {
        public event Action<string> OnDataChange;

        protected void InvokeDataChange([CallerMemberName] string propertyName = null)
        {
            OnDataChange?.Invoke(propertyName);
        }

        protected bool SetProperty<T>(ref T backingField, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, newValue)) return false;
            backingField = newValue;
            InvokeDataChange(propertyName);
            return true;
        }
    }

    public interface IViewModel
    {
        event Action<string> OnDataChange;
    }
}
