using System.Collections.Generic;
using LostDungeon.Core.Patterns;
using NUnit.Framework;

namespace LostDungeon.Tests
{
    public class StateMachineTests
    {
        class Owner { public readonly List<string> Log = new List<string>(); public float Ticked; }

        class LoggingState : IState<Owner>
        {
            readonly string name;
            public LoggingState(string name) { this.name = name; }
            public void Enter(Owner owner) => owner.Log.Add("enter " + name);
            public void Tick(Owner owner, float deltaTime) => owner.Ticked += deltaTime;
            public void Exit(Owner owner) => owner.Log.Add("exit " + name);
        }

        [Test]
        public void ChangeState_ExitsOldState_BeforeEnteringNewState()
        {
            var owner = new Owner();
            var machine = new StateMachine<Owner>(owner);
            var patrol = new LoggingState("patrol");
            var chase = new LoggingState("chase");
            machine.ChangeState(patrol);
            machine.ChangeState(chase);
            CollectionAssert.AreEqual(new[] { "enter patrol", "exit patrol", "enter chase" }, owner.Log);
            Assert.AreSame(patrol, machine.Previous);
            Assert.IsTrue(machine.IsIn(chase));
        }

        [Test]
        public void ChangeState_RaisesStateChangedEvent_WithPreviousAndCurrent()
        {
            var machine = new StateMachine<Owner>(new Owner());
            var idle = new LoggingState("idle");
            var attack = new LoggingState("attack");
            IState<Owner> from = null, to = null;
            machine.ChangeState(idle);
            machine.StateChanged += (p, c) => { from = p; to = c; };
            machine.ChangeState(attack);
            Assert.AreSame(idle, from);
            Assert.AreSame(attack, to);
        }

        [Test]
        public void ChangeState_ToCurrentState_DoesNothing()
        {
            var owner = new Owner();
            var machine = new StateMachine<Owner>(owner);
            var idle = new LoggingState("idle");
            machine.ChangeState(idle);
            machine.ChangeState(idle);
            Assert.AreEqual(1, owner.Log.Count);
        }

        [Test]
        public void Tick_ForwardsDeltaTime_AndTracksTimeInState()
        {
            var owner = new Owner();
            var machine = new StateMachine<Owner>(owner);
            machine.ChangeState(new LoggingState("idle"));
            machine.Tick(0.25f);
            machine.Tick(0.5f);
            Assert.AreEqual(0.75f, owner.Ticked, 0.0001f);
            Assert.AreEqual(0.75f, machine.TimeInState, 0.0001f);
            machine.ChangeState(new LoggingState("chase"));
            Assert.AreEqual(0f, machine.TimeInState);
        }
    }
}
