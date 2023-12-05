using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class WorldTime : GameTime
    {
        public bool IsPaused;
        public double Speed = 1.0d;

        public static void Update(WorldTime worldTime, GameTime gameTime)
        {
            if (worldTime.IsPaused)
            {
                worldTime.ElapsedGameTime = TimeSpan.Zero;
                return;
            }

            var speed = worldTime.Speed;
            var elapsedRealSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            var elapsedWorldSeconds = elapsedRealSeconds * speed;

            worldTime.ElapsedGameTime = TimeSpan.FromSeconds(elapsedWorldSeconds);
            worldTime.TotalGameTime += worldTime.ElapsedGameTime;
        }
    }
}