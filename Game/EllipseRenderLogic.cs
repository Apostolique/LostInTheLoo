using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameProject
{
    public class EllipseRenderLogic : RenderLogic
    {
        public override void Render(Entity entity)
        {
            var segment = (EllipseSegment)entity.Segments[0];
            G.SB.Begin(view: G.Camera.GetView(segment.Z));
            G.SB.DrawEllipse(segment.Center, segment.Radius1, segment.Radius2, segment.Color1, segment.Color2, segment.Thickness, segment.Rotation);
            if(G.RenderAABB) G.SB.BorderRectangle(entity.AABB.Position, entity.AABB.Size, TWColor.Black, 4f);
            G.SB.End();
            G.R.DrawTo(GameRoot.Target1, GameRoot.Target2);
        }
    }
}
