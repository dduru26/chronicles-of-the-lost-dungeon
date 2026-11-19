using System;

namespace LostDungeon.Core.Patterns
{
    public class StateMachine<TOwner>
    {
        readonly TOwner owner;

        public IState<TOwner> Current { get; private set; }
        public IState<TOwner> Previous { get; private set; }
        public float TimeInState { get; private set; }

        public event Action<IState<TOwner>, IState<TOwner>> StateChanged;

        public StateMachine(TOwner owner)
        {
            this.owner = owner;
        }

        public void ChangeState(IState<TOwner> next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (ReferenceEquals(next, Current)) return;

            Current?.Exit(owner);
            Previous = Current;
            Current = next;
            TimeInState = 0f;
            Current.Enter(owner);
            StateChanged?.Invoke(Previous, Current);
        }

        public void Tick(float deltaTime)
        {
            if (Current == null) return;
            TimeInState += deltaTime;
            Current.Tick(owner, deltaTime);
        }

        public bool IsIn(IState<TOwner> state) => ReferenceEquals(Current, state);
    }
}
