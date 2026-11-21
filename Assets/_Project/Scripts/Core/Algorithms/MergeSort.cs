using System;
using System.Collections.Generic;

namespace LostDungeon.Core.Algorithms
{
    public static class MergeSort
    {
        public static void Sort<T>(IList<T> items, Comparison<T> comparison)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            if (items.Count < 2) return;

            var buffer = new T[items.Count];
            SortRange(items, buffer, 0, items.Count - 1, comparison);
        }

        static void SortRange<T>(IList<T> items, T[] buffer, int left, int right, Comparison<T> comparison)
        {
            if (left >= right) return;

            int middle = left + (right - left) / 2;
            SortRange(items, buffer, left, middle, comparison);
            SortRange(items, buffer, middle + 1, right, comparison);
            if (comparison(items[middle], items[middle + 1]) <= 0) return;
            Merge(items, buffer, left, middle, right, comparison);
        }

        static void Merge<T>(IList<T> items, T[] buffer, int left, int middle, int right, Comparison<T> comparison)
        {
            int i = left;
            int j = middle + 1;
            int k = left;

            while (i <= middle && j <= right)
                buffer[k++] = comparison(items[i], items[j]) <= 0 ? items[i++] : items[j++];

            while (i <= middle) buffer[k++] = items[i++];
            while (j <= right) buffer[k++] = items[j++];

            for (int index = left; index <= right; index++) items[index] = buffer[index];
        }
    }
}
