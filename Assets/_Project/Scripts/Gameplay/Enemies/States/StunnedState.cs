using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class StunnedState : EnemyState
    {
        public IState<EnemyController> OnFinished;

        public override void Enter(EnemyController enemy)
        {
            enemy.StopMoving();
            enemy.PlayClip(enemy.Config.HurtClip, true);
        }

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            enemy.StopMoving();
            if (enemy.TimeInState >= enemy.Config.StunDuration) enemy.ChangeState(OnFinished);
        }
    }
}
