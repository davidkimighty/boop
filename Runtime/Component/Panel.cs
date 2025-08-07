using UnityEngine;

namespace boop
{
    public abstract class Panel : MonoBehaviour, IUIElement
    {
        public virtual void Initialize(ViewModel viewModel) { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }   
}
