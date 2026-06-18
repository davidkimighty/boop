using System.Collections.Generic;
using UnityEngine;

namespace boop
{
    public class ToggleGroup : MonoBehaviour
    {
        [SerializeField] private List<Toggle> _toggles = new();
        [SerializeField] private bool _deselectOthers = true;

        private void OnEnable()
        {
            if (_deselectOthers)
            {
                for (int i = 0; i < _toggles.Count; i++)
                {
                    if (_toggles[i] == null) continue;
                    _toggles[i].OnClick += DeselectOthers;
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _toggles.Count; i++)
            {
                if (_toggles[i] == null) continue;
                _toggles[i].OnClick -= DeselectOthers;
            }
        }

        public void Add(Toggle toggle)
        {
            if (_toggles.Contains(toggle)) return;

            if (_deselectOthers)
                toggle.OnClick += DeselectOthers;
            _toggles.Add(toggle);
        }

        public void Remove(Toggle toggle)
        {
            if (!_toggles.Contains(toggle)) return;

            toggle.OnClick -= DeselectOthers;
            _toggles.Remove(toggle);
        }

        public void DeselectAll()
        {
            for (int i = 0; i < _toggles.Count; i++)
                _toggles[i].SetToggle(false);
        }

        private void DeselectOthers(Toggle toggle, bool state)
        {
            if (!state) return;

            for (int i = 0; i < _toggles.Count; i++)
            {
                if (toggle == _toggles[i] || _toggles[i] == null) continue;
                _toggles[i].SetToggle(false);
            }
        }
    }
}
