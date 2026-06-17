using System;
using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "StateColor", menuName = "boop/State Color")]
    public class StateColorConfig : ScriptableObject
    {
        public StateColor[] Normal;
        public StateColor[] Selected;
    }

    [Serializable]
    public struct StateColor
    {
        public Interactable.State State;
        public Color Color;
        public float Duration;
        public AnimationCurve Curve;
    }
}