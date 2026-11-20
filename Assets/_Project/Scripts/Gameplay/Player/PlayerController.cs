using System;
using LostDungeon.Core.Combat;
using LostDungeon.Core.Events;
using LostDungeon.Gameplay.Combat;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerMotor), typeof(PlayerInputReader), typeof(Health))]
    public class PlayerController : MonoBehaviour, IAbilityUser
    {
        [SerializeField] PlayerAnimationDriver animationDriver;
        [SerializeField, Min(1f)] float flashRate = 20f;

        PlayerMotor motor;
        PlayerInputReader input;
        Health health;

        public event Action Died;
        public event Action Respawned;

        public Transform Transform => transform;
        public Rigidbody2D Body => motor.Body;
        public int Facing => motor.Facing;
        public bool IsGrounded => motor.IsGrounded;
        public PlayerMotor Motor => motor;
        public PlayerInputReader InputReader => input;
        public Health Health => health;
        public PlayerAnimationDriver Animation => animationDriver;
        public bool IsDead => !health.IsAlive;
        public bool ControlsEnabled { get; set; } = true;

        void Awake()
        {
            motor = GetComponent<PlayerMotor>();
            input = GetComponent<PlayerInputReader>();
            health = GetComponent<Health>();
            if (health == null) health = gameObject.AddComponent<Health>();
            if (animationDriver == null) animationDriver = GetComponentInChildren<PlayerAnimationDriver>();
        }

        void OnEnable()
        {
            health.Changed += HandleHealthChanged;
            health.Damaged += HandleDamaged;
            health.Died += HandleDied;
        }

        void OnDisable()
        {
            health.Changed -= HandleHealthChanged;
            health.Damaged -= HandleDamaged;
            health.Died -= HandleDied;
        }

        void Start()
        {
            health.InvulnerabilityDuration = motor.Config.InvulnerabilityAfterHit;
            health.Initialize(motor.Config.MaxHealth);
        }

        public void SetInvulnerable(float duration) => health.SetInvulnerable(duration);
        public void LockVelocity(Vector2 velocity, float duration, bool ignoreGravity) => motor.LockVelocity(velocity, duration, ignoreGravity);

        public void Respawn(Vector3 position)
        {
            transform.position = position;
            motor.Body.linearVelocity = Vector2.zero;
            health.Revive();
            animationDriver.ResetActions();
            ControlsEnabled = true;
            Respawned?.Invoke();
        }

        void HandleHealthChanged(int current, int max) => GameEvents.RaiseHealthChanged(current, max);

        void HandleDamaged(DamageInfo info)
        {
            if (!health.IsAlive) return;
            int direction = info.DirectionFrom(transform.position.x);
            Vector2 knockback = motor.Config.HurtKnockback;
            motor.ApplyKnockback(new Vector2(direction * knockback.x, knockback.y), motor.Config.HurtControlLock);
            animationDriver.PlayAction("Hurt");
        }

        void HandleDied()
        {
            ControlsEnabled = false;
            motor.Halt();
            animationDriver.PlayAction("Death", true);
            GameEvents.RaisePlayerDied();
            Died?.Invoke();
        }

        void Update()
        {
            bool flashing = health.IsAlive && health.IsInvulnerable && !motor.IsDashing;
            animationDriver.SetFlash(!flashing || Mathf.FloorToInt(Time.time * flashRate) % 2 == 0);

            if (!ControlsEnabled)
            {
                motor.SetMoveInput(0f);
                motor.SetJumpHeld(false);
                return;
            }

            motor.SetMoveInput(input.Move);
            motor.SetJumpHeld(input.JumpHeld);

            if (input.Pressed(PlayerButton.Jump))
            {
                bool dropped = input.DownHeld && motor.TryDropThroughPlatform();
                if (!dropped) motor.RequestJump();
            }
        }
    }
}
