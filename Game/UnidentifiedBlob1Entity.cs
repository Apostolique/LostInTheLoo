using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1Entity : TinyPetEntity
    {
        public float CourseDiviationSpeed;
        public EllipseSegment Segment;
        public UnidentifiedBlob1Entity Twin;
        public double SplitTimer;
        public float SplitScaleChangePerSecond;
        public float TargetRotation;

        public override float Radius1 { get => this.Segment.Radius1; set => this.Segment.Radius1 = value; }
        public override float Radius2 { get => this.Segment.Radius2; set => this.Segment.Radius2 = value; }
    }
}
