using Unity.Mathematics;

namespace Core.Grid
{
    public interface IGrid
    {
        GridSettings Settings { get; }

        /// <summary>World position (float2) to grid coord (cell index).</summary>
        GridCoord WorldToGrid(float2 world);

        /// <summary>Grid cell center to world position.</summary>
        float2 GridToWorldCenter(GridCoord coord);

        /// <summary>Snap world position to nearest cell center.</summary>
        float2 SnapWorldToCellCenter(float2 world, bool clampToBounds = true);

        /// <summary>True if coord is inside grid bounds.</summary>
        bool IsInside(GridCoord coord);

        /// <summary>Clamp coord into bounds.</summary>
        GridCoord Clamp(GridCoord coord);
    }
}
