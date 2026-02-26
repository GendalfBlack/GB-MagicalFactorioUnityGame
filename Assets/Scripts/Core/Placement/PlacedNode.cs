using Core.Grid;

namespace Core.Placement
{
    public readonly struct PlacedNode
    {
        public readonly NodeId Id;
        public readonly ElementType Type;
        public readonly GridCoord Coord;

        public PlacedNode(NodeId id, ElementType type, GridCoord coord)
        {
            Id = id;
            Type = type;
            Coord = coord;
        }
    }
}
