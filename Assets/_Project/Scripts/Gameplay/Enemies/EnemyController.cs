using System;
using LostDungeon.Core.Combat;
using LostDungeon.Core.Events;
using LostDungeon.Core.Patterns;
using LostDungeon.Gameplay.Animation;
using LostDungeon.Gameplay.Combat;
using LostDungeon.Gameplay.Enemies.Behaviours;
using LostDungeon.Gameplay.Enemies.States;
using LostDungeon.Gameplay.Interfaces;
using LostDungeon.Gameplay.Player;
using LostDungeon.Gameplay.Pooling;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] EnemyConfig config;
        [SerializeField] EnemyBehaviour behaviour;
        [SerializeField] SpriteAnimator animator;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] LayerMask groundMask;
        [SerializeField] LayerMask playerMask;
        [SerializeField] Color hitTint = new Color(1f, 0.45f, 0.45f);
        [SerializeField, Min(0f)] float hoverAmplitude = 0.35f;

        Rigidbody2D body;
        Collider2D bodyCollider;
        Health health;
        StateMachine<EnemyController> machine;
        EnemyBrain brain;
        HurtState hurtState;
        DeadState deadState;
        Vector2 homePosition;
        float stateTime;
        float flashTimer;
        float hoverPhase;
        Bounds lastStrikeBox;
        float lastStrikeTimer;

        public event Action<EnemyController> Defeated;
        public event Action<EnemyController, DamageInfo> Blocked;

        public EnemyConfig Config => config;
        public Rigidbody2D Body => body;
        public Health Health => health;
        public int Facing { get; private set; } = -1;
        public float TimeInState => stateTime;
        public bool IsDead => machine != null && machine.Current == deadState;
        public string CurrentStateName => machine?.Current?.GetType().Name ?? "None";
        public Vector2 HomePosition => homePosition;

        PlayerController Target => PlayerController.Active;
        bool HasTarget => Target != null && !Target.IsDead;
        public float DistanceToTargetX => HasTarget ? Mathf.Abs(Target.transform.position.x - transform.position.x) : float.MaxValue;
        public int DirectionToTarget => HasTarget && Target.transform.position.x >= transform.position.x ? 1 : -1;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
            health = GetComponent<Health>();
            if (animator == null) animator = GetComponentInChildren<SpriteAnimator>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            body.freezeRotation = true;
            body.gravityScale = config.IsFlying ? 0f : config.GravityScale;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            machine = new StateMachine<EnemyController>(this);
            machine.StateChanged += (_, _) => stateTime = 0f;
            hurtState = new HurtState();
            deadState = new DeadState();
            homePosition = transform.position;
            hoverPhase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        void OnEnable()
        {
            health.Damaged += HandleDamaged;
            health.Blocked += HandleBlocked;
            health.Died += HandleDied;
        }

        void OnDisable()
        {
            health.Damaged -= HandleDamaged;
            health.Blocked -= HandleBlocked;
            health.Died -= HandleDied;
        }

        void Start()
        {
            health.Initialize(config.MaxHealth);
            brain = behaviour.Build(this);
            machine.ChangeState(brain.InitialState);
        }

        void Update()
        {
            stateTime += Time.deltaTime;
            machine.Tick(Time.deltaTime);

            if (flashTimer > 0f) flashTimer -= Time.deltaTime;
            if (lastStrikeTimer > 0f) lastStrikeTimer -= Time.deltaTime;
            spriteRenderer.color = flashTimer > 0f ? hitTint : Color.white;
            spriteRenderer.flipX = Facing < 0;
        }

        void FixedUpdate()
        {
            if (!config.IsFlying || IsDead || machine.Current == hurtState) return;
            float targetY = homePosition.y + Mathf.Sin(Time.time * 2f + hoverPhase) * hoverAmplitude;
            body.linearVelocity = new Vector2(body.linearVelocity.x, (targetY - transform.position.y) * 4f);
        }

        public void ChangeState(IState<EnemyController> state)
        {
            if (state != null && !IsDead) machine.ChangeState(state);
        }

        public bool IsInState(IState<EnemyController> state) => machine.IsIn(state);
        public void ResetStateTimer() => stateTime = 0f;

        void HandleDamaged(DamageInfo info)
        {
            flashTimer = 0.12f;
            if (!health.IsAlive) return;

            IState<EnemyController> reaction = brain?.DamageReaction?.Invoke(info);
            if (reaction != null)
            {
                machine.ChangeState(reaction);
                return;
            }

            if (!config.Staggerable) return;

            if (machine.Current == hurtState)
            {
                hurtState.Retrigger(this, info);
                return;
            }

            hurtState.Prepare(brain.RecoverState, info);
            machine.ChangeState(hurtState);
        }

        void HandleBlocked(DamageInfo info)
        {
            PlayClip(config.BlockClip, true);
            Blocked?.Invoke(this, info);
        }

        void HandleDied()
        {
            machine.ChangeState(deadState);
            GameEvents.RaiseEnemyDefeated(config.Id);
            Defeated?.Invoke(this);
        }

        public void PlayClip(string clipName, bool restart = false)
        {
            if (!string.IsNullOrEmpty(clipName) && animator.HasClip(clipName)) animator.Play(clipName, restart);
        }

        public float ClipDuration(string clipName)
        {
            SpriteAnimationClip clip = animator.AnimationSet != null ? animator.AnimationSet.Find(clipName) : null;
            return clip?.Duration ?? 0f;
        }

        public void Face(int direction)
        {
            if (direction != 0) Facing = direction > 0 ? 1 : -1;
        }

        public void FaceTarget()
        {
            if (HasTarget) Face(DirectionToTarget);
        }

        public bool CanSeeTarget(float range)
        {
            if (!HasTarget) return false;
            Vector2 offset = Target.transform.position - transform.position;
            return Mathf.Abs(offset.x) <= range && Mathf.Abs(offset.y) <= config.VerticalSightRange;
        }

        public void SetHorizontalVelocity(float x) => body.linearVelocity = new Vector2(x, body.linearVelocity.y);
        public void SetVelocity(Vector2 velocity) => body.linearVelocity = velocity;
        public void StopMoving() => SetHorizontalVelocity(0f);

        public bool HasGroundAhead(int direction)
        {
            if (config.IsFlying) return true;
            Bounds bounds = bodyCollider.bounds;
            var origin = new Vector2(direction > 0 ? bounds.max.x + 0.1f : bounds.min.x - 0.1f, bounds.min.y + 0.1f);
            return Physics2D.Raycast(origin, Vector2.down, 0.6f, groundMask);
        }

        public bool HasWallAhead(int direction)
        {
            Bounds bounds = bodyCollider.bounds;
            var origin = new Vector2(bounds.center.x, bounds.min.y + 0.3f);
            return Physics2D.Raycast(origin, new Vector2(direction, 0f), bounds.extents.x + 0.2f, groundMask);
        }

        bool CanMove(int direction) => HasGroundAhead(direction) && !HasWallAhead(direction);

        public void Patrol(float speed)
        {
            float fromHome = transform.position.x - homePosition.x;
            bool beyondRadius = config.PatrolRadius > 0f && Mathf.Abs(fromHome) > config.PatrolRadius && Mathf.Sign(fromHome) == Facing;
            if (!CanMove(Facing) || beyondRadius) Facing = -Facing;
            SetHorizontalVelocity(CanMove(Facing) ? Facing * speed : 0f);
        }

        public bool MoveTowardTarget(float speed)
        {
            FaceTarget();
            bool canMove = CanMove(Facing);
            SetHorizontalVelocity(canMove ? Facing * speed : 0f);
            return canMove;
        }

        public bool CanMoveAwayFromTarget() => CanMove(-DirectionToTarget);

        public bool MoveAwayFromTarget(float speed)
        {
            int direction = -DirectionToTarget;
            if (!CanMove(direction))
            {
                StopMoving();
                return false;
            }
            SetHorizontalVelocity(direction * speed);
            return true;
        }

        public bool MeleeStrike(float range, float height, int damage)
        {
            Bounds bounds = bodyCollider.bounds;
            var center = new Vector2(bounds.center.x + Facing * (bounds.extents.x + range * 0.5f), bounds.min.y + height * 0.5f);
            var size = new Vector2(range, height);
            lastStrikeBox = new Bounds(center, size);
            lastStrikeTimer = 0.2f;

            Collider2D hit = Physics2D.OverlapBox(center, size, 0f, playerMask);
            if (hit == null) return false;

            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null || !target.IsAlive) return false;

            target.TakeDamage(new DamageInfo(damage, DamageType.Melee, transform.position.x, transform.position.y));
            return true;
        }

        public void FireProjectileAtTarget()
        {
            if (!HasTarget || config.Projectile == null) return;

            Vector2 origin = (Vector2)bodyCollider.bounds.center + new Vector2(Facing * config.ProjectileSpawnOffset.x, config.ProjectileSpawnOffset.y);
            Vector2 aimPoint = (Vector2)Target.transform.position + Vector2.up;
            Vector2 direction = (aimPoint - origin).normalized;

            GameObject shot = PoolManager.GetOrCreate().Spawn(config.Projectile.gameObject, origin, Quaternion.identity);
            shot.GetComponent<Projectile>().Launch(direction * config.ProjectileSpeed, config.Damage, DamageType.Projectile, playerMask, 3f, transform);
        }

        public void DisableBody()
        {
            bodyCollider.enabled = false;
            body.linearVelocity = Vector2.zero;
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        public void Despawn() => gameObject.SetActive(false);

        void OnDrawGizmosSelected()
        {
            if (config == null) return;
            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(config.SightRange * 2f, config.VerticalSightRange * 2f, 0f));
            if (lastStrikeTimer > 0f)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(lastStrikeBox.center, lastStrikeBox.size);
            }
        }
    }
}
