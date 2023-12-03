using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameProject
{
    public class CircleWithMaskRenderLogic : RenderLogic
    {
        public override void Render(Entity entity)
        {
            var segment = (CircleWithMaskSegment)entity.Segments[0];

            G.SB.Begin(view: G.Camera.GetView(segment.Z));
            G.SB.FillCircle(segment.Center, segment.Radius, segment.Color);
            if(G.RenderAABB) G.SB.BorderRectangle(entity.AABB.Position, entity.AABB.Size, TWColor.White, 4f);
            G.SB.End();

            G.R.DrawInfinite(segment.Texture, GameRoot.Target3, segment.Z, segment.Scale, segment.Offset);
            G.R.ApplyMask(GameRoot.Target3, GameRoot.Target1, GameRoot.Target2);
        }
    }
}
