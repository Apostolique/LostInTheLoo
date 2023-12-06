using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameProject
{
    public class TinyPetDefinition
    {
        public bool CanPoop;
        public double MinPoopTimeDelay = 10; // default value
        public double MaxPoopTimeDelay = 20; // default value
        public float BaseRadius1 = 1;
        public float BaseRadius2 = 1;
        public float MaxScale = 5;
    }
}