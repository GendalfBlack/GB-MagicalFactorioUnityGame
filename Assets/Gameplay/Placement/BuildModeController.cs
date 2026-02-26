using Core.Grid;
using Core.Placement;
using Gameplay.Input;
using Gameplay.Player;
using Scenes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.tvOS;

namespace Gameplay.Placement
{
    public sealed class BuildModeController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private CompositionRoot root;
        [SerializeField] private PlayerGridProbe playerProbe;
        [SerializeField] private Camera cam;

        [Header("Rules")]
        [Min(0)][SerializeField] private int buildRadius = 2;

        [Header("Element")]
        [SerializeField] private ElementType selectedType = ElementType.Water;

        public GridCoord TargetCoord { get; private set; }
        public bool TargetInRange { get; private set; }
        public bool CanPlaceHere { get; private set; }

        private void Awake()
        {
            if (root == null) root = FindFirstObjectByType<CompositionRoot>();
            if (playerProbe == null) playerProbe = FindFirstObjectByType<PlayerGridProbe>();
            if (cam == null) cam = Camera.main;
        }

        private void Update()
        {
            if (root == null || root.Grid == null || root.Placement == null || cam == null || playerProbe == null)
                return;

            // 1) ¬изначаЇмо кл≥тинку п≥д мишею
            Vector2 mw = MouseWorld.GetWorld2D(cam);
            TargetCoord = root.Grid.WorldToGrid(new float2(mw.x, mw.y));

            // 2) ѕерев≥р€Їмо Уприсутн≥стьФ Ч чи в рад≥ус≥ в≥д гравц€
            var pc = playerProbe.CurrentCoord;
            int dist = math.abs(TargetCoord.X - pc.X) + math.abs(TargetCoord.Y - pc.Y); // Manhattan
            TargetInRange = dist <= buildRadius;

            // 3) ѕерев≥рка Placement
            CanPlaceHere = TargetInRange && root.Placement.CanPlace(TargetCoord);

            // 4) Input
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (CanPlaceHere)
                {
                    if (root.Placement.TryPlace(selectedType, TargetCoord, out var node))
                    {
                        Debug.Log($"PLACED {node.Type} at {node.Coord} id={node.Id.Value}");
                    }
                }
            }

            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                if (root.Placement.TryRemove(TargetCoord, out var removed))
                {
                    Debug.Log($"REMOVED {removed.Type} at {removed.Coord} id={removed.Id.Value}");
                }
            }

            // ѕеремиканн€ типу дл€ тесту
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) selectedType = ElementType.Water;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) selectedType = ElementType.Fire;
        }

        private void OnDrawGizmos()
        {
            if (root == null || root.Grid == null) return;

            var center = root.Grid.GridToWorldCenter(TargetCoord);
            Gizmos.color = CanPlaceHere
                ? new Color(0.2f, 1f, 0.4f, 0.35f)
                : new Color(1f, 0.2f, 0.2f, 0.35f);

            Gizmos.DrawCube(new Vector3(center.x, center.y, 0f), Vector3.one * 0.22f);
        }
    }
}
