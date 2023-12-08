using Num = System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;

namespace GameProject {
    public class Batch {
        public Batch(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _effect = Assets.Micro;

            _defaultEffect = _effect;
            _defaultPass = _effect.CurrentTechnique.Passes[0];

            _vertices = new VertexMicro[_initialVertices];
            _indices = new uint[_initialIndices];

            GenerateIndexArray();

            _vertexBuffer = new DynamicVertexBuffer(_graphicsDevice, typeof(VertexMicro), _vertices.Length, BufferUsage.WriteOnly);

            _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(uint), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _shapes = Assets.MicroShapes;
            _core = Assets.Tara;
        }

        // TODO: Shapes (filled, borders)
        //       Textures
        //       Shaders

        public void Begin(Matrix? view = null, Matrix? projection = null, Effect? effect = null) {
            if (view != null) {
                _view = view.Value;
            } else {
                _view = Matrix.Identity;
            }
            if (effect != null) {
                _effect = effect;
                _customEffect = true;
            } else {
                _effect = _defaultEffect;
                _customEffect = false;
            }

            if (projection != null) {
                _projection = projection.Value;
            } else {
                Viewport viewport = _graphicsDevice.Viewport;
                _projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            }
        }
        public void Draw(MicroShapes shape, MicroRamps ramp, float coreBlendBegin, float coreBlendEnd, Vector2 direction, Num.Matrix3x2? world = null, Color? color = null) {
            EnsureSizeOrDouble(ref _vertices, _vertexCount + 4);
            _indicesChanged = EnsureSizeOrDouble(ref _indices, _indexCount + 6) || _indicesChanged;

            // TODO: world shouldn't be null.
            if (world == null) {
                world = Num.Matrix3x2.Identity;
            }

            if (direction != Vector2.Zero) {
                direction.Normalize();
            }

            float r = ((float)ramp + 0.5f) / (float)MicroRamps.Count;

            int shapeOffset = (int)shape;

            float row = shapeOffset / 6;
            float column = shapeOffset - (row * 6);

            Num.Vector2 topLeft = new Num.Vector2(column * 256, row * 256);
            Num.Vector2 topRight = new Num.Vector2(column * 256 + 256, row * 256);
            Num.Vector2 bottomRight = new Num.Vector2(column * 256 + 256, row * 256 + 256);
            Num.Vector2 bottomLeft = new Num.Vector2(column * 256, row * 256 + 256);

            Num.Vector2 wTopLeft = Num.Vector2.Transform(new Num.Vector2(0f, 0f), world.Value);
            Num.Vector2 wTopRight = Num.Vector2.Transform(new Num.Vector2(1f, 0f), world.Value);
            Num.Vector2 wBottomRight = Num.Vector2.Transform(new Num.Vector2(1f, 1f), world.Value);
            Num.Vector2 wBottomLeft = Num.Vector2.Transform(new Num.Vector2(0, 1f), world.Value);

            _vertices[_vertexCount + 0] = new VertexMicro(
                new Vector3(wTopLeft.X, wTopLeft.Y, 0f),
                GetUVMirror(_shapes, topLeft, row * 256),
                GetUV(_core, topLeft),
                color ?? Color.White,
                r,
                coreBlendBegin,
                coreBlendEnd,
                direction
            );
            _vertices[_vertexCount + 1] = new VertexMicro(
                new Vector3(wTopRight.X, wTopRight.Y, 0f),
                GetUVMirror(_shapes, topRight, row * 256),
                GetUV(_core, topRight),
                color ?? Color.White,
                r,
                coreBlendBegin,
                coreBlendEnd,
                direction
            );
            _vertices[_vertexCount + 2] = new VertexMicro(
                new Vector3(wBottomRight.X, wBottomRight.Y, 0f),
                GetUVMirror(_shapes, bottomRight, row * 256),
                GetUV(_core, bottomRight),
                color ?? Color.White,
                r,
                coreBlendBegin,
                coreBlendEnd,
                direction
            );
            _vertices[_vertexCount + 3] = new VertexMicro(
                new Vector3(wBottomLeft.X, wBottomLeft.Y, 0f),
                GetUVMirror(_shapes, bottomLeft, row * 256),
                GetUV(_core, bottomLeft),
                color ?? Color.White,
                r,
                coreBlendBegin,
                coreBlendEnd,
                direction
            );

            _triangleCount += 2;
            _vertexCount += 4;
            _indexCount += 6;
        }
        public void End() {
            Flush();

            // TODO: Restore old states like rasterizer, depth stencil, blend state?
        }

