using System;
using System.Collections.Generic;

namespace boop
{
    public class UIRegistry
    {
        public event Action<IPanel> OnShowPanel;
        public event Action<IPanel> OnHidePanel;

        private Dictionary<string, IElement> _elements = new();
        private List<IPanel> _activePanels = new();

        public void Register(string id, IElement ui)
        {
            if (!_elements.ContainsKey(id))
                _elements[id] = ui;
        }

        public void Unregister(string id)
        {
            _elements.Remove(id);
        }

        public void ShowPanel(string id, bool bringToFront = true)
        {
            if (!_elements.TryGetValue(id, out IElement element)) return;

            var panel = element as IPanel;
            if (panel == null) return;

            if (bringToFront)
            {
                // resort view order
            }

            _activePanels.Add(panel);
            panel.Show();
            OnShowPanel?.Invoke(panel);
        }

        public void HidePanel(string id)
        {
            if (!_elements.TryGetValue(id, out IElement element)) return;

            var panel = element as IPanel;
            if (panel == null) return;

            panel.Hide();
            _activePanels.Remove(panel);
            // resort view order
            OnHidePanel?.Invoke(panel);
        }

        public void HideAll()
        {
            foreach (IPanel panel in _activePanels)
            {
                panel.Hide();
                OnHidePanel?.Invoke(panel);
            }
            _activePanels.Clear();
        }
    }

    public interface IElement
    {

    }
}
