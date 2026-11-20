using System;
using LostDungeon.Core.Combat;
using LostDungeon.Gameplay.Interfaces;
using LostDungeon.Gameplay.Pooling;
using UnityEngine;

namespace LostDungeon.Gameplay.Combat
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] SpriteRenderer visual;
        [SerializeField] LayerMask blockingMask;

        Rigidbody2D body;
        PooledObject pooled;
        int damage;
        DamageType damageType;
        LayerMask targetMask;
        float lifeTimer;
        Transform owner;
        bool alive;

        public static event Action<Projectile, Vector2> Impacted;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            GetComponent<Collider2D>().isTrigger = true;
        }

        public void Launch(Vector2 velocity, int damageAmount, DamageType type, LayerMask targets, float lifetime, Transform source)
        {
            damage = damageAmount;
            damageType = type;
            targetMask = targets;
            lifeTimer = lifetime;
            owner = source;
            alive = true;
            body.linearVelocity = velocity;
            if (visual != null) visual.flipX = velocity.x < 0f;
        }

        void Update()
        {
            if (!alive) return;
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f) Despawn();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!alive) return;
            if (owner != null && other.transform.IsChildOf(owner)) return;

            int layerBit = 1 << other.gameObject.layer;

            if ((targetMask & layerBit) != 0)
            {
                IDamageable target = other.GetComponentInParent<IDamageable>();
                if (target != null && target.IsAlive)
                {
                    target.TakeDamage(new DamageInfo(damage, damageType, transform.position.x, transform.position.y));
                    Despawn();
                    return;
                }
            }

            if ((blockingMask & layerBit) != 0) Despawn();
        }

        void Despawn()
        {
            if (!alive) return;
            alive = false;
            Impacted?.Invoke(this, transform.position);
            body.linearVelocity = Vector2.zero;
            if (pooled == null) pooled = GetComponent<PooledObject>();
            if (pooled != null) pooled.ReturnToPool();
            else gameObject.SetActive(false);
        }

        public void OnSpawned() { }

        public void OnDespawned()
        {
            alive = false;
            if (body != null) body.linearVelocity = Vector2.zero;
        }
    }
}
