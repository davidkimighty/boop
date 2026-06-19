using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace boop
{
    public class TileGrid : LayoutGroup
    {
        [SerializeField] private int _columns = 10;
        [SerializeField] private float _unitSize = 50f;
        [SerializeField] private float _spacing = 5f;
#if UNITY_EDITOR
        [SerializeField] private bool _drawGizmos = false;
        [SerializeField] private Color _gizmoColor = new(0f, 1f, 1f, 0.5f);
        private readonly List<(Vector2 pos, Vector2 size)> _gizmoCells = new();
#endif
        private List<List<bool>> _grid = new();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            LayoutBlocks();
        }

        public override void CalculateLayoutInputVertical()
        {
            LayoutBlocks();
        }

        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;

            var rect = (RectTransform)transform;
            Rect r = rect.rect;

            Gizmos.color = _gizmoColor;

            foreach (var (pos, size) in _gizmoCells)
            {
                Vector3 topLeft = rect.TransformPoint(new Vector3(r.xMin + pos.x, r.yMax - pos.y));
                Vector3 topRight = rect.TransformPoint(new Vector3(r.xMin + pos.x + size.x, r.yMax - pos.y));
                Vector3 bottomRight = rect.TransformPoint(new Vector3(r.xMin + pos.x + size.x, r.yMax - pos.y - size.y));
                Vector3 bottomLeft = rect.TransformPoint(new Vector3(r.xMin + pos.x, r.yMax - pos.y - size.y));

                Gizmos.DrawLine(topLeft, topRight);
                Gizmos.DrawLine(topRight, bottomRight);
                Gizmos.DrawLine(bottomRight, bottomLeft);
                Gizmos.DrawLine(bottomLeft, topLeft);
            }
        }
#endif

        private void LayoutBlocks()
        {
            _grid.Clear();
            float _maxX = 0f;
            float _maxY = 0f;

            List<(RectTransform child, Vector2 position, Vector2 size)> layoutData = new();

            foreach (RectTransform child in rectChildren)
            {
                if (!child.gameObject.activeSelf) continue;

                var span = child.GetComponent<ITileSpan>();
                int w = Mathf.Clamp(span?.Width ?? 1, 1, _columns);
                int h = Mathf.Max(span?.Height ?? 1, 1);

                Vector2Int gridPos = FindSpace(w, h);
                MarkGrid(gridPos, w, h);

                float width = w * _unitSize + (w - 1) * _spacing;
                float height = h * _unitSize + (h - 1) * _spacing;
                Vector2 size = new(width, height);

                float x = gridPos.x * (_unitSize + _spacing);
                float y = gridPos.y * (_unitSize + _spacing);
                Vector2 pos = new(x, y);

                _maxX = Mathf.Max(_maxX, pos.x + width);
                _maxY = Mathf.Max(_maxY, pos.y + height);

                layoutData.Add((child, pos, size));
            }

            Vector2 bounds = new(_maxX, _maxY);
            Vector2 offset = new(GetStartOffset(0, bounds.x), GetStartOffset(1, bounds.y));

#if UNITY_EDITOR
            _gizmoCells.Clear();
#endif
            foreach (var (child, position, size) in layoutData)
            {
                SetChildAlongAxis(child, 0, offset.x + position.x, size.x);
                SetChildAlongAxis(child, 1, offset.y + position.y, size.y);
#if UNITY_EDITOR
                _gizmoCells.Add((new Vector2(offset.x + position.x, offset.y + position.y), size));
#endif
            }
        }

        private Vector2Int FindSpace(int w, int h)
        {
            for (int row = 0; ; row++)
            {
                InitializeRow(row + h - 1);

                for (int col = 0; col <= _columns - w; col++)
                {
                    if (CanFit(col, row, w, h))
                        return new Vector2Int(col, row);
                }
            }
        }

        private bool CanFit(int col, int row, int w, int h)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                    if (_grid[row + y][col + x]) return false;
            }
            return true;
        }

        private void MarkGrid(Vector2Int pos, int w, int h)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                    _grid[pos.y + y][pos.x + x] = true;
            }
        }

        private void InitializeRow(int rowIndex)
        {
            while (_grid.Count <= rowIndex)
            {
                List<bool> row = new();
                for (int i = 0; i < _columns; i++)
                    row.Add(false);
                _grid.Add(row);
            }
        }
    }
}
