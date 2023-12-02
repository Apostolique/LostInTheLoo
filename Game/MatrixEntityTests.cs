using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public static class MatrixEntityTests
    {
        public static void Test()
        {
            var absoluteRotationEntity = new MatrixEntity()
            {
                LocalPosition = new Vector3(1, 2, 3),
                LocalRotationOrbit = 15,
                LocalRotationSpin = 4,
                LocalScale = new Vector3(5, 6, 7),
                Parent = new MatrixEntity()
                {
                    LocalPosition = new Vector3(8, 9, 10),
                    LocalRotationOrbit = 16,
                    LocalRotationSpin = 11,
                    LocalScale = new Vector3(12, 13, 14),
                },
            };
            absoluteRotationEntity.Parent.AddChild(absoluteRotationEntity);
            absoluteRotationEntity.Parent.UpdateAbsoluteRecursive();
            Debug.Assert(absoluteRotationEntity.AbsoluteRotation == (15+4+16+11));

            var entity2 = new MatrixEntity()
            {
                LocalPosition = new Vector3(22, 0, 0),
                LocalRotationSpin = MathHelper.Pi,
                LocalScale = Vector3.One * 2.0f,
            };
            entity2.UpdateAbsoluteRecursive();
            Debug.Assert(entity2.AbsolutePosition == new Vector3(22, 0, 0));
            
            var entity = new MatrixEntity()
            {
                LocalPosition = Vector3.UnitX,
                Parent = new MatrixEntity()
                {
                    LocalPosition = Vector3.UnitX,
                }
            };
            entity.Parent.AddChild(entity);
            entity.Parent.UpdateAbsoluteRecursive();
            Debug.Assert(entity.AbsolutePosition == new Vector3(2, 0, 0));
        }
    }
}