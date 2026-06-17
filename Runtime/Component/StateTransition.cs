using UnityEngine;

namespace boop
{
    public abstract class StateTransition : MonoBehaviour, IStateTransition
    {
        public abstract void Transition(Interactable.State state, bool selected, bool instant = false);
    }
}