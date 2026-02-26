using Core.Grid;
using Scenes;
using Unity.Mathematics;
using UnityEngine;

namespace Gameplay.Player
{
    public sealed class PlayerGridProbe : MonoBehaviour
    {
        [SerializeField] private CompositionRoot root;

        public GridCoord CurrentCoord { get; private set; }

        private void Awake()
        {
            if (root == null) root = FindFirstObjectByType<CompositionRoot>();
        }

        private void Update()
        {
            if (root == null || root.Grid == null) return;

            var p = (Vector2)transform.position;
            CurrentCoord = root.Grid.WorldToGrid(new float2(p.x, p.y));
        }

        private void OnDrawGizmosSelected()
        {
            if (root == null || root.Grid == null) return;

            var center = root.Grid.GridToWorldCenter(CurrentCoord);
            Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.6f);
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y, 0), 0.15f);
        }
    }
}
