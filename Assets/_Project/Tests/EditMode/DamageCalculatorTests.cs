using LostDungeon.Core.Combat;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class DamageCalculatorTests
    {
        [Test]
        public void Calculate_ReturnsBaseDamage_WithNoModifiers()
        {
            Assert.AreEqual(3, DamageCalculator.Calculate(3));
        }

        [Test]
        public void Calculate_AppliesCriticalMultiplier_AndRoundsAwayFromZero()
        {
            Assert.AreEqual(3, DamageCalculator.Calculate(2, 1f, true));
            Assert.AreEqual(5, DamageCalculator.Calculate(3, 1f, true));
        }

        [Test]
        public void Calculate_SubtractsArmor_ButNeverDropsBelowOne()
        {
            Assert.AreEqual(2, DamageCalculator.Calculate(5, 1f, false, 3));
            Assert.AreEqual(1, DamageCalculator.Calculate(2, 1f, false, 10));
        }

        [Test]
        public void Calculate_ReturnsZero_ForNonPositiveBaseOrMultiplier()
        {
            Assert.AreEqual(0, DamageCalculator.Calculate(0));
            Assert.AreEqual(0, DamageCalculator.Calculate(4, 0f));
        }

        [Test]
        public void IsBlockedByShield_BlocksFrontalHits_AndAllowsHitsFromBehind()
        {
            Assert.IsTrue(DamageCalculator.IsBlockedByShield(1, 10f, 12f));
            Assert.IsFalse(DamageCalculator.IsBlockedByShield(1, 10f, 8f));
            Assert.IsTrue(DamageCalculator.IsBlockedByShield(-1, 10f, 8f));
        }
    }
}
