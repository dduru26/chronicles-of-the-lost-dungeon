using LostDungeon.Core.Items;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class InventoryTests
    {
        [Test]
        public void Add_AccumulatesCounts_ForTheSameItem()
        {
            var inventory = new Inventory();
            inventory.Add("coin", 3);
            inventory.Add("coin", 2);
            Assert.AreEqual(5, inventory.Count("coin"));
            Assert.IsTrue(inventory.Has("coin", 5));
        }

        [Test]
        public void TryRemove_Fails_WhenNotEnoughItems()
        {
            var inventory = new Inventory();
            inventory.Add("potion", 1);
            Assert.IsFalse(inventory.TryRemove("potion", 2));
            Assert.AreEqual(1, inventory.Count("potion"));
        }

        [Test]
        public void TryRemove_DropsTheEntry_WhenCountReachesZero()
        {
            var inventory = new Inventory();
            inventory.Add("key");
            Assert.IsTrue(inventory.TryRemove("key"));
            Assert.AreEqual(0, inventory.Count("key"));
            Assert.AreEqual(0, inventory.DistinctCount);
        }

        [Test]
        public void ItemChanged_ReportsTheNewTotal()
        {
            var inventory = new Inventory();
            string changedId = null;
            int changedTotal = -1;
            inventory.ItemChanged += (id, total) => { changedId = id; changedTotal = total; };
            inventory.Add("gem", 2);
            Assert.AreEqual("gem", changedId);
            Assert.AreEqual(2, changedTotal);
        }

        [Test]
        public void ToEntries_ThenLoad_RestoresTheSameContents()
        {
            var original = new Inventory();
            original.Add("coin", 12);
            original.Add("potion", 2);
            var restored = new Inventory();
            restored.Load(original.ToEntries());
            Assert.AreEqual(12, restored.Count("coin"));
            Assert.AreEqual(2, restored.Count("potion"));
        }
    }
}
