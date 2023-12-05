using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameProject
{
    public class LacrymariaOlorUpdateLogic : UpdateLogic
    {
        public override void Update(Entity entity, GameTime gameTime)
        {
            base.Update(entity, gameTime);

            var olor = (LacrymariaOlorEntity)entity;
            olor.NeckLengthSpeed = ((float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds) * 0.5f + 1.0f) * 2.0f;
            olor.HeadRotationCurrent = (float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds * olor.HeadRotationSpeed);
            olor.NeckLengthCurrent = (float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds * olor.NeckLengthSpeed) * 0.5f + 1.0f; // keep it between 0 and 1
            var x = MathF.Cos(olor.HeadRotationCurrent);
            var y = MathF.Sin(olor.HeadRotationCurrent);
            var headOffset = Vector2.Normalize(new Vector2(x, y)) * olor.NeckLengthCurrent * olor.NeckMaxLength;
            olor.HeadPosition = olor.BodyPosition + headOffset;

            RectangleF aabb1 = G.SB.GetEllipseAABB(olor.BodyPosition - Vector2.UnitX * 35, 40, 10, 0f);
            RectangleF aabb2 = G.SB.GetLineAABB(olor.BodyPosition, olor.HeadPosition, 5);
            entity.AABB = RectangleF.Union(aabb1, aabb2);

            G.EntitiesByLocation.Update(olor.Leaf, entity.AABB);
        }
    }
}
