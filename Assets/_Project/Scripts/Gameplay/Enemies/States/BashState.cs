using LostDungeon.Core.Patterns;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class BashState : EnemyState
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
            EnemyConfig config = enemy.Config;

            if (!struck && enemy.TimeInState >= config.AttackWindup)
            {
                struck = true;
                enemy.SetHorizontalVelocity(enemy.Facing * config.LungeSpeed);
                enemy.MeleeStrike(config.AttackRange, config.AttackHeight, config.Damage);
            }
            else if (struck)
            {
                enemy.SetHorizontalVelocity(Mathf.MoveTowards(enemy.Body.linearVelocity.x, 0f, 30f * deltaTime));
            }
            else
            {
                enemy.StopMoving();
            }

            if (enemy.TimeInState >= config.AttackWindup + config.AttackRecover) enemy.ChangeState(OnFinished);
        }
    }
}
