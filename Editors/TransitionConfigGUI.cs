using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace boop
{
    public class TransitionConfigGUI
    {
        private const float StateWidth = 80f;
        private const float DurationWidth = 48f;
        private const float CurveWidth = 60f;
        private const float ColumnGap = 4f;
        private const float RowPadding = 2f;
        private const float HeaderOffset = ReorderableList.Defaults.dragHandleWidth - ReorderableList.Defaults.padding;

        private static readonly string[] ListNames = { nameof(ColorTransitionConfig.Normal), nameof(ColorTransitionConfig.Selected) };
        private static readonly TransformChannel[] Channels = { TransformChannel.Position, TransformChannel.Rotation, TransformChannel.Scale };
        private static readonly string[] ChannelNames = { nameof(TransformValue.Position), nameof(TransformValue.Rotation), nameof(TransformValue.Scale) };
        private static readonly GUIContent ValueLabel = new(nameof(StateTarget<Color>.Value));
        private static readonly GUIContent DurationLabel = new(nameof(StateTarget<Color>.Duration));
        private static readonly GUIContent CurveLabel = new(nameof(StateTarget<Color>.Curve));

        private readonly SerializedObject _serializedObject;
        private readonly ReorderableList[] _lists;

        public TransitionConfigGUI(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _lists = new ReorderableList[ListNames.Length];
            for (int i = 0; i < ListNames.Length; ++i)
            {
                ReorderableList list = new(serializedObject, serializedObject.FindProperty(ListNames[i]), true, true, true, true);
                list.drawHeaderCallback = rect => DrawHeader(rect, list.serializedProperty.displayName);
                list.elementHeightCallback = index => GetRowHeight(list.serializedProperty.GetArrayElementAtIndex(index));
                list.drawElementCallback = (rect, index, active, focused) => DrawRow(rect, list.serializedProperty.GetArrayElementAtIndex(index));
                _lists[i] = list;
            }
        }

        public Object Config => _serializedObject.targetObject;

        private static float LineStep => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public float GetHeight()
        {
            _serializedObject.Update();
            float height = 0f;
            for (int i = 0; i < _lists.Length; ++i)
                height += _lists[i].GetHeight() + EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public void Draw(Rect rect)
        {
            _serializedObject.Update();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < _lists.Length; ++i)
            {
                float height = _lists[i].GetHeight();
                _lists[i].DoList(new Rect(rect.x, rect.y, rect.width, height));
                rect.y += height + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel = indentLevel;
            _serializedObject.ApplyModifiedProperties();
        }

        public void DrawLayout()
        {
            Draw(EditorGUILayout.GetControlRect(false, GetHeight()));
        }

        private static void DrawHeader(Rect rect, string title)
        {
            rect.xMin += HeaderOffset;
            GetColumns(rect, out Rect state, out Rect value, out Rect duration, out Rect curve);
            EditorGUI.LabelField(state, title, EditorStyles.boldLabel);
            EditorGUI.LabelField(value, ValueLabel, EditorStyles.miniLabel);
            EditorGUI.LabelField(duration, DurationLabel, EditorStyles.miniLabel);
            EditorGUI.LabelField(curve, CurveLabel, EditorStyles.miniLabel);
        }

        private static float GetRowHeight(SerializedProperty element)
        {
            SerializedProperty channels = element.FindPropertyRelative(nameof(StateTarget<TransformValue>.Value)).FindPropertyRelative(nameof(TransformValue.Channels));
            int lines = 1;
            if (channels != null)
            {
                for (int i = 0; i < Channels.Length; ++i)
                {
                    if ((channels.intValue & (int)Channels[i]) != 0)
                        lines++;
                }
            }
            return lines * LineStep + RowPadding;
        }

        private static void DrawRow(Rect rect, SerializedProperty element)
        {
            SerializedProperty value = element.FindPropertyRelative(nameof(StateTarget<Color>.Value));
            SerializedProperty channels = value.FindPropertyRelative(nameof(TransformValue.Channels));
            Rect line = new Rect(rect.x, rect.y + RowPadding, rect.width, EditorGUIUtility.singleLineHeight);
            GetColumns(line, out Rect state, out Rect valueRect, out Rect duration, out Rect curve);

            EditorGUI.PropertyField(state, element.FindPropertyRelative(nameof(StateTarget<Color>.State)), GUIContent.none);
            EditorGUI.PropertyField(valueRect, channels ?? value, GUIContent.none);
            EditorGUI.PropertyField(duration, element.FindPropertyRelative(nameof(StateTarget<Color>.Duration)), GUIContent.none);
            EditorGUI.PropertyField(curve, element.FindPropertyRelative(nameof(StateTarget<Color>.Curve)), GUIContent.none);
            if (channels == null) return;

            for (int i = 0; i < Channels.Length; ++i)
            {
                if ((channels.intValue & (int)Channels[i]) == 0) continue;

                line.y += LineStep;
                SerializedProperty vector = value.FindPropertyRelative(ChannelNames[i]);
                EditorGUI.LabelField(new Rect(state.x, line.y, state.width, line.height), vector.displayName, EditorStyles.miniLabel);
                EditorGUI.PropertyField(Rect.MinMaxRect(valueRect.x, line.y, curve.xMax, line.yMax), vector, GUIContent.none);
            }
        }

        private static void GetColumns(Rect line, out Rect state, out Rect value, out Rect duration, out Rect curve)
        {
            state = new Rect(line.x, line.y, StateWidth, line.height);
            curve = new Rect(line.xMax - CurveWidth, line.y, CurveWidth, line.height);
            duration = new Rect(curve.x - ColumnGap - DurationWidth, line.y, DurationWidth, line.height);
            value = Rect.MinMaxRect(state.xMax + ColumnGap, line.y, duration.x - ColumnGap, line.yMax);
        }
    }
}
