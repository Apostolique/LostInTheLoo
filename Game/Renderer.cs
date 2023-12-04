using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Apos.Input;
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

        public void ApplyBokeh(RenderTarget2D source, RenderTarget2D real, RenderTarget2D imaginary, RenderTarget2D temp, RenderTarget2D destination, float z, float blur, float opacity) {
            if (G.DisableBokeh) {
                G.GraphicsDevice.SetRenderTarget(destination);
                G.S.Begin();
                G.S.Draw(source, Vector2.Zero, TWColor.White * opacity);
                G.S.End();
            } else {
                Bokeh(16, 1);
                G.GraphicsDevice.SetRenderTarget(temp);
                G.GraphicsDevice.Clear(TWColor.Transparent);

                for (int i = 0; i < _componentsCount; i++) {
                    G.GraphicsDevice.SetRenderTargets(real, imaginary);
                    G.GraphicsDevice.Clear(TWColor.Transparent);

                    blur = InputHelper.NewMouse.X / 1000f;

                    Assets.BokehVertical.Parameters["kernelSize"]?.SetValue((float)_kernelSize);
                    Assets.BokehVertical.Parameters["kernel"]?.SetValue(_kernels[i]);
                    Assets.BokehVertical.Parameters["radius"]?.SetValue(blur);

                    G.S.Begin(effect: Assets.BokehVertical);
                    G.S.Draw(source, Vector2.Zero, TWColor.White);
                    G.S.End();

                    G.GraphicsDevice.SetRenderTarget(temp);

                    Assets.BokehHorizontal.Parameters["kernelSize"]?.SetValue((float)_kernelSize);
                    Assets.BokehHorizontal.Parameters["kernel"]?.SetValue(_kernels[i]);
                    Assets.BokehHorizontal.Parameters["radius"]?.SetValue(blur);
                    Assets.BokehHorizontal.Parameters["imaginary"]?.SetValue(imaginary);
                    Assets.BokehHorizontal.Parameters["z"]?.SetValue(_kernelParameters[i].Z);
                    Assets.BokehHorizontal.Parameters["w"]?.SetValue(_kernelParameters[i].W);
                    G.S.Begin(effect: Assets.BokehHorizontal);
                    G.S.Draw(real, Vector2.Zero, TWColor.White);
                    G.S.End();
                }

                G.GraphicsDevice.SetRenderTarget(destination);
                G.S.Begin();
                G.S.Draw(temp, Vector2.Zero, TWColor.White);
                G.S.End();
            }

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

        // https://github.com/Sergio0694/ComputeSharp/blob/main/samples/ComputeSharp.ImageProcessing/Processors/HlslBokehBlurProcessor.Implementation.cs
        private void Bokeh(int radius, int componentsCount) {
            _radius = radius;
            _kernelSize = _radius * 2 + 1;
            _componentsCount = componentsCount;

            (int radius, int componentsCount) parameters = (_radius, _componentsCount);

            // Reuse the initialized values from the cache, if possible
            if (_cache.TryGetValue(parameters, out (Vector4[] Parameters, float Scale, Vector2[][] Kernels) info)) {
                _kernelParameters = info.Parameters;
                _kernelsScale = info.Scale;
                _kernels = info.Kernels;
            } else {
                // Initialize the complex kernels and parameters with the current arguments
                (_kernelParameters, _kernelsScale) = GetParameters();
                _kernels = CreateComplexKernels();

                NormalizeKernels();

                // Store them in the cache for future use
                _ = _cache.TryAdd(parameters, (_kernelParameters, _kernelsScale, _kernels));
            }
        }
        private (Vector4[] Parameters, float Scale) GetParameters() {
            int index = Math.Max(0, Math.Min(_componentsCount - 1, KernelComponents.Length));
            return (KernelComponents[index], KernelScales[index]);
        }
        private Vector2[][] CreateComplexKernels() {
            Vector2[][] kernels = new Vector2[_kernelParameters.Length][];
            ref Vector4 baseRef = ref MemoryMarshal.GetReference(_kernelParameters.AsSpan());

            for (int i = 0; i < _kernelParameters.Length; i++) {
                ref Vector4 paramsRef = ref Unsafe.Add(ref baseRef, i);
                kernels[i] = CreateComplex1DKernel(paramsRef.X, paramsRef.Y);
            }

            return kernels;
        }
        private Vector2[] CreateComplex1DKernel(float a, float b) {
            Vector2[] kernel = new Vector2[_kernelSize];
            ref Vector2 baseRef = ref MemoryMarshal.GetReference(kernel.AsSpan());
            int r = _radius;
            int n = -r;

            for (int i = 0; i < _kernelSize; i++, n++) {
                float value = n * _kernelsScale * (1f / r);
                value *= value;

                Unsafe.Add(ref baseRef, i) = new Vector2(
                    (float)(Math.Exp(-a * value) * Math.Cos(b * value)),
                    (float)(Math.Exp(-a * value) * Math.Sin(b * value))
                );
            }

            return kernel;
        }
        private void NormalizeKernels() {
            // Calculate the complex weighted sum
            float total = 0;
            Span<Vector2[]> kernelsSpan = _kernels.AsSpan();
            ref Vector2[] baseKernelsRef = ref MemoryMarshal.GetReference(kernelsSpan);
            ref Vector4 baseParamsRef = ref MemoryMarshal.GetReference(_kernelParameters.AsSpan());

            for (int i = 0; i < _kernelParameters.Length; i++) {
                ref Vector2[] kernelRef = ref Unsafe.Add(ref baseKernelsRef, i);
                int length = kernelRef.Length;
                ref Vector2 valueRef = ref kernelRef[0];
                ref Vector4 paramsRef = ref Unsafe.Add(ref baseParamsRef, i);

                for (int j = 0; j < length; j++) {
                    for (int k = 0; k < length; k++) {
                        ref Vector2 jRef = ref Unsafe.Add(ref valueRef, j);
                        ref Vector2 kRef = ref Unsafe.Add(ref valueRef, k);

                        total +=
                            (paramsRef.Z * ((jRef.X * kRef.X) - (jRef.Y * kRef.Y))) +
                            (paramsRef.W * ((jRef.X * kRef.Y) + (jRef.Y * kRef.X)));
                    }
                }
            }

            // Normalize the kernels
            float scalar = 1f / (float)Math.Sqrt(total);

            for (int i = 0; i < kernelsSpan.Length; i++) {
                ref Vector2[] kernelsRef = ref Unsafe.Add(ref baseKernelsRef, i);
                int length = kernelsRef.Length;
                ref Vector2 valueRef = ref kernelsRef[0];

                for (int j = 0; j < length; j++) {
                    Unsafe.Add(ref valueRef, j) = c_mul(Unsafe.Add(ref valueRef, j), scalar);
                }
            }
        }

        Vector2 c_mul(Vector2 left, float right) {
            return new Vector2(left.X * right, left.Y * right);
        }

        private int _radius = 32;
        private int _kernelSize;
        private int _componentsCount = 2;
        private Vector4[] _kernelParameters;
        private Vector2[][] _kernels;
        private float _kernelsScale;

        private Dictionary<(int Radius, int ComponentsCount), (Vector4[] Parameters, float Scale, Vector2[][] Kernels)> _cache = [];

        private float[] KernelScales = [1.4f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f];
        private Vector4[][] KernelComponents = [
            // 1 component
            [new Vector4(0.862325f, 1.624835f, 0.767583f, 1.862321f)],

            // 2 components
            [
                new Vector4(0.886528f, 5.268909f, 0.411259f, -0.548794f),
                new Vector4(1.960518f, 1.558213f, 0.513282f, 4.56111f)
            ],

            // 3 components
            [
                new Vector4(2.17649f, 5.043495f, 1.621035f, -2.105439f),
                new Vector4(1.019306f, 9.027613f, -0.28086f, -0.162882f),
                new Vector4(2.81511f, 1.597273f, -0.366471f, 10.300301f)
            ],

            // 4 components
            [
                new Vector4(4.338459f, 1.553635f, -5.767909f, 46.164397f),
                new Vector4(3.839993f, 4.693183f, 9.795391f, -15.227561f),
                new Vector4(2.791880f, 8.178137f, -3.048324f, 0.302959f),
                new Vector4(1.342190f, 12.328289f, 0.010001f, 0.244650f)
            ],

            // 5 components
            [
                new Vector4(4.892608f, 1.685979f, -22.356787f, 85.91246f),
                new Vector4(4.71187f, 4.998496f, 35.918936f, -28.875618f),
                new Vector4(4.052795f, 8.244168f, -13.212253f, -1.578428f),
                new Vector4(2.929212f, 11.900859f, 0.507991f, 1.816328f),
                new Vector4(1.512961f, 16.116382f, 0.138051f, -0.01f)
            ],

            // 6 components
            [
                new Vector4(5.143778f, 2.079813f, -82.326596f, 111.231024f),
                new Vector4(5.612426f, 6.153387f, 113.878661f, 58.004879f),
                new Vector4(5.982921f, 9.802895f, 39.479083f, -162.028887f),
                new Vector4(6.505167f, 11.059237f, -71.286026f, 95.027069f),
                new Vector4(3.869579f, 14.81052f, 1.405746f, -3.704914f),
                new Vector4(2.201904f, 19.032909f, -0.152784f, -0.107988f)
            ]
        ];
    }
}
