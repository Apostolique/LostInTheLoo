using System;
using System.Collections.Generic;
using System.IO;
using Apos.Camera;
using Apos.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public static class G {
        public static void Setup(Game game, GraphicsDeviceManager graphics) {
            Game = game;
            Window = Game.Window;
            Graphics = graphics;
            GraphicsDevice = Game.GraphicsDevice;

            R = new Renderer();
        }

        public static Game Game;
        public static GameWindow Window;
        public static GraphicsDeviceManager Graphics;
        public static GraphicsDevice GraphicsDevice;
        public static SpriteBatch S;
        public static ShapeBatch SB;
        public static Renderer R;
        public static Camera Camera;
        public static AABBTree<Entity> EntitiesByLocation = new AABBTree<Entity>();
        public static AABBTree<Entity> StaticFoodEntitiesByLocation = new AABBTree<Entity>();
        public static readonly bool DisableBokeh = File.Exists("bokeh.disable");
        public const float WorldSize = 10000;
        public const float WorldSizeHalf = WorldSize * 0.5f;
        public const float WorldDepth = 0.5f;
        public const float WorldDepthHalf = WorldDepth * 0.5f;
        public static readonly Vector3 MaxWorldPosition = new Vector3(WorldSizeHalf, WorldSizeHalf, WorldDepth);
        public static readonly Vector3 MinWorldPosition = new Vector3(-WorldSizeHalf, -WorldSizeHalf, 0);
        public static readonly Random Random = new Random();
        public static readonly bool RenderAABB = File.Exists("renderaabb.enable");
    }
}
