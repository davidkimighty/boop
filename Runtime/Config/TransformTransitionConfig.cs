using System;
using UnityEngine;

namespace boop
{
    [Flags]
    public enum TransformChannel
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2,
    }

    [CreateAssetMenu(fileName = "Transition_Transform", menuName = "boop/Transition/Transform")]
    public class TransformTransitionConfig : ScriptableObject
    {
        public StateTarget<TransformValue>[] Normal;
        public StateTarget<TransformValue>[] Selected;
    }

    [Serializable]
    public struct TransformValue
    {
        public TransformChannel Channels;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}
