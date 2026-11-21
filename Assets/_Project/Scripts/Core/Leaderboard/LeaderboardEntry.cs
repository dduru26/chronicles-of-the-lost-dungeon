using System;

namespace LostDungeon.Core.Leaderboard
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string player_name;
        public int score;
        public int levels_cleared;
        public float time_seconds;

        public LeaderboardEntry() { }

        public LeaderboardEntry(string playerName, int score, int levelsCleared, float timeSeconds)
        {
            player_name = playerName;
            this.score = score;
            levels_cleared = levelsCleared;
            time_seconds = timeSeconds;
        }

        public static int CompareByRank(LeaderboardEntry a, LeaderboardEntry b)
        {
            int byScore = b.score.CompareTo(a.score);
            if (byScore != 0) return byScore;
            int byLevels = b.levels_cleared.CompareTo(a.levels_cleared);
            if (byLevels != 0) return byLevels;
            return a.time_seconds.CompareTo(b.time_seconds);
        }
    }
}
