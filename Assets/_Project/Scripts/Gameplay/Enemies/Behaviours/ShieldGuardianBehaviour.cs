using LostDungeon.Core.Combat;
using LostDungeon.Gameplay.Enemies.States;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.Behaviours
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Enemies/Behaviours/Shield Guardian", fileName = "ShieldGuardianBehaviour")]
    public class ShieldGuardianBehaviour : EnemyBehaviour
    {
        [SerializeField] DamageType stunnedBy = DamageType.Magic;

        public override EnemyBrain Build(EnemyController enemy)
        {
            var patrol = new PatrolState();
            var guard = new GuardState();
            var bash = new BashState();
            var stunned = new StunnedState();

            patrol.OnTargetSpotted = guard;
            guard.OnTargetLost = patrol;
            guard.OnInRange = bash;
            bash.OnFinished = guard;
            stunned.OnFinished = guard;

            enemy.Health.BlockFilter = info =>
                info.Type == DamageType.Melee
                && (enemy.IsInState(guard) || enemy.IsInState(patrol))
                && DamageCalculator.IsBlockedByShield(enemy.Facing, enemy.transform.position.x, info.SourceX);

            DamageType stunType = stunnedBy;
            return new EnemyBrain(patrol, guard, info => info.Type == stunType && !enemy.IsInState(stunned) ? stunned : null);
        }
    }
}
