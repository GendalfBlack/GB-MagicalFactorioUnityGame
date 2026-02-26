using System;
using Unity.Mathematics;

namespace Core.Grid
{
    /// <summary>Rectangular bounds in grid coordinates.</summary>
    [Serializable]
    public readonly struct GridRect
    {
        public readonly int MinX;
        public readonly int MinY;
        public readonly int Width;
        public readonly int Height;

        public int MaxXExclusive => MinX + Width;
        public int MaxYExclusive => MinY + Height;

        public GridRect(int minX, int minY, int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be > 0");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be > 0");

            MinX = minX;
            MinY = minY;
            Width = width;
            Height = height;
        }

        public bool Contains(GridCoord c) =>
            c.X >= MinX && c.X < MaxXExclusive &&
            c.Y >= MinY && c.Y < MaxYExclusive;

        public int2 Clamp(int2 v)
        {
            int x = math.clamp(v.x, MinX, MaxXExclusive - 1);
            int y = math.clamp(v.y, MinY, MaxYExclusive - 1);
            return new int2(x, y);
        }

        public override string ToString() => $"GridRect(min=({MinX},{MinY}), size=({Width},{Height}))";
    }
}
