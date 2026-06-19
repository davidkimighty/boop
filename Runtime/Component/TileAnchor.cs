using UnityEngine;
using UnityEngine.UI;

namespace boop
{
    public class TileAnchor : MonoBehaviour, ITileSpan
    {
        [SerializeField] protected int _width = 1;
        [SerializeField] protected int _height = 1;

        public int Width
        {
            get => _width;
            set
            {
                value = Mathf.Max(value, 1);
                if (_width == value) return;
                _width = value;
                SetDirty();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                value = Mathf.Max(value, 1);
                if (_height == value) return;
                _height = value;
                SetDirty();
            }
        }

        protected void OnEnable() => SetDirty();
        protected void OnDisable() => SetDirty();
        protected void OnTransformParentChanged() => SetDirty();

#if UNITY_EDITOR
        protected void OnValidate()
        {
            _width = Mathf.Max(_width, 1);
            _height = Mathf.Max(_height, 1);
            SetDirty();
        }
#endif

        private void SetDirty()
        {
            if (!isActiveAndEnabled) return;
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
        }
    }
}
