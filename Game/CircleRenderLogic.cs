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
            G.SB.Begin(view: G.Camera.GetView(segment.Z));
            G.SB.DrawCircle(segment.Center, segment.Radius, segment.Color1, segment.Color2, segment.Thickness);
            G.SB.End();
            G.R.ApplyBokeh(GameRoot.Target1, GameRoot.Target2, segment.BokehBlurRadius, segment.Z);
        }
    }
}