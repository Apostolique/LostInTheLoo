using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class WasteRecycleBlobUpdateLogic : UpdateLogic
    {
        private const double digestTime = 2.0d;

        public override void Update(Entity entity, GameTime gameTime)
        {
            var blob = (WasteRecycleBlobEntity)entity;
            var state = blob.State;
            if(state == null)
            {
                blob.State = state = Idle;
            }

            state(blob, gameTime);
        }

        private void Idle(WasteRecycleBlobEntity blob, GameTime gameTime)
        {
            var food = blob.TargetFood;
            if(food != null && food.HasBeenEaten)
            {
                blob.TargetFood = null!;
            }

            if(blob.NextFoodScan <= G.WorldTime.TotalGameTime.TotalSeconds)
            {
                blob.NextFoodScan += WasteRecycleBlobEntity.FoodScanDelay;
                if(food == null)
                {
                    var bounds = blob.AABB;
                    bounds.Inflate(100, 100);
                    food = G.StaticFoodEntitiesByLocation
                        .Query(bounds)
                        .Where(entity => entity != null)
                        .OfType<StaticFoodEntity>()
                        .Where(entity => !entity.HasBeenEaten)
                        .Where(entity => entity.Type == StaticFoodTypes.Brown)
                        .Take(3)
                        .OrderBy(_ => G.Random.NextDouble())
                        .FirstOrDefault()
                        ;
                    if(food != null)
                    {
                        blob.TargetFood = food;
                        blob.TargetPosition = ((MicroSegment)food.Segments[0]).Center.ToVector3XY(food.Z);
                        blob.MovementSpeedMultiplier = WasteRecycleBlobEntity.SeekSpeedMultiplier;
                    }
                }
            }

            var distance = Vector3.Distance(blob.AbsolutePosition, blob.TargetPosition);
            if(distance < 0.2f)
            {
                food = blob.TargetFood;
                if(food != null)
                {
                    food.HasBeenEaten = true;
                    blob.TargetFood = null!;
                    food.Leaf = G.EntitiesByLocation.Remove(food.Leaf);
                    food.Leaf2 = G.StaticFoodEntitiesByLocation.Remove(food.Leaf2);
                    var foodSize = ((MicroSegment)food.Segments[0]).Size;
                    blob.Scale += foodSize;
                    blob.Segment.Size = blob.OriginalSize * blob.Scale;
                    blob.DeathFromStarvationTime = G.WorldTime.TotalGameTime.TotalSeconds + foodSize * 15f;
                    blob.NextFoodScan += digestTime + 2.0d; // delay to search for food again
                    blob.State = Digest;
                    blob.DigestTimer = G.WorldTime.TotalGameTime.TotalSeconds + digestTime;
                }

                var newDistance = G.Random.NextSingle(50, 150);
                blob.TargetPosition = G.Random.NextVector3FromRadius(blob.AbsolutePosition, newDistance);
                blob.MovementSpeedMultiplier = WasteRecycleBlobEntity.RoamSpeedMultiplier;
                return;
            }

            var direction = blob.TargetPosition - blob.AbsolutePosition;

            var amount = blob.MovementSpeedMultiplier * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            if(amount > distance)
            {
                amount = distance;
            }

            if(direction != Vector3.Zero)
            {
                direction.Normalize();
            }

            var movement = direction * amount;
            blob.LocalPosition += movement;
            blob.TargetRotation = MathF.Atan2(direction.Y, -direction.X);
            blob.LocalRotationSpin = MathHelper.Lerp(blob.LocalRotationSpin, blob.TargetRotation, (float)G.WorldTime.ElapsedGameTime.TotalSeconds);

            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            blob.Segment.Rotation = blob.LocalRotationSpin;
            blob.AABB = MicroSegment.GetAABB(blob.Segment.Center, blob.Segment.Size, blob.Segment.Rotation);
            blob.Z = blob.AbsolutePosition.Z;
            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
        }

        private void Digest(WasteRecycleBlobEntity blob, GameTime gameTime)
        {
            if(blob.DigestTimer > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            if(blob.Scale > WasteRecycleBlobEntity.MaxRadius)
            {
                const float foodSize = 4;
                blob.Scale *= 0.5f;
                blob.Segment.Size = blob.OriginalSize * blob.Scale;

                var position = blob.AbsolutePosition;
                var food = WorldGenerator.CreateStaticFood(position);
                var poopSegment = (MicroSegment)food.Segments[0];
                poopSegment.Size = foodSize;
            }

            blob.State = Idle;
        }
    }
}
