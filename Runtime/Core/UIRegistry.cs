using System;
using System.Collections.Generic;

namespace boop
{
    public class UIRegistry
    {
        public event Action<IView> OnShowView;
        public event Action<IView> OnHideView;

        private Dictionary<string, IElement> _elements = new();
        private List<IView> _activeViews = new();

        public void Register(string id, IElement ui)
        {
            if (!_elements.ContainsKey(id))
                _elements[id] = ui;
        }

        public void Unregister(string id)
        {
            _elements.Remove(id);
        }

        public void ShowView(string id, bool bringToFront = true)
        {
            if (!_elements.TryGetValue(id, out IElement element)) return;

            var view = element as IView;
            if (view == null) return;

            if (bringToFront)
            {
                // resort view order
            }

            _activeViews.Add(view);
            view.Show();
            OnShowView?.Invoke(view);
        }

        public void HideView(string id)
        {
            if (!_elements.TryGetValue(id, out IElement element)) return;

            var view = element as IView;
            if (view == null) return;

            view.Hide();
            _activeViews.Remove(view);
            // resort view order
            OnHideView?.Invoke(view);
        }

        public void HideAll()
        {
            foreach (IView view in _activeViews)
            {
                view.Hide();
                OnHideView?.Invoke(view);
            }
            _activeViews.Clear();
        }
    }

    public interface IElement
    {

    }
}
