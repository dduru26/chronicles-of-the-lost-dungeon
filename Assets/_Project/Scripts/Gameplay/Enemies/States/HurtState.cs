using LostDungeon.Core.Combat;
using LostDungeon.Core.Patterns;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class HurtState : EnemyState
    {
        IState<EnemyController> returnState;
        DamageInfo lastHit;

        public void Prepare(IState<EnemyController> recoverTo, DamageInfo hit)
        {
            returnState = recoverTo;
            lastHit = hit;
        }

        public override void Enter(EnemyController enemy) => Apply(enemy);

        public void Retrigger(EnemyController enemy, DamageInfo hit)
        {
            lastHit = hit;
            enemy.ResetStateTimer();
            Apply(enemy);
        }

        void Apply(EnemyController enemy)
        {
            enemy.PlayClip(enemy.Config.HurtClip, true);
            int direction = lastHit.DirectionFrom(enemy.transform.position.x);
            Vector2 knockback = enemy.Config.Knockback;
            enemy.SetVelocity(new Vector2(direction * knockback.x, enemy.Config.IsFlying ? 0f : knockback.y));
        }

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            enemy.SetHorizontalVelocity(Mathf.MoveTowards(enemy.Body.linearVelocity.x, 0f, 40f * deltaTime));
            if (enemy.TimeInState >= enemy.Config.HurtDuration) enemy.ChangeState(returnState);
        }
    }
}
