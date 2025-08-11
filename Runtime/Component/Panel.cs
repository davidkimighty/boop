using System;
using UnityEngine;

namespace boop
{
    public abstract class Panel : MonoBehaviour, IView
    {
        public Guid Id { get; private set; }

        public abstract void Initialize(IViewModel viewModel);

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        void IView.AssignId(Guid id)
        {
            Id = id;
        }
    }
}
