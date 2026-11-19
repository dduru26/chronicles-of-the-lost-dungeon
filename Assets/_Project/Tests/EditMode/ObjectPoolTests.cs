using LostDungeon.Core.Patterns;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class ObjectPoolTests
    {
        class Item { public bool Active; }

        ObjectPool<Item> CreatePool(int prewarm = 0, int maxSize = 10) =>
            new ObjectPool<Item>(() => new Item(), i => i.Active = true, i => i.Active = false, null, prewarm, maxSize);

        [Test]
        public void Get_CreatesNewItem_WhenPoolIsEmpty()
        {
            var pool = CreatePool();
            Item item = pool.Get();
            Assert.IsNotNull(item);
            Assert.IsTrue(item.Active);
            Assert.AreEqual(1, pool.CountActive);
            Assert.AreEqual(1, pool.TotalCreated);
        }

        [Test]
        public void Release_ThenGet_ReusesTheSameInstance()
        {
            var pool = CreatePool();
            Item first = pool.Get();
            pool.Release(first);
            Item second = pool.Get();
            Assert.AreSame(first, second);
            Assert.AreEqual(1, pool.TotalCreated);
        }

        [Test]
        public void Release_ReturnsFalse_ForDoubleReleaseOrForeignItem()
        {
            var pool = CreatePool();
            Item item = pool.Get();
            Assert.IsTrue(pool.Release(item));
            Assert.IsFalse(pool.Release(item));
            Assert.IsFalse(pool.Release(new Item()));
        }

        [Test]
        public void Prewarm_FillsInactiveItems_WithoutActivatingThem()
        {
            var pool = CreatePool(prewarm: 5);
            Assert.AreEqual(5, pool.CountInactive);
            Assert.AreEqual(0, pool.CountActive);
            pool.Get();
            Assert.AreEqual(5, pool.TotalCreated);
        }

        [Test]
        public void Release_DestroysOverflow_BeyondMaxSize()
        {
            int destroyed = 0;
            var pool = new ObjectPool<Item>(() => new Item(), null, null, _ => destroyed++, 0, 2);
            var a = pool.Get(); var b = pool.Get(); var c = pool.Get();
            pool.Release(a); pool.Release(b); pool.Release(c);
            Assert.AreEqual(2, pool.CountInactive);
            Assert.AreEqual(1, destroyed);
        }
    }
}
