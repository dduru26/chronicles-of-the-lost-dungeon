using System;
using System.Collections.Generic;
using LostDungeon.Gameplay.Pooling;
using UnityEngine;

namespace LostDungeon.Gameplay.Items
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Items/Pickup Catalog", fileName = "PickupCatalog")]
    public class PickupCatalog : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string itemId;
            public Pickup prefab;
        }

        [SerializeField] List<Entry> entries = new List<Entry>();
        [SerializeField, Min(1)] int maxCoinsPerDrop = 12;
        [SerializeField] Vector2 launchSpeed = new Vector2(3.5f, 9f);

        public Pickup Find(string itemId)
        {
            foreach (Entry entry in entries)
                if (entry.itemId == itemId) return entry.prefab;
            return null;
        }

        public void Spawn(string itemId, int amount, Vector2 position)
        {
            Pickup prefab = Find(itemId);
            if (prefab == null || amount <= 0) return;

            if (itemId == ItemIds.Gold)
            {
                int coins = Mathf.Min(amount, maxCoinsPerDrop);
                int perCoin = amount / coins;
                int remainder = amount % coins;
                for (int i = 0; i < coins; i++) SpawnOne(prefab, itemId, perCoin + (i < remainder ? 1 : 0), position);
                return;
            }

            SpawnOne(prefab, itemId, amount, position);
        }

        void SpawnOne(Pickup prefab, string itemId, int amount, Vector2 position)
        {
            GameObject instance = PoolManager.GetOrCreate().Spawn(prefab.gameObject, position, Quaternion.identity);
            var pickup = instance.GetComponent<Pickup>();
            pickup.Configure(itemId, amount);
            pickup.Launch(new Vector2(UnityEngine.Random.Range(-launchSpeed.x, launchSpeed.x), UnityEngine.Random.Range(launchSpeed.y * 0.6f, launchSpeed.y)));
        }
    }
}
