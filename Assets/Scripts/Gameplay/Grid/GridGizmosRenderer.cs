using Core.Grid;
using GameData;
using Unity.Mathematics;
using UnityEngine;

namespace Gameplay.Grid
{
    [ExecuteAlways]
    public sealed class GridGizmosRenderer : MonoBehaviour
    {
        [SerializeField] private GridConfigSO gridConfig;

        private void OnDrawGizmos()
        {
            if (gridConfig == null || !gridConfig.draw) return;

            // Build a temporary grid from config (для Gizmos це ок).
            var bounds = new GridRect(gridConfig.minX, gridConfig.minY, gridConfig.width, gridConfig.height);
            var settings = new GridSettings(
                gridConfig.cellSize,
                new float2(gridConfig.originWorld.x, gridConfig.originWorld.y),
                bounds
            );

            DrawGrid(settings);
        }

        private void DrawGrid(GridSettings settings)
        {
            Gizmos.matrix = Matrix4x4.identity;

            // Дрібний мінімалізм: сітка + рамка.
            var cs = settings.CellSize;
            var o = settings.OriginWorld;
            var b = settings.Bounds;

            // Border
            Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
            var min = new Vector3(o.x + b.MinX * cs, o.y + b.MinY * cs, 0f);
            var max = new Vector3(o.x + b.MaxXExclusive * cs, o.y + b.MaxYExclusive * cs, 0f);

            Gizmos.DrawLine(min, new Vector3(max.x, min.y, 0f));
            Gizmos.DrawLine(new Vector3(max.x, min.y, 0f), max);
            Gizmos.DrawLine(max, new Vector3(min.x, max.y, 0f));
            Gizmos.DrawLine(new Vector3(min.x, max.y, 0f), min);

            // Grid lines
            Gizmos.color = new Color(1f, 1f, 1f, 0.12f);

            // Vertical lines
            for (int x = b.MinX; x <= b.MaxXExclusive; x++)
            {
                float wx = o.x + x * cs;
                Gizmos.DrawLine(
                    new Vector3(wx, o.y + b.MinY * cs, 0f),
                    new Vector3(wx, o.y + b.MaxYExclusive * cs, 0f)
                );
            }

            // Horizontal lines
            for (int y = b.MinY; y <= b.MaxYExclusive; y++)
            {
                float wy = o.y + y * cs;
                Gizmos.DrawLine(
                    new Vector3(o.x + b.MinX * cs, wy, 0f),
                    new Vector3(o.x + b.MaxXExclusive * cs, wy, 0f)
                );
            }

            if (gridConfig.drawCellCenters)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.18f);
                float s = gridConfig.centerMarkerSize;

                for (int y = b.MinY; y < b.MaxYExclusive; y++)
                    for (int x = b.MinX; x < b.MaxXExclusive; x++)
                    {
                        var center = new Vector3(o.x + (x + 0.5f) * cs, o.y + (y + 0.5f) * cs, 0f);
                        Gizmos.DrawLine(center + Vector3.left * s, center + Vector3.right * s);
                        Gizmos.DrawLine(center + Vector3.up * s, center + Vector3.down * s);
                    }
            }
        }
    }
}
