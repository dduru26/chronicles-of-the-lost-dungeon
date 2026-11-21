using LostDungeon.Gameplay.Combat;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Enemies/Enemy Config", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] string id = "enemy";
        [SerializeField] string displayName = "Enemy";

        [Header("Stats")]
        [SerializeField, Min(1)] int maxHealth = 5;
        [SerializeField, Min(1)] int damage = 1;
        [SerializeField, Min(0)] int goldDrop = 2;

        [Header("Movement")]
        [SerializeField] bool isFlying;
        [SerializeField, Min(0f)] float gravityScale = 4.85f;
        [SerializeField, Min(0f)] float patrolSpeed = 1.6f;
        [SerializeField, Min(0f)] float chaseSpeed = 2.9f;
        [SerializeField, Min(0f)] float patrolRadius = 4f;

        [Header("Perception")]
        [SerializeField, Min(0f)] float sightRange = 7.5f;
        [SerializeField, Min(0f)] float verticalSightRange = 3f;

        [Header("Attack")]
        [SerializeField, Min(0f)] float attackRange = 1.6f;
        [SerializeField, Min(0f)] float attackHeight = 2f;
        [SerializeField, Min(0f)] float attackWindup = 0.38f;
        [SerializeField, Min(0f)] float attackRecover = 0.45f;
        [SerializeField, Min(0f)] float lungeSpeed;

        [Header("Ranged")]
        [SerializeField] Projectile projectile;
        [SerializeField, Min(0f)] float projectileSpeed = 10f;
        [SerializeField, Min(0.1f)] float fireInterval = 1.5f;
        [SerializeField, Min(0f)] float keepDistance = 4.4f;
        [SerializeField] Vector2 projectileSpawnOffset = new Vector2(0.6f, 0f);

        [Header("Reactions")]
        [SerializeField] bool staggerable = true;
        [SerializeField] Vector2 knockback = new Vector2(6.9f, 4.4f);
        [SerializeField, Min(0f)] float hurtDuration = 0.25f;
        [SerializeField, Min(0f)] float stunDuration = 1.4f;

        [Header("Animation Clips")]
        [SerializeField] string idleClip = "Idle";
        [SerializeField] string moveClip = "Run";
        [SerializeField] string attackClip = "Attack";
        [SerializeField] string hurtClip = "TakeHit";
        [SerializeField] string deathClip = "Death";
        [SerializeField] string blockClip = "Shield";

        public string Id => id;
        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
        public int Damage => damage;
        public int GoldDrop => goldDrop;
        public bool IsFlying => isFlying;
        public float GravityScale => gravityScale;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float PatrolRadius => patrolRadius;
        public float SightRange => sightRange;
        public float VerticalSightRange => verticalSightRange;
        public float AttackRange => attackRange;
        public float AttackHeight => attackHeight;
        public float AttackWindup => attackWindup;
        public float AttackRecover => attackRecover;
        public float LungeSpeed => lungeSpeed;
        public Projectile Projectile => projectile;
        public float ProjectileSpeed => projectileSpeed;
        public float FireInterval => fireInterval;
        public float KeepDistance => keepDistance;
        public Vector2 ProjectileSpawnOffset => projectileSpawnOffset;
        public bool Staggerable => staggerable;
        public Vector2 Knockback => knockback;
        public float HurtDuration => hurtDuration;
        public float StunDuration => stunDuration;
        public string IdleClip => idleClip;
        public string MoveClip => moveClip;
        public string AttackClip => attackClip;
        public string HurtClip => hurtClip;
        public string DeathClip => deathClip;
        public string BlockClip => blockClip;
    }
}
