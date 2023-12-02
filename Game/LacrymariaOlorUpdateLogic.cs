using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class LacrymariaOlorUpdateLogic : UpdateLogic
    {
        public override void Update(Entity entity, GameTime gameTime)
        {
            base.Update(entity, gameTime);

            var olor = (LacrymariaOlorEntity)entity;
            olor.NeckLengthSpeed = ((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 0.5f + 1.0f) * 2.0f;
            olor.HeadRotationCurrent = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * olor.HeadRotationSpeed);
            olor.NeckLengthCurrent = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * olor.NeckLengthSpeed) * 0.5f + 1.0f; // keep it between 0 and 1
            var x = MathF.Cos(olor.HeadRotationCurrent);
            var y = MathF.Sin(olor.HeadRotationCurrent);
            var headOffset = Vector2.Normalize(new Vector2(x, y)) * olor.NeckLengthCurrent * olor.NeckMaxLength;
            olor.HeadPosition = olor.BodyPosition + headOffset;
        }
    }
}