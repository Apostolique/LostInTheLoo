using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoGame.Extended;

namespace GameProject
{
    /// <summary>
    /// Represents an organism, scent or the like.
    /// </summary>
    public class Entity
    {
        public RenderLogic RenderLogic;
        public UpdateLogic UpdateLogic;
        public Segment[] Segments;
        public RectangleF AABB;
        public float Z;
        public int Leaf;
    }
}
