using Core.Placement;
using UnityEngine;

namespace Gameplay.Placement
{
    public sealed class NodeView : MonoBehaviour
    {
        public int NodeId { get; private set; }
        public ElementType Type { get; private set; }

        public void Initialize(NodeId id, ElementType type)
        {
            NodeId = id.Value;
            Type = type;
        }
    }
}
