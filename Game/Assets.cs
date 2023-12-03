using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public static class Assets {
        public static void LoadAssets(ContentManager content, GraphicsDevice graphicsDevice) {
            LoadFonts(content);
            LoadTextures(content);
            LoadShaders(content);
            LoadSounds(content);
        }

        public static void LoadJson() {
            Settings = Utility.EnsureJson("Settings.json", SettingsContext.Default.Settings);
        }
        public static void LoadFonts(ContentManager content) {
            FontSystem = new FontSystem();
            FontSystem.AddFont(TitleContainer.OpenStream($"{content.RootDirectory}/source-code-pro-medium.ttf"));
        }
        private static void LoadTextures(ContentManager content) {
            Background = content.Load<Texture2D>("background");
            Noise1 = content.Load<Texture2D>("noise1");
            Noise2 = content.Load<Texture2D>("noise2");
        }
        private static void LoadShaders(ContentManager content) {
            Shapes = content.Load<Effect>("apos-shapes");
            Bokeh = content.Load<Effect>("bokeh");
            Infinite = content.Load<Effect>("infinite");
            Mask = content.Load<Effect>("mask");
        }
        private static void LoadSounds(ContentManager content) {
            Death = content.Load<SoundEffect>("music/LITL T1 Death");
            S1 = content.Load<SoundEffect>("music/LITL T1 S1");
            S2 = content.Load<SoundEffect>("music/LITL T1 S2");
            S3 = content.Load<SoundEffect>("music/LITL T1 S3");
            S4 = content.Load<SoundEffect>("music/LITL T1 S4");
            Low = content.Load<SoundEffect>("music/LITL T1 Low");
            Medium = content.Load<SoundEffect>("music/LITL T1 Medium");
            MediumHigh = content.Load<SoundEffect>("music/LITL T1 Medium High");
            High = content.Load<SoundEffect>("music/LITL T1 High");
        }

        public static Settings Settings;
        public static FontSystem FontSystem;

        public static Texture2D Background;
        public static Texture2D Noise1;
        public static Texture2D Noise2;

        public static Effect Bokeh;
        public static Effect Shapes;
        public static Effect Infinite;
        public static Effect Mask;

        public static SoundEffect Death;
        public static SoundEffect S1;
        public static SoundEffect S2;
        public static SoundEffect S3;
        public static SoundEffect S4;
        public static SoundEffect Low;
        public static SoundEffect Medium;
        public static SoundEffect MediumHigh;
        public static SoundEffect High;
    }
}
