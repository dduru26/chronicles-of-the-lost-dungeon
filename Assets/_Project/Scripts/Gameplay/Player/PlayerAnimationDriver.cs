using LostDungeon.Gameplay.Animation;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(SpriteAnimator), typeof(SpriteRenderer))]
    public class PlayerAnimationDriver : MonoBehaviour
    {
        [SerializeField] PlayerMotor motor;
        [SerializeField, Min(0f)] float runThreshold = 0.5f;
        [SerializeField, Min(0f)] float apexThreshold = 2f;

        SpriteAnimator animator;
        SpriteRenderer spriteRenderer;
        string actionClip;
        bool actionActive;
        bool actionLocked;

        public bool IsPlayingAction => actionActive;

        void Awake()
        {
            animator = GetComponent<SpriteAnimator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (motor == null) motor = GetComponentInParent<PlayerMotor>();
        }

        void OnEnable() => animator.ClipFinished += HandleClipFinished;
        void OnDisable() => animator.ClipFinished -= HandleClipFinished;

        public void PlayAction(string clipName, bool lockUntilReset = false)
        {
            if (actionLocked) return;
            actionClip = clipName;
            actionActive = animator.Play(clipName, true);
            actionLocked = actionActive && lockUntilReset;
        }

        public void ResetActions()
        {
            actionLocked = false;
            actionActive = false;
        }

        public void SetFlash(bool visible) => spriteRenderer.enabled = visible;

        void HandleClipFinished(string clipName)
        {
            if (actionActive && !actionLocked && clipName == actionClip) actionActive = false;
        }

        void LateUpdate()
        {
            spriteRenderer.flipX = motor.Facing < 0;
            if (actionActive) return;

            Vector2 velocity = motor.Velocity;
            string clip;
            if (motor.IsDashing) clip = "Run";
            else if (!motor.IsGrounded) clip = velocity.y > apexThreshold ? "JumpStart" : velocity.y > -apexThreshold ? "JumpTransition" : "JumpFall";
            else clip = Mathf.Abs(velocity.x) > runThreshold ? "Run" : "Idle";

            animator.Play(clip);
        }
    }
}
