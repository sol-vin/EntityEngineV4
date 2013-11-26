using System;
using EntityEngineV4.Data;

namespace EntityEngineV4.PowerTools
{
    public static class RandomHelper
    {
        private static Random _random = new Random();
        /// <summary>
        /// Gets a randomized boolean.
        /// </summary>
        /// <param name="random">The random</param>
        /// <param name="chance">The chance of rolling true, 2 means its true 1/2 times</param>
        /// <returns></returns>
        public static bool RandomBool(int chance = 2)
        {
            return (_random.Next(0, chance) == 0);
        }


        /// <summary>
        /// Returns a random sign value, either -1 or 1.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static int GetSign(int chance = 2)
        {
            return (RandomBool(chance)) ? -1 : 1;
        }

        public static float GetFloat()
        {
            return (float) _random.NextDouble();
        }

        /// <summary>
        /// Get's a randomf loat within the specified range
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetFloat(float min, float max)
        {
            return min + (max - min) * GetFloat();
        }
        
        public static float NextGaussian(float average, float variation)
        {
            return average + 2.0f * ((float)_random.NextDouble() - 0.5f) * variation;
        }

        public static int NextInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static HSVColor RandomHue()
        {
            return new HSVColor(GetFloat(), 1f, 1f, 1f);
        }
    }
}
