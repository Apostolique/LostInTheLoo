using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace GameProject
{
    public class TinyPetDefinition
    {
        public bool CanPoop = true;
        public double MinPoopTimeDelay = 10; // default value
        public double MaxPoopTimeDelay = 20; // default value
        public float BaseSize = 1;
        public float MaxScale = 5;
        public bool CanEatFood = true;
        public int[] EatsFoods = new int[] {
            StaticFoodTypes.Blue,
            StaticFoodTypes.Green,
            StaticFoodTypes.Red,
        };

        public float FoodBonusMultiplier = 1f / 10f;
        public double MinDigestTime = 1;
        public double MaxDigestTime = 2;
        public bool CanDieFromStarvation = true;
        public double DeathAnimationTime = 2; // how long is the death animation
        public double MinRandomMovementSpeedDelay = 10;
        public double MaxRandomMovementSpeedDelay = 15;
        public float MinRandomMovementSpeedMultiplier = 0;
        public float MaxRandomMovementSpeedMultiplier = 100;
        public double MinSinusMovementSpeedMultiplierSpeed = 2;
        public double MaxSinusMovementSpeedMultiplierSpeed = 20;
        public double MinSinusMovementSpeedMultiplierScale = 10;
        public double MaxSinusMovementSpeedMultiplierScale = 30;
        public Color Color1;
        public Color Color2;
    }
}
