using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1UpdateLogic : UpdateLogic
    {
        private const double splitTime = 11.0d;

        public override void Update(Entity entity, GameTime gameTime)
        {
            var blob = (UnidentifiedBlob1Entity)entity;
            Poop(blob);
            DieFromSize(blob);
            FindFood(blob);
            EatFood(blob);
            DieFromStarvation(blob);
            FindRandomTargetPosition(blob);
            SetRandomMovementSpeed(blob);
            SumTotalMovementSpeed(blob);

            var state = blob.State;
            if(state == null)
            {
                blob.State = state = Idle;
            }

            state(blob, gameTime);
        }

        private void Poop(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }

            if (!pet.Definition.CanPoop)
            {
                return;
            }

            if (pet.NextPoopTime > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            pet.NextPoopTime = G.WorldTime.TotalGameTime.TotalSeconds + G.Random.NextDouble(pet.Definition.MinPoopTimeDelay, pet.Definition.MaxPoopTimeDelay);

            if (pet.Scale <= 1.0f)
            {
                return;
            }

            pet.Scale -= 1.0f / pet.Definition.BaseRadius1;
            pet.Radius1 = pet.Definition.BaseRadius1 * pet.Scale;
            pet.Radius2 = pet.Definition.BaseRadius2 * pet.Scale;

            var position = pet.AbsolutePosition;
            var poop = WorldGenerator.CreateStaticFood(position);
            var poopSegment = (CircleSegment)poop.Segments[0];
            poopSegment.Radius = 1.0f;
            poopSegment.Color1 = Color.Brown;
            poopSegment.Color2 = Color.Brown;
            poop.Type = StaticFoodTypes.Brown;
        }

        private void DieFromSize(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }
            
            if (pet.Radius1 <= 0.0f || pet.Radius2 <= 0.0f)
            {
                pet.DeathTime = G.WorldTime.TotalGameTime.TotalSeconds + pet.Definition.DeathAnimationTime;
                pet.IsDying = true;
                pet.State = Dying;
                return;
            }
        }

        private void FindFood(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }
            
            if (!pet.Definition.CanEatFood)
            {
                return;
            }

            if (pet.TargetFood != null)
            {
                if (!pet.TargetFood.HasBeenEaten)
                {
                    return;
                }
                pet.TargetFood = null!;
            }

            if (pet.NextFoodScan > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }
            pet.NextFoodScan += 2.0d;

            var bounds = pet.AABB;
            bounds.Inflate(100, 100);
            var food = G.StaticFoodEntitiesByLocation
                .Query(bounds)
                .Where(entity => entity != null)
                .OfType<StaticFoodEntity>()
                .Where(entity => !entity.HasBeenEaten)
                .Where(entity => pet.Definition.EatsFoods.Contains(entity.Type))
                .Take(3)
                .OrderBy(_ => G.Random.NextDouble())
                .FirstOrDefault()
                ;
            if(food != null)
            {
                pet.TargetFood = food;
                pet.TargetPosition = ((CircleSegment)food.Segments[0]).Center.ToVector3XY(food.Z);
            }
        }

        private void EatFood(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }
            
            if (!pet.Definition.CanEatFood)
            {
                return;
            }

            if (pet.TargetFood == null)
            {
                return;
            }

            if (pet.TargetFood.HasBeenEaten)
            {
                pet.TargetFood = null!;
                return;
            }

            if (pet.DistanceToTargetPositon >= 0.2f)
            {
                return;
            }

            var food = pet.TargetFood;
            food.HasBeenEaten = true;
            food.Leaf = G.EntitiesByLocation.Remove(food.Leaf);
            food.Leaf2 = G.StaticFoodEntitiesByLocation.Remove(food.Leaf2);

            var foodRadius = ((CircleSegment)food.Segments[0]).Radius;
            pet.Scale += foodRadius / pet.Definition.BaseRadius1;
            pet.Radius1 = pet.Definition.BaseRadius1 * pet.Scale;
            pet.Radius2 = pet.Definition.BaseRadius2 * pet.Scale;
            pet.DeathFromStarvationTime = G.WorldTime.TotalGameTime.TotalSeconds + foodRadius * pet.Definition.FoodBonusMultiplier;

            pet.TargetFood = null!;
            pet.TargetPosition = G.Random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);

            var digestTime = G.Random.NextDouble(pet.Definition.MinDigestTime, pet.Definition.MaxDigestTime);
            pet.NextFoodScan += digestTime + 2.0d; // delay to search for food again
            pet.DigestTimer = G.WorldTime.TotalGameTime.TotalSeconds + digestTime;
            pet.State = Digest;
        }

        private void DieFromStarvation(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }
            
            if (!pet.Definition.CanDieFromStarvation)
            {
                return;
            }

            if (pet.DeathFromStarvationTime > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            // goodbye little blob 😥
            pet.DeathTime = G.WorldTime.TotalGameTime.TotalSeconds + pet.Definition.DeathAnimationTime;
            pet.IsDying = true;
            pet.State = Dying;
        }

        private void FindRandomTargetPosition(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }

            if (pet.DistanceToTargetPositon > 0.2f)
            {
                return;
            }

            if (pet.TargetFood != null)
            {
                return;
            }

            pet.TargetPosition = G.Random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
        }

        private void SetRandomMovementSpeed(TinyPetEntity pet)
        {
            if(pet.NextRandomMovementSpeedMultiplierChange <= G.WorldTime.TotalGameTime.TotalSeconds)
            {
                pet.NextRandomMovementSpeedMultiplierChange = G.Random.NextDouble(pet.Definition.MinRandomMovementSpeedDelay, pet.Definition.MaxRandomMovementSpeedDelay);
                pet.RandomMovementSpeedMultiplier = G.Random.NextSingle(pet.Definition.MinRandomMovementSpeedMultiplier, pet.Definition.MaxRandomMovementSpeedMultiplier);
            }
        }

        private void SumTotalMovementSpeed(TinyPetEntity pet)
        {
            pet.TotalMovementSpeedMultiplier = pet.RandomMovementSpeedMultiplier;
        }

        private void Idle(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            var direction = blob.TargetPosition - blob.AbsolutePosition;

            var courseDiviation = new Vector3(
                (float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds * blob.CourseDiviationSpeed),
                (float)Math.Cos(G.WorldTime.TotalGameTime.TotalSeconds * blob.CourseDiviationSpeed),
                0
            );

            var amount = blob.TotalMovementSpeedMultiplier * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            if(amount > blob.DistanceToTargetPositon)
            {
                amount = blob.DistanceToTargetPositon;
            }
            else
            {
                direction += courseDiviation;
            }

            if(direction != Vector3.Zero)
            {
                direction.Normalize();
            }

            var movement = direction * amount;
            blob.LocalPosition += movement;
            blob.DistanceToTargetPositon = Vector3.Distance(blob.AbsolutePosition, blob.TargetPosition);
            blob.TargetRotation = MathF.Atan2(-direction.Y, -direction.X);
            blob.LocalRotationSpin = MathHelper.Lerp(blob.LocalRotationSpin, blob.TargetRotation, (float)G.WorldTime.ElapsedGameTime.TotalSeconds);

            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            blob.Segment.Rotation = blob.LocalRotationSpin;
            blob.AABB = G.SB.GetEllipseAABB(blob.Segment.Center, blob.Radius1, blob.Radius2, blob.Segment.Rotation);
            blob.Z = blob.AbsolutePosition.Z;
            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
        }

        private void Digest(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            if(blob.DigestTimer > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            if(blob.Scale > blob.Definition.MaxScale)
            {
                blob.State = BeginSplit;
                return;
            }

            blob.State = Idle;
        }

        private void BeginSplit(UnidentifiedBlob1Entity blob, GameTime gameTime)
        {
            blob.DeathFromStarvationTime += splitTime;
            blob.SplitTimer = G.WorldTime.TotalGameTime.TotalSeconds + splitTime;
            blob.SplitScaleChangePerSecond = blob.Scale * 0.5f / (float)splitTime;

            var twin = blob.Twin = WorldGenerator.CreateRandomUnidentifiedBlob1(0, blob.Definition);
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
            twin.RandomMovementSpeedMultiplier = blob.RandomMovementSpeedMultiplier;
            twin.NextFoodScan = blob.NextFoodScan;
            twin.NextRandomMovementSpeedMultiplierChange = blob.NextRandomMovementSpeedMultiplierChange;
            twin.RenderLogic = blob.RenderLogic;
            var segment = blob.Segment;
            twin.Segment = new EllipseSegment(segment.Center.X, segment.Center.Y, blob.Radius1, blob.Radius2, segment.Color1, segment.Color2, segment.Thickness, segment.Rotation, segment.Z);
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

            var movement = Vector3.UnitX * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            blob.LocalPosition += movement;
            twin.LocalPosition -= movement;

            blob.Scale -= blob.SplitScaleChangePerSecond * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            blob.Radius1 = blob.Definition.BaseRadius1 * blob.Scale;
            blob.Radius2 = blob.Definition.BaseRadius2 * blob.Scale;
            twin.Segment.Radius1 = blob.Segment.Radius1;
            twin.Segment.Radius2 = blob.Segment.Radius2;

            blob.UpdateAbsoluteRecursive();
            twin.UpdateAbsoluteRecursive();

            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            twin.Segment.Center = twin.AbsolutePosition.ToVector2XY();
            twin.Segment.Z = twin.AbsolutePosition.Z;

            blob.AABB = G.SB.GetEllipseAABB(blob.Segment.Center, blob.Radius1, blob.Radius2, blob.Segment.Rotation);
            blob.Z = blob.AbsolutePosition.Z;

            twin.AABB = G.SB.GetEllipseAABB(twin.Segment.Center, twin.Radius1, twin.Radius2, twin.Segment.Rotation);
            twin.Z = twin.AbsolutePosition.Z;

            G.EntitiesByLocation.Update(blob.Leaf, blob.AABB);
            G.EntitiesByLocation.Update(twin.Leaf, twin.AABB);

            if(blob.SplitTimer <= G.WorldTime.TotalGameTime.TotalSeconds)
            {
                blob.Twin = null!;
                twin.Twin = null!;
                blob.SplitScaleChangePerSecond = 0.0f;
                twin.SplitScaleChangePerSecond = 0.0f;
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
            var jitter = Vector3.UnitX * (float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds * jitterSpeed);
            blob.LocalPosition += jitter;
            blob.UpdateAbsoluteRecursive();
            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();

            if (blob.DeathTime > G.WorldTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            blob.Leaf = G.EntitiesByLocation.Remove(blob.Leaf);
            var min = blob.AABB.TopLeft.ToVector3XY(blob.Z);
            var max = blob.AABB.BottomRight.ToVector3XY(blob.Z);
            var radius = blob.Radius1 + blob.Radius2 * 2;
            while(radius > 0.0f)
            {
                var position = G.Random.NextVector3(min, max);
                var food = WorldGenerator.CreateStaticFood(position);
                radius -= food.AABB.Width;
            }
        }
    }
}
