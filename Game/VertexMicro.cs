using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexMicro(Vector3 position, Vector2 textureCoordinate1, Vector2 textureCoordinate2, Color c, float ramp, float coreBlendBegin, float coreBlendEnd, Vector2 direction) : IVertexType {
        public Vector3 Position = position;
        public Vector2 TextureCoordinate1 = textureCoordinate1;
        public Vector2 TextureCoordinate2 = textureCoordinate2;
        public Color Color = c;
        public Vector4 Meta1 = new Vector4(ramp, coreBlendBegin, coreBlendEnd, 0);
        public Vector4 Meta2 = new Vector4(direction.X, direction.Y, 0, 0);
        public static readonly VertexDeclaration VertexDeclaration;

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override readonly int GetHashCode() {
            return System.HashCode.Combine(Position, TextureCoordinate1, TextureCoordinate2, Color, Meta1, Meta2);
        }

        public override readonly string ToString() {
            return
                "{{Position:" + Position +
                " Color1:" + Color +
                " TextureCoordinate1:" + TextureCoordinate1 +
                " TextureCoordinate2:" + TextureCoordinate2 +
                " Thickness:" + Meta1.X +
                " Shape:" + Meta1.Y +
                " PixelSize:" + Meta1.Z +
                " Width:" + Meta1.W +
                "}}";
        }

        public static bool operator ==(VertexMicro left, VertexMicro right) {
            return
                left.Position == right.Position &&
                left.TextureCoordinate1 == right.TextureCoordinate1 &&
                left.TextureCoordinate2 == right.TextureCoordinate2 &&
                left.Color == right.Color &&
                left.Meta1 == right.Meta1 &&
                left.Meta2 == right.Meta2;
        }

        public static bool operator !=(VertexMicro left, VertexMicro right) {
            return !(left == right);
        }

        public override readonly bool Equals(object? obj) {
            if (obj == null)
                return false;

            if (obj.GetType() != base.GetType())
                return false;

            return this == ((VertexMicro)obj);
        }

        static VertexMicro() {
            var elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(sizeof(float) * 7, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 7 + sizeof(int), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(sizeof(float) * 11 + sizeof(int), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
            };
            VertexDeclaration = new VertexDeclaration(elements);
        }
    }
}
