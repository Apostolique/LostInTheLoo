using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MonoGame.Extended;

namespace GameProject
{
    /// <summary>
    /// Represents an organism, scent or the like.
    /// </summary>
    public class Entity : IComparable<Entity>
    {
        public RenderLogic RenderLogic;
        public UpdateLogic UpdateLogic;
        public Segment[] Segments;
        public RectangleF AABB;
        public float Z;
        public int Leaf;

        public int CompareTo(Entity? value) {
            if (value == null) return 1;
            int compareZ = Z.CompareTo(value.Z);
            return compareZ == 0 ? Leaf.CompareTo(value.Leaf) : compareZ;
        }
        public int GetHashCode([DisallowNull] Entity obj) {
            return obj.Leaf.GetHashCode();
        }
        public bool Equals([AllowNull] Entity a, [AllowNull] Entity b) {
            if (a == null && b == null) return true;
            else if (a == null || b == null) return false;
            else return a.Leaf == b.Leaf;
        }
    }
}
