using Core.Grid;
using System;

namespace Core.Placement
{
    public interface IPlacementService
    {
        event Action<PlacedNode> NodePlaced;
        event Action<PlacedNode> NodeRemoved;

        bool CanPlace(GridCoord coord);
        bool TryPlace(ElementType type, GridCoord coord, out PlacedNode node);
        bool TryRemove(GridCoord coord, out PlacedNode removed);
        bool TryGet(GridCoord coord, out PlacedNode node);
    }
}
