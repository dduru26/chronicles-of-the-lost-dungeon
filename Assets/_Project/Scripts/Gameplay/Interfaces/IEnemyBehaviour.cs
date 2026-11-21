using LostDungeon.Gameplay.Enemies;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface IEnemyBehaviour
    {
        string BehaviourName { get; }
        EnemyBrain Build(EnemyController enemy);
    }
}
