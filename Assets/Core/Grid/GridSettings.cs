using System;
using Unity.Mathematics;

namespace Core.Grid
{
    /// <summary>Grid configuration used by IGrid.</summary>
    [Serializable]
    public readonly struct GridSettings
    {
        public readonly float CellSize;        // world units per cell
        public readonly float2 OriginWorld;    // world position of grid (0,0) corner
        public readonly GridRect Bounds;       // allowed coordinates

        public GridSettings(float cellSize, float2 originWorld, GridRect bounds)
        {
            if (cellSize <= 0f) throw new ArgumentOutOfRangeException(nameof(cellSize), "CellSize must be > 0");

            CellSize = cellSize;
            OriginWorld = originWorld;
            Bounds = bounds;
        }
    }
}
