using System;

namespace LostDungeon.Core.Items
{
    [Serializable]
    public struct InventoryEntry
    {
        public string id;
        public int count;

        public InventoryEntry(string id, int count)
        {
            this.id = id;
            this.count = count;
        }
    }
}
