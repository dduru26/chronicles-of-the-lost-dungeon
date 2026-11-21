using System;
using System.Collections.Generic;
using LostDungeon.Core.Algorithms;
using UnityEngine;

namespace LostDungeon.Gameplay.Items
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Items/Loot Table", fileName = "LootTable")]
    public class LootTable : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string itemId = ItemIds.Gold;
            [Min(0f)] public float weight = 1f;
            [Min(1)] public int minAmount = 1;
            [Min(1)] public int maxAmount = 1;
        }

        [SerializeField] List<Entry> entries = new List<Entry>();
        [SerializeField, Min(1)] int rolls = 1;

        readonly List<float> weights = new List<float>();

        public int Rolls => rolls;
        public IReadOnlyList<Entry> Entries => entries;

        public (string itemId, int amount) Roll(System.Random random)
        {
            weights.Clear();
            foreach (Entry entry in entries) weights.Add(entry.weight);

            int index = WeightedRandom.PickIndex(weights, random);
            if (index < 0) return (null, 0);

            Entry picked = entries[index];
            int amount = random.Next(picked.minAmount, Mathf.Max(picked.minAmount, picked.maxAmount) + 1);
            return (picked.itemId, amount);
        }
    }
}
