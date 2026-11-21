using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.States
{
    public class DeadState : EnemyState
    {
        const float LingerAfterAnimation = 0.6f;
        float despawnAt;

        public override void Enter(EnemyController enemy)
        {
            enemy.DisableBody();
            enemy.PlayClip(enemy.Config.DeathClip, true);
            despawnAt = enemy.ClipDuration(enemy.Config.DeathClip) + LingerAfterAnimation;
        }

        public override void Tick(EnemyController enemy, float deltaTime)
        {
            if (enemy.TimeInState >= Mathf.Max(0.5f, despawnAt)) enemy.Despawn();
        }
    }
}
