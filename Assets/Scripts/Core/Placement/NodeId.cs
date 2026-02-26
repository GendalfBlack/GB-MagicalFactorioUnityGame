using System;

namespace Core.Placement
{
    public readonly struct NodeId : IEquatable<NodeId>
    {
        public readonly int Value;
        public NodeId(int value) => Value = value;

        public bool Equals(NodeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NodeId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => Value.ToString();

        public static bool operator ==(NodeId a, NodeId b) => a.Equals(b);
        public static bool operator !=(NodeId a, NodeId b) => !a.Equals(b);
    }
}
