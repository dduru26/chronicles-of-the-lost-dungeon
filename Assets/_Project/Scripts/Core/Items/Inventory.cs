using System;
using System.Collections.Generic;

namespace LostDungeon.Core.Items
{
    public class Inventory
    {
        readonly Dictionary<string, int> items = new Dictionary<string, int>(StringComparer.Ordinal);

        public event Action<string, int> ItemChanged;

        public IReadOnlyDictionary<string, int> Items => items;
        public int DistinctCount => items.Count;

        public int Count(string id) => id != null && items.TryGetValue(id, out int count) ? count : 0;

        public bool Has(string id, int amount = 1) => Count(id) >= amount;

        public void Add(string id, int amount = 1)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Item id is required.", nameof(id));
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            int total = Count(id) + amount;
            items[id] = total;
            ItemChanged?.Invoke(id, total);
        }

        public bool TryRemove(string id, int amount = 1)
        {
            if (amount <= 0 || !Has(id, amount)) return false;

            int remaining = Count(id) - amount;
            if (remaining == 0) items.Remove(id);
            else items[id] = remaining;

            ItemChanged?.Invoke(id, remaining);
            return true;
        }

        public List<InventoryEntry> ToEntries()
        {
            var entries = new List<InventoryEntry>(items.Count);
            foreach (KeyValuePair<string, int> pair in items) entries.Add(new InventoryEntry(pair.Key, pair.Value));
            return entries;
        }

        public void Load(IEnumerable<InventoryEntry> entries)
        {
            items.Clear();
            if (entries == null) return;
            foreach (InventoryEntry entry in entries)
                if (!string.IsNullOrEmpty(entry.id) && entry.count > 0) items[entry.id] = Count(entry.id) + entry.count;
        }

        public void Clear() => items.Clear();
    }
}
