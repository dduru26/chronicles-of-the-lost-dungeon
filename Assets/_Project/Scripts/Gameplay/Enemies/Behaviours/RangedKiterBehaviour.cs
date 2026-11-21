using LostDungeon.Gameplay.Enemies.States;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies.Behaviours
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Enemies/Behaviours/Ranged Kiter", fileName = "RangedKiterBehaviour")]
    public class RangedKiterBehaviour : EnemyBehaviour
    {
        public override EnemyBrain Build(EnemyController enemy)
        {
            var patrol = new PatrolState();
            var aim = new RangedAttackState();
            var retreat = new RetreatState();

            patrol.OnTargetSpotted = aim;
            aim.OnTargetLost = patrol;
            aim.OnTooClose = retreat;
            retreat.OnFinished = aim;

            return new EnemyBrain(patrol, aim);
        }
    }
}
