using System;

namespace LostDungeon.Core.Combat
{
    public static class DamageCalculator
    {
        public const float CriticalMultiplier = 1.5f;

        public static int Calculate(int baseDamage, float multiplier = 1f, bool isCritical = false, int armor = 0)
        {
            if (baseDamage <= 0 || multiplier <= 0f) return 0;

            double raw = baseDamage * multiplier * (isCritical ? CriticalMultiplier : 1f);
            int rounded = (int)Math.Round(raw, MidpointRounding.AwayFromZero);
            return Math.Max(1, rounded - Math.Max(0, armor));
        }

        public static bool RollCritical(double chance, Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            return random.NextDouble() < chance;
        }

        public static bool IsBlockedByShield(int defenderFacing, float defenderX, float attackerX)
        {
            int attackerSide = attackerX >= defenderX ? 1 : -1;
            return attackerSide == Math.Sign(defenderFacing);
        }
    }
}
