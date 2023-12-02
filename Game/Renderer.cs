using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public class Renderer {
        public Renderer() { }

        public void Clear(RenderTarget2D source) {
            G.GraphicsDevice.SetRenderTarget(source);
            G.GraphicsDevice.Clear(TWColor.Transparent);
        }
        public void Draw(RenderTarget2D source) {
            G.S.Begin();
            G.S.Draw(source, Vector2.Zero, TWColor.White);
            G.S.End();
        }

        public void DrawTo(RenderTarget2D source, RenderTarget2D destination) {
            G.GraphicsDevice.SetRenderTarget(destination);
            Draw(source);
            Clear(source);
        }

        public void ApplyBokeh(RenderTarget2D source, RenderTarget2D destination, float blurRadius, float z) {
            G.GraphicsDevice.SetRenderTarget(destination);

            Assets.Bokeh.Parameters["r"].SetValue(blurRadius * G.Camera.WorldToScreenScale(z));
            G.S.Begin(effect: Assets.Bokeh);
            G.S.Draw(source, Vector2.Zero, TWColor.White);
            G.S.End();

            Clear(source);
        }
    }
}
