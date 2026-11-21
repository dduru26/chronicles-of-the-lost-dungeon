using System;
using System.Collections.Generic;

namespace LostDungeon.Core.Algorithms
{
    public static class WeightedRandom
    {
        public static int PickIndex(IReadOnlyList<float> weights, double roll)
        {
            if (weights == null || weights.Count == 0) return -1;

            double total = 0;
            for (int i = 0; i < weights.Count; i++) total += Math.Max(0f, weights[i]);
            if (total <= 0) return -1;

            double target = Math.Min(Math.Max(roll, 0d), 0.9999999d) * total;
            double cumulative = 0;
            int lastValid = -1;

            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] <= 0f) continue;
                lastValid = i;
                cumulative += weights[i];
                if (target < cumulative) return i;
            }

            return lastValid;
        }

        public static int PickIndex(IReadOnlyList<float> weights, Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            return PickIndex(weights, random.NextDouble());
        }
    }
}
