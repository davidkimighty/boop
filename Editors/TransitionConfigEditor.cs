using UnityEditor;

namespace boop
{
    public abstract class TransitionConfigEditor : Editor
    {
        private TransitionConfigGUI _configGUI;

        private void OnEnable()
        {
            _configGUI = new TransitionConfigGUI(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            _configGUI.DrawLayout();
        }
    }

    [CustomEditor(typeof(ColorTransitionConfig))]
    public class ColorTransitionConfigEditor : TransitionConfigEditor { }

    [CustomEditor(typeof(TransformTransitionConfig))]
    public class TransformTransitionConfigEditor : TransitionConfigEditor { }
}
