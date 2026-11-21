using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class GuardState : EnemyState
    {
        public IState<EnemyController> OnTargetLost;
        public IState<EnemyController> OnInRange;
        const float MinimumGuardTime = 0.4f;

        public override void Enter(EnemyController enemy) => enemy.PlayClip(enemy.Config.BlockClip);

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            if (!enemy.CanSeeTarget(enemy.Config.SightRange * 1.4f))
            {
                enemy.ChangeState(OnTargetLost);
                return;
            }

            if (enemy.DistanceToTargetX <= enemy.Config.AttackRange && enemy.TimeInState >= MinimumGuardTime)
            {
                enemy.ChangeState(OnInRange);
                return;
            }

            bool moving = enemy.DistanceToTargetX > enemy.Config.AttackRange * 0.8f && enemy.MoveTowardTarget(enemy.Config.ChaseSpeed);
            if (!moving) enemy.StopMoving();
            enemy.PlayClip(moving ? enemy.Config.MoveClip : enemy.Config.BlockClip);
        }
    }
}
