using LostDungeon.Core.Patterns;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Pooling
{
    public class PrefabPool : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField, Min(0)] int prewarm = 8;
        [SerializeField, Min(1)] int maxSize = 64;

        ObjectPool<GameObject> pool;

        public GameObject Prefab => prefab;
        public int CountActive => pool?.CountActive ?? 0;
        public int CountAll => pool?.CountAll ?? 0;

        public void Initialize(GameObject sourcePrefab, int prewarmCount, int maxCount)
        {
            prefab = sourcePrefab;
            prewarm = prewarmCount;
            maxSize = maxCount;
            Build();
        }

        void Awake()
        {
            if (prefab != null && pool == null) Build();
        }

        void Build()
        {
            pool?.Clear();
            pool = new ObjectPool<GameObject>(Create, null, OnRelease, OnDestroyItem, prewarm, maxSize);
        }

        GameObject Create()
        {
            GameObject instance = Instantiate(prefab, transform);
            instance.AddComponent<PooledObject>().Owner = this;
            instance.SetActive(false);
            return instance;
        }

        void OnRelease(GameObject instance)
        {
            foreach (IPoolable poolable in instance.GetComponents<IPoolable>()) poolable.OnDespawned();
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
        }

        void OnDestroyItem(GameObject instance)
        {
            if (instance != null) Destroy(instance);
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            if (pool == null) Build();
            GameObject instance = pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            foreach (IPoolable poolable in instance.GetComponents<IPoolable>()) poolable.OnSpawned();
            return instance;
        }

        public T Spawn<T>(Vector3 position) where T : Component => Spawn(position, Quaternion.identity).GetComponent<T>();

        public void Despawn(GameObject instance)
        {
            if (pool == null || !pool.Release(instance)) Destroy(instance);
        }

        void OnDestroy()
        {
            pool?.Clear();
        }
    }
}
