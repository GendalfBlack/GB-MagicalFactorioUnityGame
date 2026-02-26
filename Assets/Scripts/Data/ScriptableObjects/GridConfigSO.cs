using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Ritebound/Config/Grid Config", fileName = "GridConfig")]
    public sealed class GridConfigSO : ScriptableObject
    {
        [Header("World")]
        [Min(0.01f)] public float cellSize = 1f;
        public Vector2 originWorld = Vector2.zero;

        [Header("Bounds (grid coords)")]
        public int minX = 0;
        public int minY = 0;
        [Min(1)] public int width = 25;
        [Min(1)] public int height = 25;

        [Header("Gizmos")]
        public bool draw = true;
        public bool drawCellCenters = false;
        [Min(0.1f)] public float centerMarkerSize = 0.1f;
    }
}
