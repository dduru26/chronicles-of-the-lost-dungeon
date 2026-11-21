using System.Collections.Generic;
using LostDungeon.Core.Algorithms;
using LostDungeon.Core.Leaderboard;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class LeaderboardTests
    {
        [Test]
        public void MergeSort_SortsIntegersAscending()
        {
            var values = new List<int> { 5, 1, 4, 1, 9, 2, 8 };
            MergeSort.Sort(values, (a, b) => a.CompareTo(b));
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 4, 5, 8, 9 }, values);
        }

        [Test]
        public void MergeSort_IsStable_ForEqualKeys()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry("first", 100, 2, 50f),
                new LeaderboardEntry("second", 100, 2, 50f),
                new LeaderboardEntry("third", 300, 3, 90f)
            };
            MergeSort.Sort(entries, LeaderboardEntry.CompareByRank);
            Assert.AreEqual("third", entries[0].player_name);
            Assert.AreEqual("first", entries[1].player_name);
            Assert.AreEqual("second", entries[2].player_name);
        }

        [Test]
        public void CompareByRank_BreaksScoreTies_ByLevelsThenFasterTime()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry("slow", 500, 3, 400f),
                new LeaderboardEntry("fewer levels", 500, 2, 100f),
                new LeaderboardEntry("fast", 500, 3, 200f)
            };
            MergeSort.Sort(entries, LeaderboardEntry.CompareByRank);
            Assert.AreEqual("fast", entries[0].player_name);
            Assert.AreEqual("slow", entries[1].player_name);
            Assert.AreEqual("fewer levels", entries[2].player_name);
        }

        [Test]
        public void ScoreCalculator_RewardsProgressGoldKillsAndSpeed()
        {
            int score = ScoreCalculator.Calculate(levelsCleared: 2, gold: 10, kills: 4, deaths: 0, totalSeconds: 120f);
            int expected = 2 * 1000 + 10 * 5 + 4 * 50 + 500;
            Assert.AreEqual(expected, score);
        }

        [Test]
        public void ScoreCalculator_AppliesDeathPenalty_ButNeverGoesNegative()
        {
            Assert.AreEqual(1000 - 250, ScoreCalculator.Calculate(1, 0, 0, 1, 500f));
            Assert.AreEqual(0, ScoreCalculator.Calculate(0, 0, 0, 10, 0f));
        }
    }
}
