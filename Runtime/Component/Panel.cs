using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public abstract class Panel : UIBehaviour, IElement, IPanel, IView
    {
        [SerializeField] protected Canvas _canvas;

        public Canvas Canvas => _canvas;

        public virtual void Show()
        {
            Canvas.enabled = true;
        }

        public virtual void Hide()
        {
            Canvas.enabled = false;
        }

        public virtual void Initialize(IViewModel viewModel) { }
    }
}
