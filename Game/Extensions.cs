using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var element in source)
            {
                action(element);
            }
        }

        public static Vector2 ToVector2XY(this Vector3 vector)
        {
            var result = new Vector2(vector.X, vector.Y);
            return result;
        }

        public static Vector3 NextVector3(this Random random, Vector3 min, Vector3 max)
        {
            var x = random.NextSingle() * (max.X - min.X) + min.X;
            var y = random.NextSingle() * (max.Y - min.Y) + min.Y;
            var z = random.NextSingle() * (max.Z - min.Z) + min.Z;
            var result = new Vector3(x, y, z);
            return result;
        }

        public static Vector3 ToVector3XY(this Vector2 vector, float z)
        {
            var result = new Vector3(vector.X, vector.Y, z);
            return result;
        }

        public static Vector3 ToVector3XY(this MonoGame.Extended.Point2 vector, float z)
        {
            var result = new Vector3(vector.X, vector.Y, z);
            return result;
        }
    }
}