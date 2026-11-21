using System;
using System.Collections.Generic;

namespace LostDungeon.Core.Algorithms
{
    public static class NearestSearch
    {
        public static int FindNearest(IReadOnlyList<(float, float)> points, float originX, float originY, float maxDistance)
        {
            if (points == null) return -1;

            int bestIndex = -1;
            float bestSquared = maxDistance * maxDistance;

            for (int i = 0; i < points.Count; i++)
            {
                float dx = points[i].Item1 - originX;
                float dy = points[i].Item2 - originY;
                float squared = dx * dx + dy * dy;
                if (squared > bestSquared) continue;
                if (squared == bestSquared && bestIndex >= 0) continue;
                bestSquared = squared;
                bestIndex = i;
            }

            return bestIndex;
        }

        public static int FindNearest<T>(IReadOnlyList<T> items, Func<T, (float, float)> position, float originX, float originY, float maxDistance)
        {
            if (items == null || position == null) return -1;
            var points = new List<(float, float)>(items.Count);
            foreach (T item in items) points.Add(position(item));
            return FindNearest(points, originX, originY, maxDistance);
        }
    }
}
