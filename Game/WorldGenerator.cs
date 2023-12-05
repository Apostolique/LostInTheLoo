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

        private static CircleRenderLogic circleRenderLogic = new CircleRenderLogic();
        private static EllipseRenderLogic ellipseRenderLogic = new EllipseRenderLogic();
        private static LacrymariaOlorRenderLogic lacrymariaOlorRenderLogic = new LacrymariaOlorRenderLogic();
        private static LacrymariaOlorUpdateLogic lacrymariaOlorUpdateLogic = new LacrymariaOlorUpdateLogic();
        private static UnidentifiedBlob1UpdateLogic unidentifiedBlob1UpdateLogic = new UnidentifiedBlob1UpdateLogic();
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
            Enumerable.Range(0, numberOfStaticFoodsToCreate).ForEach(_ => CreateStaticFood());
            Enumerable.Range(0, numberUnidentifiedBlob1ToCreate).ForEach(index => CreateRandomUnidentifiedBlob1(index));
            CreateLacrymariaOlorEntity(50, 50);
            CreateEllipseEntity(0, 0, 50, 20, TWColor.Pink300, TWColor.Gray800, 1, 0, 0);
        }

        private static void CreateCircleEntity(float centerX, float centerY, float radius, Color color1, Color color2, float thickness, float z)
        {
            var entity = new Entity()
            {
                RenderLogic = circleRenderLogic,
                UpdateLogic = UpdateLogic.NullLogic,
                Segments = new Segment[]
                {
                    new CircleSegment(centerX, centerY, radius, color1, color2, thickness, z),
                },
                AABB = new RectangleF(centerX - radius * 0.5f, centerY - radius * 0.5f, radius, radius),
                Z = z
            };
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
        }

        private static void CreateEllipseEntity(float centerX, float centerY, float radius1, float radius2, Color color1, Color color2, float thickness, float rotation, float z)
        {
            var entity = new Entity()
            {
                RenderLogic = ellipseRenderLogic,
                UpdateLogic = UpdateLogic.NullLogic,
                Segments = new Segment[]
                {
                    new EllipseSegment(centerX, centerY, radius1, radius2, color1, color2, thickness, rotation, z),
                },
                AABB = G.SB.GetEllipseAABB(new Vector2(centerX, centerY), radius1, radius2, rotation),
                Z = z
            };
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
        }

        public static void CreateLacrymariaOlorEntity(float x, float y)
        {
            var entity = new LacrymariaOlorEntity()
            {
                RenderLogic = lacrymariaOlorRenderLogic,
                UpdateLogic = lacrymariaOlorUpdateLogic,
                BodyPosition = new Vector2(x, y),
                AABB = new RectangleF(x, y, 20, 20),
                Z = 0f
            };
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
        }

        public static UnidentifiedBlob1Entity CreateRandomUnidentifiedBlob1(int index)
        {
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var targetPosition = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var radius1 = 20;
            var entity = new UnidentifiedBlob1Entity()
            {
                LocalPosition = position,
                RenderLogic = ellipseRenderLogic,
                UpdateLogic = unidentifiedBlob1UpdateLogic,
                CourseDiviationSpeed = random.NextSingle() * 50 + 1,
                TargetPosition = targetPosition,
                Segment = new EllipseSegment(position.X, position.Y, radius1, 10, Color.LimeGreen, Color.GreenYellow, 2, 0, position.Z),
                MovementSpeedMultiplier = random.NextSingle() * 100 + 1,
                NextMovementSpeedMultiplierChange = random.NextDouble() * 4 + 1,
                Z = position.Z,
                NextFoodScan = index * 0.2f,
                DeathFromStarvationTime = random.NextDouble() * 240 + 1,
                AABB = G.SB.GetCircleAABB(new Vector2(position.X, position.Y), radius1),
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
                    new CircleSegment(position.X, position.Y, radius, color1, color2, 0.1f, position.Z),
                },
                UpdateLogic = UpdateLogic.NullLogic,
                RenderLogic = circleRenderLogic,
                AABB = G.SB.GetCircleAABB(new Vector2(position.X, position.Y), radius),
            };
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            entity.Leaf2 = G.StaticFoodEntitiesByLocation.Add(entity.AABB, entity);
            return entity;
        }
    }
}
