using Core.Grid;
using System;
using System.Collections.Generic;

namespace Core.Placement
{
    public sealed class PlacementService : IPlacementService
    {
        public event Action<PlacedNode> NodePlaced;
        public event Action<PlacedNode> NodeRemoved;

        private readonly IGrid _grid;

        private readonly Dictionary<GridCoord, PlacedNode> _byCoord = new();
        private int _nextId = 1;

        public PlacementService(IGrid grid)
        {
            _grid = grid;
        }

        public bool CanPlace(GridCoord coord)
        {
            if (!_grid.IsInside(coord)) return false;
            return !_byCoord.ContainsKey(coord);
        }

        public bool TryPlace(ElementType type, GridCoord coord, out PlacedNode node)
        {
            node = default;
            if (!CanPlace(coord)) return false;

            var id = new NodeId(_nextId++);
            node = new PlacedNode(id, type, coord);
            _byCoord.Add(coord, node);

            NodePlaced?.Invoke(node);
            return true;
        }

        public bool TryRemove(GridCoord coord, out PlacedNode removed)
        {
            if (_byCoord.TryGetValue(coord, out removed))
            {
                _byCoord.Remove(coord);
                NodeRemoved?.Invoke(removed);
                return true;
            }
            return false;
        }

        public bool TryGet(GridCoord coord, out PlacedNode node) => _byCoord.TryGetValue(coord, out node);
    }
}
