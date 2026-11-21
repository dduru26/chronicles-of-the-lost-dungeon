using System;

namespace LostDungeon.Core.Leaderboard
{
    public static class ScoreCalculator
    {
        public const int PointsPerLevel = 1000;
        public const int PointsPerGold = 5;
        public const int PointsPerKill = 50;
        public const int DeathPenalty = 250;
        public const float ParSecondsPerLevel = 120f;
        public const int MaxTimeBonusPerLevel = 500;

        public static int Calculate(int levelsCleared, int gold, int kills, int deaths, float totalSeconds)
        {
            levelsCleared = Math.Max(0, levelsCleared);
            float par = levelsCleared * ParSecondsPerLevel;
            float saved = Math.Max(0f, par - Math.Max(0f, totalSeconds));
            int timeBonus = par > 0f ? (int)Math.Round(saved / par * MaxTimeBonusPerLevel * levelsCleared) : 0;

            int total = levelsCleared * PointsPerLevel
                        + Math.Max(0, gold) * PointsPerGold
                        + Math.Max(0, kills) * PointsPerKill
                        + timeBonus
                        - Math.Max(0, deaths) * DeathPenalty;

            return Math.Max(0, total);
        }
    }
}
