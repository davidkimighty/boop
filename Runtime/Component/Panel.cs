using UnityEngine;
using UnityEngine.EventSystems;

namespace boop
{
    public abstract class Panel : UIBehaviour, IElement, IView
    {
        [SerializeField] protected Canvas _canvas;

        public Canvas Canvas => _canvas;

        public abstract void Initialize(IViewModel viewModel);

        public virtual void Show()
        {
            Canvas.enabled = true;
        }

        public virtual void Hide()
        {
            Canvas.enabled = false;
        }
    }
}
