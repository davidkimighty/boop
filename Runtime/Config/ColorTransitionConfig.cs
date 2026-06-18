using UnityEngine;

namespace boop
{
    [CreateAssetMenu(fileName = "Transition_Color", menuName = "boop/Transition/Color")]
    public class ColorTransitionConfig : ScriptableObject
    {
        public StateTarget<Color>[] Normal;
        public StateTarget<Color>[] Selected;
    }
}