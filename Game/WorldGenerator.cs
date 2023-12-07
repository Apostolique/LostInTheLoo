using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apos.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject
{
    public static class WorldGenerator
    {
        private const int numberUnidentifiedBlob1ToCreate = 1000;
        private const int numberOfStaticFoodsToCreate = 15000;
        private const int numberOfWasteRecycleBlobToCreate = 1000;
        private const int numberOfWildlyRandomBlobsToCreate = 500;

        private static MicroRenderLogic microRenderLogic = new MicroRenderLogic();
        private static UnidentifiedBlob1UpdateLogic unidentifiedBlob1UpdateLogic = new UnidentifiedBlob1UpdateLogic();
        private static WasteRecycleBlobUpdateLogic wasteRecycleBlobUpdateLogic = new WasteRecycleBlobUpdateLogic();
        private static readonly Random random = new Random();
        private static readonly Color[] staticFoodColors1 = new Color[]
        {
            TWColor.Green600,
            TWColor.Red600,
            TWColor.Blue600,
        };
        private static readonly Color[] staticFoodColors2 = new Color[]
        {
            TWColor.Green900,
            TWColor.Red900,
            TWColor.Blue900,
        };

        public static void Generate()
        {
            var unidentifiedBlob1Definition = new TinyPetDefinition()
            {
                CanPoop = true,
                MinPoopTimeDelay = 5,
                MaxPoopTimeDelay = 10,
                MaxScale = 3,
                BaseSize = 20,
                CanEatFood = true,
                EatsFoods = new int[]
                {
                    StaticFoodTypes.Blue,
                    StaticFoodTypes.Green,
                    StaticFoodTypes.Red,
                },
                FoodBonusMultiplier = 15,
                MinDigestTime = 2,
                MaxDigestTime = 4,
                CanDieFromStarvation = true,
                MinRandomMovementSpeedDelay = 10,
                MaxRandomMovementSpeedDelay = 11,
                MinRandomMovementSpeedMultiplier = 0,
                MaxRandomMovementSpeedMultiplier = 100,
                MinSinusMovementSpeedMultiplierScale = 20,
                MaxSinusMovementSpeedMultiplierScale = 22,
                MinSinusMovementSpeedMultiplierSpeed = 10,
                MaxSinusMovementSpeedMultiplierSpeed = 12,
                Color1 = Color.LimeGreen,
                Color2 = Color.YellowGreen,
            };

            Enumerable.Range(0, numberOfStaticFoodsToCreate).ForEach(_ => CreateStaticFood());
            Enumerable.Range(0, numberUnidentifiedBlob1ToCreate).ForEach(index => CreateRandomUnidentifiedBlob1(index, unidentifiedBlob1Definition));
            Enumerable.Range(0, numberOfWasteRecycleBlobToCreate).ForEach(index => CreateWasteRecycleBlob(index));
            Enumerable.Range(0, numberOfWildlyRandomBlobsToCreate).ForEach(index => CreateWildlyRandomBlob(index));
        }

        public static TinyPetEntity CreateRandomUnidentifiedBlob1(int index, TinyPetDefinition definition)
        {
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var targetPosition = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var radius1 = definition.BaseSize;
            var entity = new TinyPetEntity()
            {
                Definition = definition,
                LocalPosition = position,
                RenderLogic = microRenderLogic,
                UpdateLogic = unidentifiedBlob1UpdateLogic,
                CourseDiviationSpeed = random.NextSingle() * 50 + 1,
                TargetPosition = targetPosition,
                Segment = new MicroSegment(Batch.MicroShapes.Bean, Batch.MicroRamps.Ramp02, position.X, position.Y, radius1, 0f, definition.Color1, position.Z),
                RandomMovementSpeedMultiplier = random.NextSingle(definition.MinRandomMovementSpeedMultiplier, definition.MaxRandomMovementSpeedMultiplier),
                NextRandomMovementSpeedMultiplierChange = random.NextDouble(definition.MinRandomMovementSpeedDelay, definition.MaxRandomMovementSpeedDelay),
                Z = position.Z,
                NextFoodScan = index * 0.2f,
                DeathFromStarvationTime = random.NextDouble() * 240 + 1,
                AABB = MicroSegment.GetAABB(new Vector2(position.X, position.Y), radius1),
                SinusMovementSpeedMultiplierSpeed = random.NextDouble(definition.MinSinusMovementSpeedMultiplierSpeed, definition.MaxSinusMovementSpeedMultiplierSpeed),
                SinusMovementSpeedMultiplierOffset = random.NextDouble(-Math.PI, Math.PI),
                SinusMovementSpeedMultiplierScale = random.NextDouble(definition.MinSinusMovementSpeedMultiplierScale, definition.MaxSinusMovementSpeedMultiplierScale),
            };
            entity.Segments = new Segment[] { entity.Segment };
            entity.UpdateAbsoluteRecursive();
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            return entity;
        }

        public static TinyPetEntity CreateWildlyRandomBlob(int index)
        {
            var twColorType = typeof(TWColor);
            var twColorFields = twColorType
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(field => field.FieldType == typeof(Color))
                .ToArray()
                ;
            var foods = new int []
            {
                StaticFoodTypes.Blue,
                StaticFoodTypes.Brown,
                StaticFoodTypes.Green,
                StaticFoodTypes.Red,
            };
            var definition = new TinyPetDefinition()
            {
                BaseSize = random.NextSingle(2.0f, 30.0f),
                CanDieFromStarvation = random.NextDouble() > 0.5d,
                CanEatFood = random.NextDouble() > 0.5d,
                CanPoop = random.NextDouble() > 0.5d,
                Color1 = (Color)twColorFields[random.Next(twColorFields.Length)].GetValue(null)!,
                Color2 = (Color)twColorFields[random.Next(twColorFields.Length)].GetValue(null)!,
                DeathAnimationTime = random.NextDouble(1, 10),
                EatsFoods = foods.OrderBy(_ => random.NextDouble()).Take(random.Next(foods.Length+1)).ToArray(),
                FoodBonusMultiplier = random.NextSingle(1.0f, 100.0f),
                MinDigestTime = random.NextDouble(1, 10),
                MaxDigestTime = random.NextDouble(10, 20),
                MinPoopTimeDelay = random.NextDouble(5, 10),
                MaxPoopTimeDelay = random.NextDouble(10, 20),
                MinRandomMovementSpeedDelay = random.NextDouble(1, 10),
                MaxRandomMovementSpeedDelay = random.NextDouble(10, 20),
                MinRandomMovementSpeedMultiplier = random.NextSingle(0.5f, 15),
                MaxRandomMovementSpeedMultiplier = random.NextSingle(15, 40),
                MaxScale = 3,
                MinSinusMovementSpeedMultiplierScale = random.NextDouble(1, 10),
                MaxSinusMovementSpeedMultiplierScale = random.NextDouble(10, 20),
                MinSinusMovementSpeedMultiplierSpeed = random.NextDouble(10, 20),
                MaxSinusMovementSpeedMultiplierSpeed = random.NextDouble(20, 40),
            };
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var targetPosition = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var radius1 = definition.BaseSize;
            var entity = new TinyPetEntity()
            {
                Definition = definition,
                LocalPosition = position,
                RenderLogic = microRenderLogic,
                UpdateLogic = unidentifiedBlob1UpdateLogic,
                CourseDiviationSpeed = random.NextSingle() * 50 + 1,
                TargetPosition = targetPosition,
                Segment = new MicroSegment(Batch.MicroShapes.Drill, Batch.MicroRamps.Ramp02, position.X, position.Y, radius1, 0f, definition.Color1, position.Z),
                RandomMovementSpeedMultiplier = random.NextSingle(definition.MinRandomMovementSpeedMultiplier, definition.MaxRandomMovementSpeedMultiplier),
                NextRandomMovementSpeedMultiplierChange = random.NextDouble(definition.MinRandomMovementSpeedDelay, definition.MaxRandomMovementSpeedDelay),
                Z = position.Z,
                NextFoodScan = index * 0.2f,
                DeathFromStarvationTime = random.NextDouble() * 240 + 1,
                AABB = MicroSegment.GetAABB(new Vector2(position.X, position.Y), radius1),
                SinusMovementSpeedMultiplierSpeed = random.NextDouble(definition.MinSinusMovementSpeedMultiplierSpeed, definition.MaxSinusMovementSpeedMultiplierSpeed),
                SinusMovementSpeedMultiplierOffset = random.NextDouble(-Math.PI, Math.PI),
                SinusMovementSpeedMultiplierScale = random.NextDouble(definition.MinSinusMovementSpeedMultiplierScale, definition.MaxSinusMovementSpeedMultiplierScale),
            };
            entity.Segments = new Segment[] { entity.Segment };
            entity.UpdateAbsoluteRecursive();
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            return entity;
        }

        public static WasteRecycleBlobEntity CreateWasteRecycleBlob(int index)
        {
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var size = random.NextSingle(WasteRecycleBlobEntity.MinRadius, WasteRecycleBlobEntity.MaxRadius);
            var entity = new WasteRecycleBlobEntity()
            {
                OriginalSize = size,
                Scale = 1,
                LocalPosition = position,
                RenderLogic = microRenderLogic,
                UpdateLogic = wasteRecycleBlobUpdateLogic,
                TargetPosition = position,
                Segment = new MicroSegment(Batch.MicroShapes.Skewer, Batch.MicroRamps.Ramp02, position.X, position.Y, size, 0f, Color.SaddleBrown, position.Z),
                MovementSpeedMultiplier = WasteRecycleBlobEntity.RoamSpeedMultiplier,
                Z = position.Z,
                NextFoodScan = index * 0.005f,
                AABB = MicroSegment.GetAABB(new Vector2(position.X, position.Y), size),
            };
            entity.Segments = new Segment[] { entity.Segment };
            entity.UpdateAbsoluteRecursive();
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            return entity;
        }

        public static void CreateStaticFood()
        {
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            CreateStaticFood(position);
        }

        public static StaticFoodEntity CreateStaticFood(Vector3 position)
        {
            var category = random.Next(0, StaticFoodCategories.MaxIndex + 1);
            var type = random.Next(0, StaticFoodTypes.MaxIndex + 1);
            var color1 = staticFoodColors1[type];
            var color2 = staticFoodColors2[type];
            var radius = random.NextSingle() * 4 + 1;
            var entity = new StaticFoodEntity()
            {
                Category = category,
                Type = type,
                Segments = new Segment[]
                {
                    new MicroSegment(Batch.MicroShapes.Skewer, Batch.MicroRamps.Ramp02, position.X, position.Y, radius, 0.1f, color1, position.Z),
                },
                UpdateLogic = UpdateLogic.NullLogic,
                RenderLogic = microRenderLogic,
                AABB = MicroSegment.GetAABB(new Vector2(position.X, position.Y), radius),
            };
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            entity.Leaf2 = G.StaticFoodEntitiesByLocation.Add(entity.AABB, entity);
            return entity;
        }
    }
}
