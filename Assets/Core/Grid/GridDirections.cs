namespace Core.Grid
{
    public static class GridDirections
    {
        public static readonly GridCoord Up = new GridCoord(0, 1);
        public static readonly GridCoord Down = new GridCoord(0, -1);
        public static readonly GridCoord Left = new GridCoord(-1, 0);
        public static readonly GridCoord Right = new GridCoord(1, 0);
    }
}
