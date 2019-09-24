namespace BrainAI.Config
{
    using System;

    public class DefaultRandom : IRandom
    {
        private readonly Random r = new Random();

        public float NextFloat()
        {
            return (float)this.r.NextDouble();
        }

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Range(int min, int max)
        {
            return this.r.Next(min, max);
        }
    }
}