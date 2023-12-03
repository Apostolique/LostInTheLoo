using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1UpdateLogic : UpdateLogic
    {
        public override void Update(Entity entity, GameTime gameTime)
        {
            var blob = (UnidentifiedBlob1Entity)entity;

            var distance = Vector3.Distance(blob.AbsolutePosition, blob.TargetPosition);
            if(distance < 1.0f)
            {
                blob.TargetPosition = G.Random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            }

            var direction = blob.TargetPosition - blob.AbsolutePosition;
            if(direction != Vector3.Zero)
            {
                direction.Normalize();
            }

            var courseDiviation = new Vector3(
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * blob.CourseDiviationSpeed),
                (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds * blob.CourseDiviationSpeed),
                0
            );
            if(courseDiviation != Vector3.Zero)
            {
                courseDiviation.Normalize();
            }

            if(blob.NextMovementSpeedMultiplierChange <= gameTime.TotalGameTime.TotalSeconds)
            {
                blob.NextMovementSpeedMultiplierChange = G.Random.NextDouble() * 10 + 1;
                blob.MovementSpeedMultiplier = G.Random.NextSingle() * 100;
            }

            var movement = Vector3.Normalize(direction + courseDiviation) * blob.MovementSpeedMultiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            blob.LocalPosition += movement;
            blob.LocalRotationSpin = MathF.Atan2(movement.Y, -movement.X);

            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            blob.Segment.Roation = (float)gameTime.TotalGameTime.TotalSeconds; // I don't think rotation works
            blob.AABB.X = blob.AbsolutePosition.X;
            blob.AABB.Y = blob.AbsolutePosition.Y;
            blob.AABB.Width = 40f;
            blob.AABB.Height = 20f;
            blob.Z = blob.AbsolutePosition.Z;
            entity.AABB = blob.AABB;
            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
        }
    }
}
