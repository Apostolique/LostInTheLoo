using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexShape : IVertexType {
        public VertexShape(Vector3 position, Vector2 textureCoordinate1, Vector2 textureCoordinate2, float shape, Color c1, Color c2, float thickness, float sdfSize, float pixelSize, float height = 1.0f, float aaSize = 2f, float rounded = 0f) {
            if (thickness <= 0f) {
                c2 = c1;
                thickness = 0f;
            }

            Position = position;
            TextureCoordinate1 = textureCoordinate1;
            TextureCoordinate2 = textureCoordinate2;
            Color1 = c1;
            Color2 = c2;

            Meta1 = new Vector4(thickness, shape, sdfSize, height);
            Meta2 = new Vector4(pixelSize, aaSize, rounded, 0f);
        }

        public Vector3 Position;
        public Vector2 TextureCoordinate1;
        public Vector2 TextureCoordinate2;
        public Color Color1;
        public Color Color2;
        public Vector4 Meta1;
        public Vector4 Meta2;
        public static readonly VertexDeclaration VertexDeclaration;

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode() {
            unchecked {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate1.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate2.GetHashCode();
                hashCode = (hashCode * 397) ^ Color1.GetHashCode();
                hashCode = (hashCode * 397) ^ Color2.GetHashCode();
                hashCode = (hashCode * 397) ^ Meta1.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() {
            return
                "{{Position:" + this.Position +
                " Color1:" + this.Color1 +
                " Color2:" + this.Color1 +
                " TextureCoordinate1:" + this.TextureCoordinate1 +
                " TextureCoordinate2:" + this.TextureCoordinate2 +
                " Thickness:" + this.Meta1.X +
                " Shape:" + this.Meta1.Y +
                " PixelSize:" + this.Meta1.Z +
                " Width:" + this.Meta1.W +
                "}}";
        }

        public static bool operator ==(VertexShape left, VertexShape right) {
            return
                left.Position == right.Position &&
                left.TextureCoordinate1 == right.TextureCoordinate1 &&
                left.TextureCoordinate2 == right.TextureCoordinate2 &&
                left.Color1 == right.Color1 &&
                left.Color2 == right.Color2 &&
                left.Meta1 == right.Meta1 &&
                left.Meta2 == right.Meta2;
        }

        public static bool operator !=(VertexShape left, VertexShape right) {
            return !(left == right);
        }

        public override bool Equals(object? obj) {
            if (obj == null)
                return false;

            if (obj.GetType() != base.GetType())
                return false;

            return this == ((VertexShape)obj);
        }

        static VertexShape() {
            var elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(sizeof(float) * 7, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 7 + sizeof(int), VertexElementFormat.Color, VertexElementUsage.Color, 1),
                new VertexElement(sizeof(float) * 7 + sizeof(int) * 2, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(sizeof(float) * 11 + sizeof(int) * 2, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
            };
            VertexDeclaration = new VertexDeclaration(elements);
        }
    }
}
