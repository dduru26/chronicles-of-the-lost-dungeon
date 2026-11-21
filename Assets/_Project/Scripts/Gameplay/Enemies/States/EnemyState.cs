using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies.States
{
    public abstract class EnemyState : IState<EnemyController>
    {
        public virtual void Enter(EnemyController enemy) { }
        public virtual void Tick(EnemyController enemy, float deltaTime) { }
        public virtual void Exit(EnemyController enemy) { }
    }
}
