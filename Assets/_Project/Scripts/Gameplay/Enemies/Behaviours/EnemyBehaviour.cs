using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.Behaviours
{
    public abstract class EnemyBehaviour : ScriptableObject, IEnemyBehaviour
    {
        public virtual string BehaviourName => name;
        public abstract EnemyBrain Build(EnemyController enemy);
    }
}
