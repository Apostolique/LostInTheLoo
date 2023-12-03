using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameProject
{
    public class CircleRenderLogic : RenderLogic
    {
        public override void Render(Entity entity)
        {
            var segment = (CircleSegment)entity.Segments[0];
            G.SB.DrawCircle(segment.Center, segment.Radius, segment.Color1, segment.Color2, segment.Thickness);
            if(G.RenderAABB) G.SB.BorderRectangle(entity.AABB.Position, entity.AABB.Size, TWColor.Black, 4f);
        }
    }
}
