using Microsoft.Xna.Framework;

namespace GameProject {
    public class Moves {
        public class Line {
            public Line(Vector2 xy, Vector2 velocity) {
                XY = xy;
                Velocity = velocity;
            }

            public Vector2 XY;
            public Vector2 Velocity;

            public void Update(GameTime gameTime) {
                XY += Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }
}
