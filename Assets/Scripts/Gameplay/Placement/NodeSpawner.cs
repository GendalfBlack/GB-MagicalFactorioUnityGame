using System.Collections.Generic;
using Core.Placement;
using Scenes;
using UnityEngine;

namespace Gameplay.Placement
{
    public sealed class NodeSpawner : MonoBehaviour
    {
        [SerializeField] private CompositionRoot root;

        [Header("Prefabs")]
        [SerializeField] private GameObject waterNodePrefab;
        [SerializeField] private GameObject fireNodePrefab;

        [Header("Hierarchy")]
        [SerializeField] private Transform nodesParent;

        private readonly Dictionary<int, GameObject> _spawned = new();

        private void Awake()
        {
            if (root == null) root = FindFirstObjectByType<CompositionRoot>();
            if (nodesParent == null) nodesParent = transform;
        }

        private void OnEnable()
        {
            if (root?.Placement == null) return;
            root.Placement.NodePlaced += OnNodePlaced;
            root.Placement.NodeRemoved += OnNodeRemoved;
        }

        private void OnDisable()
        {
            if (root?.Placement == null) return;
            root.Placement.NodePlaced -= OnNodePlaced;
            root.Placement.NodeRemoved -= OnNodeRemoved;
        }

        private void OnNodePlaced(PlacedNode node)
        {
            var prefab = node.Type switch
            {
                ElementType.Water => waterNodePrefab,
                ElementType.Fire => fireNodePrefab,
                _ => null
            };

            if (prefab == null)
            {
                Debug.LogError($"[NodeSpawner] Missing prefab for {node.Type}");
                return;
            }

            var center = root.Grid.GridToWorldCenter(node.Coord);
            var go = Instantiate(prefab, new Vector3(center.x, center.y, 0f), Quaternion.identity, nodesParent);

            var view = go.GetComponent<NodeView>();
            if (view != null) view.Initialize(node.Id, node.Type);

            _spawned[node.Id.Value] = go;
        }

        private void OnNodeRemoved(PlacedNode node)
        {
            if (_spawned.TryGetValue(node.Id.Value, out var go) && go != null)
            {
                Destroy(go);
            }
            _spawned.Remove(node.Id.Value);
        }
    }
}
