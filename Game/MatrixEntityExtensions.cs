using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public static class MatrixEntityExtensions
    {
        // does not check if parent is a decendant of the child so behave!
        public static void AddChild(this MatrixEntity parent, MatrixEntity child)
        {
            if(child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }

            child.Parent = parent;
            parent.Children.Add(child);
        }

        public static void UpdateAbsoluteRecursive(this MatrixEntity entity)
        {
            entity.AbsoluteRotation
                = entity.LocalRotationSpin
                + entity.LocalRotationOrbit
                + (entity.Parent == null ? 0 : entity.Parent.AbsoluteRotation)
                ;
            entity.AbsoluteWorld
                = Matrix.Identity
                * Matrix.CreateScale(entity.LocalScale)
                * Matrix.CreateRotationZ(entity.LocalRotationSpin)
                * Matrix.CreateTranslation(entity.LocalPosition)
                * Matrix.CreateRotationZ(entity.LocalRotationOrbit)
                * (entity.Parent == null ? Matrix.Identity : entity.Parent.AbsoluteWorld)
                ;
            entity.AbsoluteWorld.Decompose(out var scale, out var rotation, out var position);
            entity.AbsolutePosition = position;
            entity.AbsoluteScale = scale;

            foreach(var child in entity.Children)
            {
                child.UpdateAbsoluteRecursive();
            }
        }
    }
}