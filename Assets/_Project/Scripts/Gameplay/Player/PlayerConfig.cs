using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Player/Player Config", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] float moveSpeed = 6f;
        [SerializeField, Min(0f)] float acceleration = 70f;
        [SerializeField, Min(0f)] float deceleration = 88f;
        [SerializeField, Range(0f, 1f)] float airControl = 0.7f;

        [Header("Jump")]
        [SerializeField, Min(0f)] float jumpVelocity = 17.8f;
        [SerializeField, Min(0f)] float gravityScale = 4.85f;
        [SerializeField, Min(1f)] float jumpCutGravityMultiplier = 2.3f;
        [SerializeField, Min(0f)] float maxFallSpeed = 22.5f;
        [SerializeField, Min(0f)] float coyoteTime = 0.1f;
        [SerializeField, Min(0f)] float jumpBufferTime = 0.12f;
        [SerializeField, Min(0f)] float platformDropTime = 0.25f;

        [Header("Health")]
        [SerializeField, Min(1)] int maxHearts = 5;
        [SerializeField, Min(0f)] float invulnerabilityAfterHit = 0.9f;
        [SerializeField] Vector2 hurtKnockback = new Vector2(9.4f, 9.4f);
        [SerializeField, Min(0f)] float hurtControlLock = 0.2f;

        [Header("Melee")]
        [SerializeField, Min(1)] int meleeDamage = 2;
        [SerializeField, Min(0.05f)] float meleeCooldown = 0.32f;
        [SerializeField, Range(0f, 1f)] float criticalChance = 0.12f;
        [SerializeField] Vector2 meleeBoxSize = new Vector2(1.6f, 1.8f);
        [SerializeField] Vector2 meleeBoxOffset = new Vector2(1.1f, 1f);

        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float AirControl => airControl;
        public float JumpVelocity => jumpVelocity;
        public float GravityScale => gravityScale;
        public float JumpCutGravityMultiplier => jumpCutGravityMultiplier;
        public float MaxFallSpeed => maxFallSpeed;
        public float CoyoteTime => coyoteTime;
        public float JumpBufferTime => jumpBufferTime;
        public float PlatformDropTime => platformDropTime;
        public int MaxHearts => maxHearts;
        public int MaxHealth => maxHearts * 2;
        public float InvulnerabilityAfterHit => invulnerabilityAfterHit;
        public Vector2 HurtKnockback => hurtKnockback;
        public float HurtControlLock => hurtControlLock;
        public int MeleeDamage => meleeDamage;
        public float MeleeCooldown => meleeCooldown;
        public float CriticalChance => criticalChance;
        public Vector2 MeleeBoxSize => meleeBoxSize;
        public Vector2 MeleeBoxOffset => meleeBoxOffset;
    }
}
