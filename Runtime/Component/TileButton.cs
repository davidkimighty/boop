#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace boop
{
    public class TileButton : Button
    {
        public TileAnchor TargetAnchor;

        [SerializeField] private float _stiffness = 200f;
        [SerializeField] private float _damping = 20f;

        private Vector2 _velocity;

        private void LateUpdate()
        {
            if (TargetAnchor == null) return;

            Vector2 current = transform.position;
            Vector2 targetPos = TargetAnchor.transform.position;

            Vector2 displacement = current - targetPos;
            Vector2 springForce = -_stiffness * displacement;
            Vector2 dampingForce = -_damping * _velocity;
            Vector2 force = springForce + dampingForce;

            _velocity += force * Time.deltaTime;
            current += _velocity * Time.deltaTime;
            transform.position = current;
        }

#if UNITY_EDITOR
        [ContextMenu("Apply Changes")]
        public void ApplyChanges()
        {
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.sizeDelta = ((RectTransform)TargetAnchor.transform).sizeDelta;
            transform.position = TargetAnchor.transform.position;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
