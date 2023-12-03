using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1UpdateLogic : UpdateLogic
    {
        private const double digestTime = 2.0d;
        private const float maxRadius = 40.0f;
        private const double splitTime = 11.0d;

        public override void Update(Entity entity, GameTime gameTime)
        {
            var blob = (UnidentifiedBlob1Entity)entity;
            var state = blob.State;
            if(state == null)
            {
                blob.State = state = Idle;
            }

            state(blob, gameTime);
        }

        private void Idle(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            var food = blob.TargetFood;
            if(food != null && food.HasBeenEaten)
            {
                blob.TargetFood = null!;
            }

            if(blob.NextFoodScan <= gameTime.TotalGameTime.TotalSeconds)
            {
                blob.NextFoodScan += 2.0d;
                if(food == null)
                {
                    var bounds = blob.AABB;
                    bounds.Inflate(100, 100);
                    food = G.StaticFoodEntitiesByLocation
                        .Query(bounds)
                        .Where(entity => entity != null)
                        .OfType<StaticFoodEntity>()
                        .Where(entity => !entity.HasBeenEaten)
                        .Take(3)
                        .OrderBy(_ => G.Random.NextDouble())
                        .FirstOrDefault()
                        ;
                    if(food != null)
                    {
                        blob.TargetFood = food;
                        blob.TargetPosition = ((CircleSegment)food.Segments[0]).Center.ToVector3XY(food.Z);
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
                    food.Leaf1 = G.EntitiesByLocation.Remove(food.Leaf1);
                    food.Leaf2 = G.StaticFoodEntitiesByLocation.Remove(food.Leaf2);
                    var foodRadius = ((CircleSegment)food.Segments[0]).Radius;
                    blob.Segment.Radius1 += foodRadius * (blob.Segment.Radius2 / blob.Segment.Radius1);
                    blob.Segment.Radius2 += foodRadius * (1.0f - blob.Segment.Radius2 / blob.Segment.Radius1);
                    blob.DeathFromStarvationTime = gameTime.TotalGameTime.TotalSeconds + 240.0d;
                    blob.NextFoodScan += digestTime + 2.0d; // delay to search for food again
                    blob.State = Digest;
                    blob.DigestTimer = gameTime.TotalGameTime.TotalSeconds + digestTime;
                }

                blob.TargetPosition = G.Random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
                return;
            }

            if(blob.DeathFromStarvationTime <= gameTime.TotalGameTime.TotalSeconds)
            {
                blob.DeathFromStarvationTime = gameTime.TotalGameTime.TotalSeconds + 2.0d;
                blob.State = Dying; // goodbye little blob 😥
                return;
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

            var amount = blob.MovementSpeedMultiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(amount > distance)
            {
                amount = distance;
            }
            else
            {
                direction += courseDiviation;
                direction.Normalize();
            }
            var movement = direction * amount;
            blob.LocalPosition += movement;
            blob.LocalRotationSpin = MathF.Atan2(movement.Y, -movement.X);

            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            blob.Segment.Roation = (float)gameTime.TotalGameTime.TotalSeconds; // I don't think rotation works
            blob.AABB.X = blob.AbsolutePosition.X - blob.Segment.Radius1 * 0.5f;
            blob.AABB.Y = blob.AbsolutePosition.Y - blob.Segment.Radius2 * 0.5f;
            blob.AABB.Width = blob.Segment.Radius1;
            blob.AABB.Height = blob.Segment.Radius2;
            blob.Z = blob.AbsolutePosition.Z;
            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
        }

        private void Digest(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            if(blob.DigestTimer > gameTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            if(blob.Segment.Radius1 > maxRadius)
            {
                blob.State = BeginSplit;
                return;
            }

            blob.State = Idle;
        }

        private void BeginSplit(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            blob.DeathFromStarvationTime += splitTime;
            blob.SplitTimer = gameTime.TotalGameTime.TotalSeconds + splitTime;
            blob.SplitRadius1ChangePerSecond = blob.Segment.Radius1 * 0.5f / (float)splitTime;
            blob.SplitRadius2ChangePerSecond = blob.Segment.Radius2 * 0.5f / (float)splitTime;

            var twin = blob.Twin = WorldGenerator.CreateRandomUnidentifiedBlob1(0);
            // twin.AbsolutePosition = blob.AbsolutePosition;
            // twin.AbsoluteRotation = blob.AbsoluteRotation;
            // twin.AbsoluteScale = blob.AbsoluteScale;
            // twin.AbsoluteWorld = blob.AbsoluteWorld;
            twin.CourseDiviationSpeed = blob.CourseDiviationSpeed;
            twin.DeathFromStarvationTime = blob.DeathFromStarvationTime;
            twin.LocalPosition = blob.LocalPosition;
            twin.LocalRotationOrbit = blob.LocalRotationOrbit;
            twin.LocalRotationSpin = blob.LocalRotationSpin;
            twin.LocalScale = blob.LocalScale;
            twin.LocalWorld = blob.LocalWorld;
            twin.MovementSpeedMultiplier = blob.MovementSpeedMultiplier;
            twin.NextFoodScan = blob.NextFoodScan;
            twin.NextMovementSpeedMultiplierChange = blob.NextMovementSpeedMultiplierChange;
            twin.RenderLogic = blob.RenderLogic;
            var segment = blob.Segment;
            twin.Segment = new EllipseSegment(segment.Center.X, segment.Center.Y, segment.Radius1, segment.Radius2, segment.Color1, segment.Color2, segment.Thickness, segment.Rotation, segment.Z);
            twin.Segments = new Segment[] { twin.Segment };
            twin.State = Null;
            twin.TargetFood = null!;
            twin.TargetPosition = blob.TargetPosition;
            twin.UpdateLogic = UpdateLogic.NullLogic;
            twin.Z = blob.Z;
            twin.AABB = blob.AABB;
            twin.UpdateAbsoluteRecursive();

            G.EntitiesByLocation.Update(twin.Leaf, twin.AABB);

            blob.State = Split;
        }

        private void Null(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            // do nothing
        }

        private void Split(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            var twin = blob.Twin;

            var movement = Vector3.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            blob.LocalPosition += movement;
            twin.LocalPosition -= movement;

            blob.Segment.Radius1 -= blob.SplitRadius1ChangePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            blob.Segment.Radius2 -= blob.SplitRadius2ChangePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            twin.Segment.Radius1 = blob.Segment.Radius1;
            twin.Segment.Radius2 = blob.Segment.Radius2;

            blob.UpdateAbsoluteRecursive();
            twin.UpdateAbsoluteRecursive();

            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            twin.Segment.Center = twin.AbsolutePosition.ToVector2XY();
            twin.Segment.Z = twin.AbsolutePosition.Z;

            blob.AABB.X = blob.AbsolutePosition.X - blob.Segment.Radius1 * 0.5f;
            blob.AABB.Y = blob.AbsolutePosition.Y - blob.Segment.Radius2 * 0.5f;
            blob.AABB.Width = blob.Segment.Radius1;
            blob.AABB.Height = blob.Segment.Radius2;
            blob.Z = blob.AbsolutePosition.Z;

            twin.AABB.X = twin.AbsolutePosition.X - twin.Segment.Radius1 * 0.5f;
            twin.AABB.Y = twin.AbsolutePosition.Y - twin.Segment.Radius2 * 0.5f;
            twin.AABB.Width = twin.Segment.Radius1;
            twin.AABB.Height = twin.Segment.Radius2;
            twin.Z = twin.AbsolutePosition.Z;

            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
            G.EntitiesByLocation.Update(twin.Leaf, twin.AABB);

            if(blob.SplitTimer <= gameTime.TotalGameTime.TotalSeconds)
            {
                blob.Twin = null!;
                twin.Twin = null!;
                blob.SplitRadius1ChangePerSecond = 0.0f;
                twin.SplitRadius1ChangePerSecond = 0.0f;
                blob.SplitRadius2ChangePerSecond = 0.0f;
                twin.SplitRadius2ChangePerSecond = 0.0f;
                blob.SplitTimer = 0.0d;
                twin.SplitTimer = 0.0d;

                twin.UpdateLogic = blob.UpdateLogic;
                blob.State = Idle;
                twin.State = Idle;
            }
        }

        private void Dying(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            const float jitterSpeed = 100;
            var jitter = Vector3.UnitX * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * jitterSpeed);
            blob.LocalPosition += jitter;
            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();

            if(blob.DeathFromStarvationTime <= gameTime.TotalGameTime.TotalSeconds)
            {
                G.EntitiesByLocation.Remove(blob.Leaf);
                var min = blob.AABB.TopLeft.ToVector3XY(blob.Z);
                var max = blob.AABB.BottomRight.ToVector3XY(blob.Z);
                while(blob.Segment.Radius1 > 0.0f)
                {
                    var position = G.Random.NextVector3(min, max);
                    var food = WorldGenerator.CreateStaticFood(position);
                    blob.Segment.Radius1 -= food.AABB.Width;
                }
            }
        }
    }
}
