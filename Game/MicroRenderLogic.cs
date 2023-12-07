using Microsoft.Xna.Framework;

namespace GameProject
{
    public class MicroRenderLogic : RenderLogic
    {
        public override void Render(Entity entity)
        {
            var segment = (MicroSegment)entity.Segments[0];
            G.B.Draw(segment.Shape, segment.Ramp, segment.CoreBlendBegin, segment.CoreBlendEnd, new Vector2(0f, 1f), Matrix32.CreateScale(segment.Size, segment.Size) * Matrix32.CreateTranslation(segment.Center - new Vector2(-segment.Size / 2f, -segment.Size / 2f)));
        }
    }
}
