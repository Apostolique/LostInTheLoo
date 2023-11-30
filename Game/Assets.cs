using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public static class Assets {
        public static void LoadAssets(ContentManager content, GraphicsDevice graphicsDevice) {
            LoadTextures(content);
            LoadShaders(content);
        }

        public static void LoadJson() {
            Settings = Utility.EnsureJson("Settings.json", SettingsContext.Default.Settings);
        }
        private static void LoadTextures(ContentManager content) {
            Background = content.Load<Texture2D>("background");
        }
        private static void LoadShaders(ContentManager content) {
            Shapes = content.Load<Effect>("apos-shapes");
            Bokeh = content.Load<Effect>("bokeh");
        }

        public static Settings Settings;
        public static Texture2D Background;
        public static Effect Bokeh;
        public static Effect Shapes;
    }
}
