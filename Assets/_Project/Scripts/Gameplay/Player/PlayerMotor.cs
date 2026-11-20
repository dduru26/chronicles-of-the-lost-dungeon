using System;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] PlayerConfig config;
        [SerializeField] LayerMask groundMask;
        [SerializeField] LayerMask platformMask;
        [SerializeField, Min(0.01f)] float groundCheckDepth = 0.08f;

        Rigidbody2D body;
        CapsuleCollider2D capsule;
        readonly Collider2D[] groundHits = new Collider2D[4];
        ContactFilter2D groundFilter;

        float moveInput;
        bool jumpHeld;
        float coyoteTimer;
        float bufferTimer;
        float dropTimer;
        float lockTimer;
        float controlLockTimer;
        bool lockIgnoresGravity;
        Vector2 lockedVelocity;
        Collider2D ignoredPlatform;

        public event Action Jumped;
        public event Action<float> Landed;

        public PlayerConfig Config => config;
        public Rigidbody2D Body => body;
        public Vector2 Velocity => body.linearVelocity;
        public bool IsGrounded { get; private set; }
        public int Facing { get; private set; } = 1;
        public bool CanTurn { get; set; } = true;
        public bool IsDashing => lockTimer > 0f;

        Vector2 FeetPoint => new Vector2(capsule.bounds.center.x, capsule.bounds.min.y);
        Vector2 FeetSize => new Vector2(capsule.bounds.size.x * 0.85f, groundCheckDepth * 2f);

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            capsule = GetComponent<CapsuleCollider2D>();
            body.gravityScale = config.GravityScale;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            capsule.sharedMaterial = new PhysicsMaterial2D("PlayerNoFriction") { friction = 0f, bounciness = 0f };
            groundFilter = new ContactFilter2D { useLayerMask = true, layerMask = groundMask | platformMask, useTriggers = false };
        }

        public void SetMoveInput(float value) => moveInput = Mathf.Clamp(value, -1f, 1f);
        public void SetJumpHeld(bool held) => jumpHeld = held;
        public void RequestJump() => bufferTimer = config.JumpBufferTime;

        public void FaceDirection(int direction)
        {
            if (direction != 0) Facing = direction > 0 ? 1 : -1;
        }

        public void LockVelocity(Vector2 velocity, float duration, bool ignoreGravity)
        {
            lockedVelocity = velocity;
            lockTimer = duration;
            lockIgnoresGravity = ignoreGravity;
        }

        public void ApplyKnockback(Vector2 velocity, float controlLock)
        {
            lockTimer = 0f;
            body.gravityScale = config.GravityScale;
            body.linearVelocity = velocity;
            controlLockTimer = controlLock;
        }

        public void Halt()
        {
            moveInput = 0f;
            lockTimer = 0f;
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        }

        public bool TryDropThroughPlatform()
        {
            if (!IsGrounded) return false;

            Collider2D platform = Physics2D.OverlapBox(FeetPoint, FeetSize, 0f, platformMask);
            if (platform == null) return false;
            if (Physics2D.OverlapBox(FeetPoint, FeetSize, 0f, groundMask) != null) return false;

            ReleaseIgnoredPlatform();
            ignoredPlatform = platform;
            Physics2D.IgnoreCollision(capsule, platform, true);
            dropTimer = config.PlatformDropTime;
            IsGrounded = false;
            coyoteTimer = 0f;
            return true;
        }

        void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            Vector2 velocity = body.linearVelocity;

            bool wasGrounded = IsGrounded;
            IsGrounded = velocity.y <= 0.01f && CheckGround();
            if (IsGrounded && !wasGrounded) Landed?.Invoke(-velocity.y);

            coyoteTimer = IsGrounded ? config.CoyoteTime : coyoteTimer - deltaTime;
            bufferTimer -= deltaTime;
            controlLockTimer -= deltaTime;

            if (dropTimer > 0f)
            {
                dropTimer -= deltaTime;
                if (dropTimer <= 0f) ReleaseIgnoredPlatform();
            }

            if (lockTimer > 0f)
            {
                lockTimer -= deltaTime;
                body.gravityScale = lockIgnoresGravity ? 0f : config.GravityScale;
                body.linearVelocity = lockedVelocity;
                if (lockTimer <= 0f) body.gravityScale = config.GravityScale;
                return;
            }

            if (controlLockTimer <= 0f)
            {
                float target = moveInput * config.MoveSpeed;
                bool hasInput = Mathf.Abs(moveInput) > 0.01f;
                float rate = (hasInput ? config.Acceleration : config.Deceleration) * (IsGrounded ? 1f : config.AirControl);
                velocity.x = Mathf.MoveTowards(velocity.x, target, rate * deltaTime);
                if (CanTurn && hasInput) Facing = moveInput > 0f ? 1 : -1;
            }

            if (bufferTimer > 0f && coyoteTimer > 0f)
            {
                velocity.y = config.JumpVelocity;
                bufferTimer = 0f;
                coyoteTimer = 0f;
                IsGrounded = false;
                Jumped?.Invoke();
            }

            bool cutJump = velocity.y > 0f && !jumpHeld;
            body.gravityScale = cutJump ? config.GravityScale * config.JumpCutGravityMultiplier : config.GravityScale;
            velocity.y = Mathf.Max(velocity.y, -config.MaxFallSpeed);
            body.linearVelocity = velocity;
        }

        bool CheckGround()
        {
            int count = Physics2D.OverlapBox(FeetPoint, FeetSize, 0f, groundFilter, groundHits);
            for (int i = 0; i < count; i++)
                if (groundHits[i] != ignoredPlatform) return true;
            return false;
        }

        void ReleaseIgnoredPlatform()
        {
            if (ignoredPlatform != null) Physics2D.IgnoreCollision(capsule, ignoredPlatform, false);
            ignoredPlatform = null;
            dropTimer = 0f;
        }

        void OnDrawGizmosSelected()
        {
            if (capsule == null) capsule = GetComponent<CapsuleCollider2D>();
            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(FeetPoint, FeetSize);
        }
    }
}
