using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UpdateLogic
    {
        public static UpdateLogic NullLogic = new UpdateLogic();

        public virtual void Update(Entity entity, GameTime gameTime)
        {
        }
    }
}