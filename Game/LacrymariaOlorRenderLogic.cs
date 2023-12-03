using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class LacrymariaOlorRenderLogic : RenderLogic
    {
        public override void Render(Entity entity)
        {
            var olor = (LacrymariaOlorEntity)entity;

            G.SB.Begin(view: G.Camera.GetView(olor.Z));
            G.SB.DrawEllipse(olor.BodyPosition - Vector2.UnitX * 35, 40, 10, TWColor.Blue500, TWColor.Red500, 1);
            G.SB.DrawLine(olor.HeadPosition, olor.BodyPosition, 5, TWColor.Blue500, TWColor.Red500, 1);
            if(G.RenderAABB) G.SB.BorderRectangle(entity.AABB.Position, entity.AABB.Size, TWColor.Black, 4f);
            G.SB.End();
            G.R.ApplyBokeh(GameRoot.Target1, GameRoot.Target2, 1, olor.Z);
        }
    }
}
