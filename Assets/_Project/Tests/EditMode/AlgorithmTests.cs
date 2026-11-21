using LostDungeon.Core.Algorithms;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class AlgorithmTests
    {
        [Test]
        public void WeightedRandom_MapsRollsToCumulativeWeightRanges()
        {
            var weights = new[] { 50f, 30f, 20f };
            Assert.AreEqual(0, WeightedRandom.PickIndex(weights, 0.0));
            Assert.AreEqual(0, WeightedRandom.PickIndex(weights, 0.49));
            Assert.AreEqual(1, WeightedRandom.PickIndex(weights, 0.5));
            Assert.AreEqual(2, WeightedRandom.PickIndex(weights, 0.85));
            Assert.AreEqual(2, WeightedRandom.PickIndex(weights, 1.0));
        }

        [Test]
        public void WeightedRandom_SkipsZeroWeights_AndHandlesEmptyTables()
        {
            Assert.AreEqual(1, WeightedRandom.PickIndex(new[] { 0f, 10f, 0f }, 0.99));
            Assert.AreEqual(-1, WeightedRandom.PickIndex(new float[0], 0.5));
            Assert.AreEqual(-1, WeightedRandom.PickIndex(new[] { 0f, 0f }, 0.5));
        }

        [Test]
        public void NearestSearch_ReturnsClosestPointWithinRange()
        {
            var points = new[] { (5f, 0f), (1f, 1f), (2f, 0f) };
            Assert.AreEqual(1, NearestSearch.FindNearest(points, 0f, 0f, 3f));
        }

        [Test]
        public void NearestSearch_ReturnsMinusOne_WhenEverythingIsOutOfRange()
        {
            var points = new[] { (5f, 0f), (0f, 9f) };
            Assert.AreEqual(-1, NearestSearch.FindNearest(points, 0f, 0f, 2f));
        }
    }
}
