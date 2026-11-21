using System;
using LostDungeon.Core.Combat;
using LostDungeon.Core.Patterns;

namespace LostDungeon.Gameplay.Enemies
{
    public sealed class EnemyBrain
    {
        public IState<EnemyController> InitialState { get; }
        public IState<EnemyController> RecoverState { get; }
        public Func<DamageInfo, IState<EnemyController>> DamageReaction { get; }

        public EnemyBrain(IState<EnemyController> initialState, IState<EnemyController> recoverState, Func<DamageInfo, IState<EnemyController>> damageReaction = null)
        {
            InitialState = initialState ?? throw new ArgumentNullException(nameof(initialState));
            RecoverState = recoverState ?? initialState;
            DamageReaction = damageReaction;
        }
    }
}
