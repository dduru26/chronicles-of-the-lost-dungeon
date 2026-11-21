using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class RangedAttackState : EnemyState
    {
        public IState<EnemyController> OnTargetLost;
        public IState<EnemyController> OnTooClose;
        float fireTimer;
        float shotTimer;

        public override void Enter(EnemyController enemy)
        {
            enemy.StopMoving();
            fireTimer = enemy.Config.FireInterval * 0.5f;
            shotTimer = -1f;
            enemy.PlayClip(enemy.Config.IdleClip);
        }

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            EnemyConfig config = enemy.Config;
            enemy.StopMoving();
            enemy.FaceTarget();

            if (!enemy.CanSeeTarget(config.SightRange * 1.3f))
            {
                enemy.ChangeState(OnTargetLost);
                return;
            }

            if (shotTimer < 0f && enemy.DistanceToTargetX < config.KeepDistance && enemy.CanMoveAwayFromTarget())
            {
                enemy.ChangeState(OnTooClose);
                return;
            }

            if (shotTimer >= 0f)
            {
                shotTimer -= deltaTime;
                if (shotTimer < 0f)
                {
                    enemy.FireProjectileAtTarget();
                    enemy.PlayClip(config.IdleClip);
                }
                return;
            }

            fireTimer -= deltaTime;
            if (fireTimer <= 0f)
            {
                fireTimer = config.FireInterval;
                shotTimer = config.AttackWindup;
                enemy.PlayClip(config.AttackClip, true);
            }
        }
    }
}
