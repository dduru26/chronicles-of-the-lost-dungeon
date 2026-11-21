using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class ChaseState : EnemyState
    {
        public IState<EnemyController> OnTargetLost;
        public IState<EnemyController> OnInRange;

        public override void Enter(EnemyController enemy) => enemy.PlayClip(enemy.Config.MoveClip);

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            if (!enemy.CanSeeTarget(enemy.Config.SightRange * 1.4f))
            {
                enemy.ChangeState(OnTargetLost);
                return;
            }

            if (enemy.DistanceToTargetX <= enemy.Config.AttackRange)
            {
                enemy.ChangeState(OnInRange);
                return;
            }

            bool moving = enemy.MoveTowardTarget(enemy.Config.ChaseSpeed);
            enemy.PlayClip(moving ? enemy.Config.MoveClip : enemy.Config.IdleClip);
        }
    }
}
