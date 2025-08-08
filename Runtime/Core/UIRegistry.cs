using System;
using System.Collections.Generic;

namespace boop
{
    public class UIRegistry
    {
        private Dictionary<Type, List<IView>> _views = new();
        private Dictionary<Guid, IView> _viewsById = new();
        private Stack<IView> _viewStack = new();

        public Guid Register(IView view)
        {
            var id = Guid.NewGuid();
            view.AssignId(id);

            Type viewType = view.GetType();
            if (!_views.TryGetValue(viewType, out List<IView> list))
                list = _views[viewType] = new List<IView>();

            list.Add(view);
            _viewsById[id] = view;
            view.Hide();
            return id;
        }

        public void Unregister(IView view)
        {
            _views.Remove(view.GetType());
        }

        public void ShowView(Guid id)
        {
            if (!_viewsById.TryGetValue(id, out IView view)) return;

            if (_viewStack.Count > 0)
                _viewStack.Peek().Hide();

            _viewStack.Push(view);
            view.Show();
        }

        public void ShowView(Type type)
        {
            if (!_views.TryGetValue(type, out var list) || list.Count == 0) return;
            ShowView(list[0].Id);
        }

        public void HideView(Guid id)
        {
            if (!_viewsById.TryGetValue(id, out IView view)) return;

            if (_viewStack.Count > 0 && _viewStack.Peek() == view)
                _viewStack.Pop();

            view.Hide();
        }

        public void HideView(Type type)
        {
            if (!_views.TryGetValue(type, out var list) || list.Count == 0) return;
            HideView(list[0].Id);
        }

        public void Back()
        {
            if (_viewStack.Count == 0) return;

            _viewStack.Pop().Hide();

            if (_viewStack.Count > 0)
                _viewStack.Peek().Show();
        }

        public void HideAll()
        {
            while (_viewStack.Count > 0)
                _viewStack.Pop().Hide();
        }
    }
}
