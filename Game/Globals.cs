using Apos.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public static class G {
        public static void Setup(Game game, GraphicsDeviceManager graphics) {
            Game = game;
            Window = Game.Window;
            Graphics = graphics;
            GraphicsDevice = Game.GraphicsDevice;

            Camera = new Camera(new DensityViewport(GraphicsDevice, Window, 2000f, 2000f));

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
    }
}
