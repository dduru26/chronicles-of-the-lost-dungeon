using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class PatrolState : EnemyState
    {
        public IState<EnemyController> OnTargetSpotted;

        public override void Enter(EnemyController enemy) => enemy.PlayClip(enemy.Config.MoveClip);

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            enemy.Patrol(enemy.Config.PatrolSpeed);
            if (OnTargetSpotted != null && enemy.CanSeeTarget(enemy.Config.SightRange)) enemy.ChangeState(OnTargetSpotted);
        }
    }
}
