using Core.Grid;
using Core.Placement;
using GameData;
using Unity.Mathematics;
using UnityEngine;

namespace Scenes
{
    public sealed class CompositionRoot : MonoBehaviour
    {
        [SerializeField] private GridConfigSO gridConfig;

        public IGrid Grid { get; private set; }
        public IPlacementService Placement { get; private set; }

        private void Awake()
        {
            if (gridConfig == null)
            {
                Debug.LogError("[CompositionRoot] GridConfigSO is not assigned.");
                enabled = false;
                return;
            }

            var bounds = new GridRect(
                gridConfig.minX,
                gridConfig.minY,
                gridConfig.width,
                gridConfig.height
            );

            var settings = new GridSettings(
                gridConfig.cellSize,
                new float2(gridConfig.originWorld.x, gridConfig.originWorld.y),
                bounds
            );

            Grid = new GridService(settings);

            Placement = new PlacementService(Grid);

            // NOTE: Пізніше тут будуть створюватися й інші Core сервіси (Placement, Linking, Ritual...)
            Debug.Log($"[CompositionRoot] Grid initialized: {settings.Bounds}, cell={settings.CellSize}");
        }
    }
}
