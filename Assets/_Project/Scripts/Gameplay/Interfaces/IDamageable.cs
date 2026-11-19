using LostDungeon.Core.Combat;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(DamageInfo info);
    }
}