        private void Flush() {
            if (_triangleCount == 0) return;

            _defaultEffect.Parameters["view_projection"]?.SetValue(_view * _projection);
            _defaultEffect.Parameters["Ramp"]?.SetValue(Assets.MicroRamps);
            _defaultEffect.Parameters["core_texture"]?.SetValue(_core);
            // Apply the default pass in case a custom shader doesn't provide a vertex shader.
            _defaultPass.Apply();

            if (_indicesChanged) {
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();

                _vertexBuffer = new DynamicVertexBuffer(_graphicsDevice, typeof(VertexMicro), _vertices.Length, BufferUsage.WriteOnly);

                GenerateIndexArray();

                _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(uint), _indices.Length, BufferUsage.WriteOnly);
                _indexBuffer.SetData(_indices);

                _indicesChanged = false;
            }

            _vertexBuffer.SetData(_vertices);
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            _graphicsDevice.Indices = _indexBuffer;

            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;

            if (_customEffect) {
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    _graphicsDevice.Textures[0] = _shapes;

                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _triangleCount);
                }
            } else {
                _graphicsDevice.Textures[0] = _shapes;
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _triangleCount);
            }

            _triangleCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
        }

        private bool EnsureSizeOrDouble<T>(ref T[] array, int neededCapacity) {
            if (array.Length < neededCapacity) {
                Array.Resize(ref array, array.Length * 2);
                return true;
            }
            return false;
        }

        private Vector2 GetUV(Texture2D texture, Num.Vector2 xy) {
            return new Vector2(xy.X / texture.Width, xy.Y / texture.Height);
        }
        private Vector3 GetUVMirror(Texture2D texture, Num.Vector2 xy, float row) {
            return new Vector3(xy.X / texture.Width, xy.Y / texture.Height, (256 - (xy.Y - row) + row) / texture.Height);
        }

        private void GenerateIndexArray() {
            uint i = Floor(_fromIndex, 6, 6);
            uint j = Floor(_fromIndex, 6, 4);
            for (; i < _indices.Length; i += 6, j += 4) {
                _indices[i + 0] = j + 0;
                _indices[i + 1] = j + 1;
                _indices[i + 2] = j + 3;
                _indices[i + 3] = j + 1;
                _indices[i + 4] = j + 2;
                _indices[i + 5] = j + 3;
            }
            _fromIndex = _indices.Length;
        }
        private uint Floor(int value, int div, uint mul) {
            return (uint)MathF.Floor((float)value / div) * mul;
        }

        public enum MicroRamps {
            Ramp01a,
            Ramp02a,
            Ramp03a,
            Ramp04a,
            Ramp05a,
            Ramp06a,
            Ramp07a,
            Ramp08a,
            Ramp09a,
            Ramp10a,
            Ramp11a,
            Ramp12a,
            Ramp01b,
            Ramp02b,
            Ramp03b,
            Ramp04b,
            Ramp05b,
            Ramp06b,
            Ramp07b,
            Ramp08b,
            Ramp09b,
            Ramp10b,
            Ramp11b,
            Ramp12b,
            Ramp01c,
            Ramp02c,
            Ramp03c,
            Ramp04c,
            Ramp05c,
            Ramp06c,
            Ramp07c,
            Ramp08c,
            Ramp09c,
            Ramp10c,
            Ramp11c,
            Ramp12c,
            Ramp01d,
            Ramp02d,
            Ramp03d,
            Ramp04d,
            Ramp05d,
            Ramp06d,
            Ramp07d,
            Ramp08d,
            Ramp09d,
            Ramp10d,
            Ramp11d,
            Ramp12d,
            Count
        }
        public Array Ramps = Enum.GetValues(typeof(MicroRamps));

        public enum MicroShapes {
            Ovoid,
            Ovoid2,
            Cylindrical,
            Cylindrical2,
            Cylindrical3,
            Cylindrical4,
            Cylindrical5,
            Triangle,
            Triangle2,
            Triangle3,
            Triangle4,
            Triangle5,
            Triangle6,
            Triangle7,
            Triangle8,
            Triangle9,
            Triangle10,
            Square,
            Square2,
            Square3,
            Square4,
            Square5,
            Square6,
            Skewer,
        }
        public Array Shapes = Enum.GetValues(typeof(MicroShapes));

        private const int _initialSprites = 2048;
        private const int _initialTriangles = _initialSprites * 2;
        private const int _initialVertices = _initialSprites * 4;
        private const int _initialIndices = _initialSprites * 6;

        private GraphicsDevice _graphicsDevice;
        private RasterizerState _rasterizerState = new RasterizerState {
            CullMode = CullMode.None
        };

        private VertexMicro[] _vertices;
        private uint[] _indices;
        private int _triangleCount = 0;
        private int _vertexCount = 0;
        private int _indexCount = 0;
        private Texture2D _shapes;
        private Texture2D _core;

        private DynamicVertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private Matrix _view;
        private Matrix _projection;
        private Effect _defaultEffect;
        private EffectPass _defaultPass;
        private Effect _effect;
        private bool _customEffect = false;

        private bool _indicesChanged = false;
        private int _fromIndex = 0;
    }
}
