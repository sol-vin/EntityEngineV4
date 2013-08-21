using System;

namespace EntityEngineV4.PowerTools
{
    public static class RandomHelper
    {
        /// <summary>
        /// Gets a randomized boolean.
        /// </summary>
        /// <param name="random">The random</param>
        /// <param name="chance">The chance of rolling true, 2 means its true 1/2 times</param>
        /// <returns></returns>
        public static bool RandomBool(this Random random, int chance = 2)
        {
            return (random.Next(0, chance) == 0);
        }


        /// <summary>
        /// Returns a random sign value, either -1 or 1.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static int GetSign(this Random random, int chance = 2)
        {
            return (RandomBool(random, chance)) ? -1 : 1;
        }

        public static float GetFloat(this Random random)
        {
            return (float) random.NextDouble();
        }

        public static float GetFloat(this Random random, float min, float max)
        {
            return min + (max - min) * GetFloat(random);
        }
    }
}
