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
        public static List<Entity> Entities = new List<Entity>();
        public static bool DisableBokeh = File.Exists("bokeh.disable");
    }
}
