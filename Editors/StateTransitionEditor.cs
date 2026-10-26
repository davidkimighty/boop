using UnityEditor;
using UnityEditorInternal;

namespace boop
{
    public abstract class StateTransitionEditor : Editor
    {
        private const string SettingsPropertyName = "_settings";

        private ReorderableList _settings;

        private void OnEnable()
        {
            _settings = new ReorderableList(serializedObject, serializedObject.FindProperty(SettingsPropertyName), true, true, true, true);
            _settings.drawHeaderCallback = rect => EditorGUI.LabelField(rect, _settings.serializedProperty.displayName);
            _settings.elementHeightCallback = index => EditorGUI.GetPropertyHeight(_settings.serializedProperty.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing;
            _settings.drawElementCallback = (rect, index, active, focused) => EditorGUI.PropertyField(rect, _settings.serializedProperty.GetArrayElementAtIndex(index));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, SettingsPropertyName);
            _settings.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ColorTransition))]
    public class ColorTransitionEditor : StateTransitionEditor { }

    [CustomEditor(typeof(TransformTransition))]
    public class TransformTransitionEditor : StateTransitionEditor { }
}
