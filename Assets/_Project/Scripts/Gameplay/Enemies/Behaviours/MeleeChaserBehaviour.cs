using LostDungeon.Gameplay.Enemies.States;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.Behaviours
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Enemies/Behaviours/Melee Chaser", fileName = "MeleeChaserBehaviour")]
    public class MeleeChaserBehaviour : EnemyBehaviour
    {
        public override EnemyBrain Build(EnemyController enemy)
        {
            var patrol = new PatrolState();
            var chase = new ChaseState();
            var attack = new MeleeAttackState();

            patrol.OnTargetSpotted = chase;
            chase.OnTargetLost = patrol;
            chase.OnInRange = attack;
            attack.OnFinished = chase;

            return new EnemyBrain(patrol, chase);
        }
    }
}
