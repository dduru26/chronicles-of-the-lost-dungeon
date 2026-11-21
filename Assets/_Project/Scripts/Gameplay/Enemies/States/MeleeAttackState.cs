using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class MeleeAttackState : EnemyState
    {
        public IState<EnemyController> OnFinished;
        bool struck;

        public override void Enter(EnemyController enemy)
        {
            struck = false;
            enemy.StopMoving();
            enemy.FaceTarget();
            enemy.PlayClip(enemy.Config.AttackClip, true);
        }

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            enemy.StopMoving();
            EnemyConfig config = enemy.Config;

            if (!struck && enemy.TimeInState >= config.AttackWindup)
            {
                struck = true;
                enemy.MeleeStrike(config.AttackRange, config.AttackHeight, config.Damage);
            }

            if (enemy.TimeInState >= config.AttackWindup + config.AttackRecover) enemy.ChangeState(OnFinished);
        }
    }
}
