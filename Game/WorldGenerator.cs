using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject
{
    public static class WorldGenerator
    {
        private const int maxUnidentifiedBlob1 = 1000;

        private static CircleRenderLogic circleRenderLogic = new CircleRenderLogic();
        private static EllipseRenderLogic ellipseRenderLogic = new EllipseRenderLogic();
        private static CircleWithMaskRenderLogic circleWithMaskRenderLogic = new CircleWithMaskRenderLogic();
        private static LacrymariaOlorRenderLogic lacrymariaOlorRenderLogic = new LacrymariaOlorRenderLogic();
        private static LacrymariaOlorUpdateLogic lacrymariaOlorUpdateLogic = new LacrymariaOlorUpdateLogic();
        private static UnidentifiedBlob1UpdateLogic unidentifiedBlob1UpdateLogic = new UnidentifiedBlob1UpdateLogic();
        private static readonly Random random = new Random();

        public static void Generate()
        {
            Enumerable.Range(0, maxUnidentifiedBlob1).ForEach(_ => CreateRandomUnidentifiedBlob1());
            CreateLacrymariaOlorEntity(50, 50);
            CreateCircleEntity(100, 100, 20, TWColor.Red500, TWColor.White, 2, -0.2f, 10);
            CreateCircleEntity(0, 0, 20, TWColor.Blue200, TWColor.Black, 1, -0.1f, 2);
            CreateEllipseEntity(0, 0, 50, 20, TWColor.Pink300, TWColor.Gray800, 1, 0, 0, 0);
            CreateCircleWithMaskEntity(-100, -100, 20, TWColor.White, 0, Assets.Noise1, 1, Vector2.Zero, TWColor.Black);
        }

        private static void CreateCircleEntity(float centerX, float centerY, float radius, Color color1, Color color2, float thickness, float z, float bokehBlurRadius)
        {
            var entity = new Entity()
            {
                RenderLogic = circleRenderLogic,
                UpdateLogic = UpdateLogic.NullLogic,
                Segments = new Segment[]
                {
                    new CircleSegment(centerX, centerY, radius, color1, color2, thickness, z, bokehBlurRadius),
                },
                AABB = new RectangleF(centerX - radius * 0.5f, centerY - radius * 0.5f, radius, radius),
                Z = z
            };
            G.EntitiesByLocation.Add(entity.AABB, entity);
            G.Entities.Add(entity);
        }

        private static void CreateEllipseEntity(float centerX, float centerY, float radius1, float radius2, Color color1, Color color2, float thickness, float rotation, float z, float bokehBlurRadius)
        {
            var entity = new Entity()
            {
                RenderLogic = ellipseRenderLogic,
                UpdateLogic = UpdateLogic.NullLogic,
                Segments = new Segment[]
                {
                    new EllipseSegment(centerX, centerY, radius1, radius2, color1, color2, thickness, rotation, z, bokehBlurRadius),
                },
                AABB = new RectangleF(centerX - radius1 * 0.5f, centerY - radius2 * 0.5f, radius1, radius2),
                Z = z
            };
            G.EntitiesByLocation.Add(entity.AABB, entity);
            G.Entities.Add(entity);
        }

        private static void CreateCircleWithMaskEntity(float centerX, float centerY, float radius, Color color, float z, Texture2D texture, float scale, Vector2 offset, Color clearColor)
        {
            var entity = new Entity()
            {
                RenderLogic = circleWithMaskRenderLogic,
                UpdateLogic = UpdateLogic.NullLogic,
                Segments = new Segment[]
                {
                    new CircleWithMaskSegment(centerX, centerY, radius, color, z, texture, scale, offset, clearColor),
                },
                AABB = new RectangleF(centerX - radius * 0.5f, centerY - radius * 0.5f, radius, radius),
                Z = z
            };
            G.EntitiesByLocation.Add(entity.AABB, entity);
            G.Entities.Add(entity);
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
            G.Entities.Add(entity);
            G.EntitiesByLocation.Add(entity.AABB, entity);
        }

        public static void CreateRandomUnidentifiedBlob1()
        {
            var position = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var targetPosition = random.NextVector3(G.MinWorldPosition, G.MaxWorldPosition);
            var entity = new UnidentifiedBlob1Entity()
            {
                LocalPosition = position,
                RenderLogic = ellipseRenderLogic,
                UpdateLogic = unidentifiedBlob1UpdateLogic,
                CourseDiviationSpeed = random.NextSingle() * 50 + 1,
                TargetPosition = targetPosition,
                Segment = new EllipseSegment(position.X, position.Y, 20, 10, Color.LimeGreen, Color.GreenYellow, 2, 0, position.Z, 10),
                MovementSpeedMultiplier = random.NextSingle() * 100 + 1,
                NextMovementSpeedMultiplierChange = random.NextDouble() * 4 + 1,
                Z = position.Z,
            };
            entity.Segments = new Segment[] { entity.Segment };
            G.Entities.Add(entity);
            entity.Leaf = G.EntitiesByLocation.Add(entity.AABB, entity);
            entity.UpdateAbsoluteRecursive();
        }
    }
}
