using System.Collections.Generic;
using LostDungeon.Gameplay.Architecture;
using UnityEngine;

namespace LostDungeon.Gameplay.Pooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField, Min(0)] int defaultPrewarm = 4;
        [SerializeField, Min(1)] int defaultMaxSize = 64;

        readonly Dictionary<GameObject, PrefabPool> pools = new Dictionary<GameObject, PrefabPool>();

        public IReadOnlyDictionary<GameObject, PrefabPool> Pools => pools;

        public static PoolManager GetOrCreate()
        {
            if (HasInstance) return Instance;
            return new GameObject(nameof(PoolManager)).AddComponent<PoolManager>();
        }

        public PrefabPool GetPool(GameObject prefab)
        {
            if (pools.TryGetValue(prefab, out PrefabPool pool) && pool != null) return pool;

            var holder = new GameObject($"Pool_{prefab.name}");
            holder.transform.SetParent(transform, false);
            pool = holder.AddComponent<PrefabPool>();
            pool.Initialize(prefab, defaultPrewarm, defaultMaxSize);
            pools[prefab] = pool;
            return pool;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) => GetPool(prefab).Spawn(position, rotation);

        public void ClearAll()
        {
            foreach (PrefabPool pool in pools.Values)
                if (pool != null) Destroy(pool.gameObject);
            pools.Clear();
        }
    }
}
