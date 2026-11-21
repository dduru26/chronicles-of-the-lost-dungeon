using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class RetreatState : EnemyState
    {
        public IState<EnemyController> OnFinished;
        const float MaxDuration = 0.8f;

        public override void Enter(EnemyController enemy) => enemy.PlayClip(enemy.Config.MoveClip);

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            bool moved = enemy.MoveAwayFromTarget(enemy.Config.ChaseSpeed * 1.2f);
            bool farEnough = enemy.DistanceToTargetX >= enemy.Config.KeepDistance;
            if (!moved || farEnough || enemy.TimeInState >= MaxDuration) enemy.ChangeState(OnFinished);
        }
    }
}
