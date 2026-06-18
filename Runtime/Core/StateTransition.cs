using System;
using UnityEngine;

namespace boop
{
    public abstract class StateTransition : MonoBehaviour, IStateTransition
    {
        public abstract void Transition(Interactable.State state, bool selected, bool instant = false);
    }

    [Serializable]
    public struct StateTarget<T>
    {
        public Interactable.State State;
        public T Value;
        public float Duration;
        public AnimationCurve Curve;
    }
}