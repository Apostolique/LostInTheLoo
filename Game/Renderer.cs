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

            var effect = G.DisableBokeh ? null : Assets.Bokeh;
            Assets.Bokeh.Parameters["r"].SetValue(blurRadius * G.Camera.WorldToScreenScale(z));
            G.S.Begin(effect: effect);
            G.S.Draw(source, Vector2.Zero, TWColor.White);
            G.S.End();

            Clear(source);
        }

        public void DrawInfinite(Texture2D t, RenderTarget2D destination, float z, float scale, Vector2 offset) {
            G.GraphicsDevice.SetRenderTarget(destination);

            Matrix uvTransform = GetUVTransform(t, offset, scale, z);
            Assets.Infinite.Parameters["view_projection"].SetValue(G.Camera.GetProjection());
            Assets.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uvTransform));

            G.S.Begin(effect: Assets.Infinite, samplerState: SamplerState.LinearWrap);
            G.S.Draw(t, G.GraphicsDevice.Viewport.Bounds, TWColor.White);
            G.S.End();
        }

        public void ApplyMask(RenderTarget2D source, RenderTarget2D mask, RenderTarget2D destination) {
            G.GraphicsDevice.SetRenderTarget(destination);

            Assets.Mask.Parameters["mask_texture"].SetValue(mask);

            G.S.Begin(effect: Assets.Mask);
            G.S.Draw(source, Vector2.Zero, TWColor.White);
            G.S.End();

            Clear(source);
        }

        private Matrix GetUVTransform(Texture2D t, Vector2 offset, float scale, float z) {
            return
                Matrix.CreateScale(t.Width, t.Height, 1f) *
                Matrix.CreateScale(scale, scale, 1f) *
                Matrix.CreateTranslation(offset.X, offset.Y, 0f) *
                G.Camera.GetView(z) *
                Matrix.CreateScale(1f / G.Camera.VirtualViewport.Width, 1f / G.Camera.VirtualViewport.Height, 1f);
        }
    }
}
