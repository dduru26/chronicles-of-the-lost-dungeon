using System;
using System.Collections.Generic;

namespace LostDungeon.Core.Patterns
{
    public class ObjectPool<T> where T : class
    {
        readonly Stack<T> inactive = new Stack<T>();
        readonly HashSet<T> active = new HashSet<T>();
        readonly Func<T> create;
        readonly Action<T> onGet;
        readonly Action<T> onRelease;
        readonly Action<T> onDestroy;

        public int MaxSize { get; }
        public int CountActive => active.Count;
        public int CountInactive => inactive.Count;
        public int CountAll => active.Count + inactive.Count;
        public int TotalCreated { get; private set; }

        public ObjectPool(Func<T> create, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, int prewarm = 0, int maxSize = 256)
        {
            if (create == null) throw new ArgumentNullException(nameof(create));
            if (maxSize < 1) throw new ArgumentOutOfRangeException(nameof(maxSize));

            this.create = create;
            this.onGet = onGet;
            this.onRelease = onRelease;
            this.onDestroy = onDestroy;
            MaxSize = maxSize;

            for (int i = 0; i < Math.Min(prewarm, maxSize); i++)
            {
                T item = CreateItem();
                onRelease?.Invoke(item);
                inactive.Push(item);
            }
        }

        public T Get()
        {
            T item = inactive.Count > 0 ? inactive.Pop() : CreateItem();
            active.Add(item);
            onGet?.Invoke(item);
            return item;
        }

        public bool Release(T item)
        {
            if (item == null || !active.Remove(item)) return false;

            onRelease?.Invoke(item);
            if (inactive.Count < MaxSize) inactive.Push(item);
            else onDestroy?.Invoke(item);
            return true;
        }

        public void ReleaseAll()
        {
            foreach (T item in new List<T>(active)) Release(item);
        }

        public void Clear()
        {
            foreach (T item in active) onDestroy?.Invoke(item);
            foreach (T item in inactive) onDestroy?.Invoke(item);
            active.Clear();
            inactive.Clear();
        }

        T CreateItem()
        {
            TotalCreated++;
            return create();
        }
    }
}
