using System;

namespace boop
{
    public abstract class ViewModel : IDisposable
    {
        public event Action OnDataChange;

        public virtual void Dispose()
        {
            OnDataChange = null;
        }

        protected void RaiseDataChange()
        {
            OnDataChange?.Invoke();
        }
    }
}
