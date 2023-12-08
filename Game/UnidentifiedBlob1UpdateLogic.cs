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
            var blob = (TinyPetEntity)entity;
            Poop(blob);
            DieFromSize(blob);
            FindFood(blob);
            EatFood(blob);
            DieFromStarvation(blob);
            FindRandomTargetPosition(blob);
            SetRandomMovementSpeed(blob);
            SetSinusMovementSpeed(blob);
            SumTotalMovementSpeed(blob);
            Move(blob);

            var state = blob.State;
            if(state == null)
            {
                blob.State = state = Null;
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

            pet.Scale -= 1.0f / pet.Definition.BaseSize;
            pet.Size = pet.Definition.BaseSize * pet.Scale;

            var position = pet.AbsolutePosition;
            var poop = WorldGenerator.CreateStaticFood(position);
            var poopSegment = (MicroSegment)poop.Segments[0];
            poopSegment.Size = 10.0f;
            poopSegment.Color = Color.Brown;
            poop.Type = StaticFoodTypes.Brown;
        }

        private void DieFromSize(TinyPetEntity pet)
        {
            if (pet.IsDying)
            {
                return;
            }

            if (pet.Size <= 0.0f)
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
                pet.TargetPosition = ((MicroSegment)food.Segments[0]).Center.ToVector3XY(food.Z);
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

            var foodSize = ((MicroSegment)food.Segments[0]).Size;
            pet.Scale += foodSize / pet.Definition.BaseSize;
            pet.Size = pet.Definition.BaseSize * pet.Scale;
            pet.DeathFromStarvationTime = G.WorldTime.TotalGameTime.TotalSeconds + foodSize * pet.Definition.FoodBonusMultiplier;

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

            if (pet.DeathFromStarvationTime < G.WorldTime.TotalGameTime.TotalSeconds)
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

        private void SetSinusMovementSpeed(TinyPetEntity pet)
        {
            var speed = pet.SinusMovementSpeedMultiplierSpeed;
            var offset = pet.SinusMovementSpeedMultiplierOffset;
            var scale = pet.SinusMovementSpeedMultiplierScale;
            var time = G.WorldTime.TotalGameTime.TotalSeconds * speed + offset;
            pet.SinusMovementSpeedMultiplier = (float)((Math.Sin(time) + 1.0d) * scale);
        }

        private void SumTotalMovementSpeed(TinyPetEntity pet)
        {
            pet.TotalMovementSpeedMultiplier = pet.RandomMovementSpeedMultiplier + pet.SinusMovementSpeedMultiplier;
        }

        private void Move(TinyPetEntity pet)
        {
            var direction = pet.TargetPosition - pet.AbsolutePosition;

            var courseDiviation = new Vector3(
                (float)Math.Sin(G.WorldTime.TotalGameTime.TotalSeconds * pet.CourseDiviationSpeed),
                (float)Math.Cos(G.WorldTime.TotalGameTime.TotalSeconds * pet.CourseDiviationSpeed),
                0
            );

            var amount = pet.TotalMovementSpeedMultiplier * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            if(amount > pet.DistanceToTargetPositon)
            {
                amount = pet.DistanceToTargetPositon;
            }
            else
            {
                direction += courseDiviation;
            }

            if(direction != Vector3.Zero)
            {
                direction.Normalize();
            }

            if (float.IsNaN(direction.X) || float.IsNaN(direction.Y) || float.IsNaN(direction.Z)) {
                direction = Vector3.Zero;
            }

            var movement = direction * amount;
            pet.LocalPosition += movement;
            pet.DistanceToTargetPositon = Vector3.Distance(pet.AbsolutePosition, pet.TargetPosition);
            pet.TargetRotation = MathF.Atan2(-direction.Y, -direction.X);
            pet.LocalRotationSpin = MathHelper.Lerp(pet.LocalRotationSpin, pet.TargetRotation, (float)G.WorldTime.ElapsedGameTime.TotalSeconds);

            pet.UpdateAbsoluteRecursive();
            pet.Segment.Center = pet.AbsolutePosition.ToVector2XY();
            pet.Segment.Z = pet.AbsolutePosition.Z;
            pet.Segment.Rotation = pet.LocalRotationSpin;
            pet.AABB = MicroSegment.GetAABB(pet.Segment.Center, pet.Size, pet.Segment.Rotation);
            pet.Z = pet.AbsolutePosition.Z;
            G.EntitiesByLocation.Update(pet.Leaf, pet.AABB);
        }

        private void Digest(TinyPetEntity blob, GameTime gameTime)
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

            blob.State = Null;
        }

        private void BeginSplit(TinyPetEntity blob, GameTime gameTime)
        {
            blob.DeathFromStarvationTime += splitTime;
            blob.SplitTimer = G.WorldTime.TotalGameTime.TotalSeconds + splitTime;
            blob.SplitScaleChangePerSecond = blob.Scale * 0.5f / (float)splitTime;
            blob.Scale *= 0.8f;
            blob.Size = blob.Definition.BaseSize * blob.Scale;

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
            twin.Segment = new MicroSegment(segment.Shape, segment.Ramp, segment.Center.X, segment.Center.Y, blob.Size, segment.Rotation, segment.Color, segment.Z);
            twin.Segments = new Segment[] { twin.Segment };
            twin.State = Null;
            twin.TargetFood = null!;
            twin.TargetPosition = blob.TargetPosition;
            twin.UpdateLogic = this;
            twin.Z = blob.Z;
            twin.AABB = blob.AABB;
            twin.UpdateAbsoluteRecursive();

            G.EntitiesByLocation.Update(twin.Leaf, twin.AABB);

            blob.State = Split;
        }

        private void Null(TinyPetEntity blob, GameTime gameTime)
        {
            // do nothing
        }

        private void Split(TinyPetEntity blob, GameTime gameTime)
        {
            var twin = blob.Twin;

            var movement = Vector3.UnitX * (float)G.WorldTime.ElapsedGameTime.TotalSeconds;
            blob.LocalPosition += movement;
            twin.LocalPosition -= movement;

            blob.Scale -= blob.SplitScaleChangePerSecond * (float)G.WorldTime.ElapsedGameTime.TotalSeconds * G.Overpopulation;
            blob.Size = blob.Definition.BaseSize * blob.Scale;
            twin.Segment.Size = blob.Segment.Size;

            blob.UpdateAbsoluteRecursive();
            twin.UpdateAbsoluteRecursive();

            blob.Segment.Center = blob.AbsolutePosition.ToVector2XY();
            blob.Segment.Z = blob.AbsolutePosition.Z;
            twin.Segment.Center = twin.AbsolutePosition.ToVector2XY();
            twin.Segment.Z = twin.AbsolutePosition.Z;

            blob.AABB = MicroSegment.GetAABB(blob.Segment.Center, blob.Size, blob.Segment.Rotation);
            blob.Z = blob.AbsolutePosition.Z;

            twin.AABB = MicroSegment.GetAABB(twin.Segment.Center, twin.Size, twin.Segment.Rotation);
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
                blob.State = Null;
                twin.State = Null;
            }
        }

        private void Dying(TinyPetEntity blob, GameTime gameTime)
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
            var size = blob.Size;
            while(size > 0.0f)
            {
                var position = G.Random.NextVector3(min, max);
                var food = WorldGenerator.CreateStaticFood(position);
                size -= food.AABB.Width;
            }
        }
    }
}
