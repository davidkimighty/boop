using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace boop
{
    [CustomPropertyDrawer(typeof(ColorTransition.ColorSetting))]
    [CustomPropertyDrawer(typeof(TransformTransition.TransformSetting))]
    public class TransitionSettingDrawer : PropertyDrawer
    {
        private const float FieldGap = 4f;

        private readonly Dictionary<string, TransitionConfigGUI> _configGUIs = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            TransitionConfigGUI configGUI = GetConfigGUI(property);
            if (configGUI != null)
                height += EditorGUIUtility.standardVerticalSpacing + configGUI.GetHeight();
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty source = property.Copy();
            source.NextVisible(true);
            SerializedProperty config = property.FindPropertyRelative(nameof(ColorTransition.ColorSetting.Config));

            Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float halfWidth = (line.width - FieldGap) * 0.5f;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(new Rect(line.x, line.y, halfWidth, line.height), source, GUIContent.none);
            EditorGUI.PropertyField(new Rect(line.xMax - halfWidth, line.y, halfWidth, line.height), config, GUIContent.none);
            EditorGUI.indentLevel = indentLevel;

            TransitionConfigGUI configGUI = GetConfigGUI(property);
            if (configGUI == null) return;

            float top = line.yMax + EditorGUIUtility.standardVerticalSpacing;
            configGUI.Draw(Rect.MinMaxRect(position.x, top, position.xMax, position.yMax));
        }

        private TransitionConfigGUI GetConfigGUI(SerializedProperty property)
        {
            Object config = property.FindPropertyRelative(nameof(ColorTransition.ColorSetting.Config)).objectReferenceValue;
            if (config == null) return null;

            if (!_configGUIs.TryGetValue(property.propertyPath, out TransitionConfigGUI configGUI) || configGUI.Config != config)
            {
                configGUI = new TransitionConfigGUI(new SerializedObject(config));
                _configGUIs[property.propertyPath] = configGUI;
            }
            return configGUI;
        }
    }
}
