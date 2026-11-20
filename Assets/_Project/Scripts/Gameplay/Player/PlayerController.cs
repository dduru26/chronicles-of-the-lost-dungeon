using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerMotor), typeof(PlayerInputReader))]
    public class PlayerController : MonoBehaviour, IAbilityUser
    {
        PlayerMotor motor;
        PlayerInputReader input;
        float invulnerableTimer;

        public Transform Transform => transform;
        public Rigidbody2D Body => motor.Body;
        public int Facing => motor.Facing;
        public PlayerMotor Motor => motor;
        public PlayerInputReader InputReader => input;
        public bool IsInvulnerable => invulnerableTimer > 0f;
        public bool ControlsEnabled { get; set; } = true;

        void Awake()
        {
            motor = GetComponent<PlayerMotor>();
            input = GetComponent<PlayerInputReader>();
        }

        public void SetInvulnerable(float duration) => invulnerableTimer = Mathf.Max(invulnerableTimer, duration);

        void Update()
        {
            invulnerableTimer -= Time.deltaTime;

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
