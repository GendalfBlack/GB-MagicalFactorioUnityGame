using Unity.Mathematics;

namespace Core.Grid
{
    /// <summary>Pure grid math implementation. No UnityEngine.</summary>
    public sealed class GridService : IGrid
    {
        public GridSettings Settings { get; }

        public GridService(GridSettings settings)
        {
            Settings = settings;
        }

        public bool IsInside(GridCoord coord) => Settings.Bounds.Contains(coord);

        public GridCoord Clamp(GridCoord coord)
        {
            var v = Settings.Bounds.Clamp(coord.ToInt2());
            return GridCoord.FromInt2(v);
        }

        public GridCoord WorldToGrid(float2 world)
        {
            // Convert to local grid space (0,0 at origin)
            float2 local = (world - Settings.OriginWorld) / Settings.CellSize;

            // Use floor to map to cell index
            int x = (int)math.floor(local.x);
            int y = (int)math.floor(local.y);

            var coord = new GridCoord(x, y);
            return coord;
        }

        public float2 GridToWorldCenter(GridCoord coord)
        {
            // Cell center = origin + (coord + 0.5) * cellSize
            return Settings.OriginWorld + (new float2(coord.X + 0.5f, coord.Y + 0.5f) * Settings.CellSize);
        }

        public float2 SnapWorldToCellCenter(float2 world, bool clampToBounds = true)
        {
            var c = WorldToGrid(world);
            if (clampToBounds) c = Clamp(c);
            return GridToWorldCenter(c);
        }
    }
}
