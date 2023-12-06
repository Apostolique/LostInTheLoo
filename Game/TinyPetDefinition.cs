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
        public bool CanEatFood;
        public int[] EatsFoods = Array.Empty<int>();
        public float FoodBonusMultiplier = 1;
        public double MinDigestTime = 1;
        public double MaxDigestTime = 2;
        public bool CanDieFromStarvation;
        public double DeathAnimationTime = 2; // how long is the death animation
        public double MinRandomMovementSpeedDelay = 10;
        public double MaxRandomMovementSpeedDelay = 15;
        public float MinRandomMovementSpeedMultiplier = 0;
        public float MaxRandomMovementSpeedMultiplier = 100;
        public double MinSinusMovementSpeedMultiplierSpeed = 2;
        public double MaxSinusMovementSpeedMultiplierSpeed = 20;
        public double MinSinusMovementSpeedMultiplierScale = 10;
        public double MaxSinusMovementSpeedMultiplierScale = 30;
    }
}