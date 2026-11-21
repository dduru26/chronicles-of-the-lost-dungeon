using System;
using LostDungeon.Gameplay.Interfaces;
using LostDungeon.Gameplay.Player;
using LostDungeon.Gameplay.Pooling;
using UnityEngine;

namespace LostDungeon.Gameplay.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour, ICollectable, IPoolable
    {
        [SerializeField] string itemId = ItemIds.Gold;
        [SerializeField, Min(1)] int amount = 1;
        [SerializeField] Transform visual;
        [SerializeField] LayerMask groundMask;
        [SerializeField] bool spin;
        [SerializeField, Min(0f)] float bobHeight = 0.12f;
        [SerializeField, Min(0f)] float bobSpeed = 3f;
        [SerializeField, Min(0f)] float magnetRadius = 2.2f;
        [SerializeField, Min(0f)] float magnetSpeed = 12f;
        [SerializeField, Min(0f)] float collectDelay = 0.3f;
        [SerializeField, Min(0f)] float restHeight = 0.3f;
        [SerializeField, Min(0f)] float gravity = 30f;

        Vector2 velocity;
        bool airborne;
        bool collected;
        float age;
        float phase;

        public static event Action<Pickup> Collected;

        public string ItemId => itemId;
        public int Amount => amount;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            phase = UnityEngine.Random.value * Mathf.PI * 2f;
            age = collectDelay;
        }

        public void Configure(string id, int count)
        {
            itemId = id;
            amount = Mathf.Max(1, count);
        }

        public void Launch(Vector2 launchVelocity)
        {
            velocity = launchVelocity;
            airborne = true;
            age = 0f;
        }

        void Update()
        {
            age += Time.deltaTime;

            if (airborne) Fall();
            else if (!AttractToPlayer()) Bob();

            if (spin && visual != null)
            {
                float width = Mathf.Abs(Mathf.Sin(Time.time * 4f + phase));
                visual.localScale = new Vector3(Mathf.Max(0.15f, width), 1f, 1f);
            }
        }

        void Fall()
        {
            velocity.y -= gravity * Time.deltaTime;
            Vector2 step = velocity * Time.deltaTime;

            if (step.y < 0f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, -step.y + restHeight, groundMask);
                if (hit)
                {
                    transform.position = new Vector3(transform.position.x + step.x, hit.point.y + restHeight, 0f);
                    airborne = false;
                    velocity = Vector2.zero;
                    return;
                }
            }

            transform.position += (Vector3)step;
        }

        bool AttractToPlayer()
        {
            PlayerController player = PlayerController.Active;
            if (player == null || player.IsDead || age < collectDelay) return false;

            Vector3 target = player.transform.position + Vector3.up;
            if ((target - transform.position).sqrMagnitude > magnetRadius * magnetRadius) return false;

            transform.position = Vector3.MoveTowards(transform.position, target, magnetSpeed * Time.deltaTime);
            return true;
        }

        void Bob()
        {
            if (visual != null) visual.localPosition = new Vector3(0f, Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight, 0f);
        }

        void OnTriggerEnter2D(Collider2D other) => TryCollect(other);
        void OnTriggerStay2D(Collider2D other) => TryCollect(other);

        void TryCollect(Collider2D other)
        {
            if (collected || age < collectDelay) return;
            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
            if (inventory != null) Collect(inventory.gameObject);
        }

        public void Collect(GameObject collector)
        {
            if (collected) return;
            PlayerInventory inventory = collector.GetComponent<PlayerInventory>();
            if (inventory == null) return;

            collected = true;
            inventory.AddItem(itemId, amount);
            Collected?.Invoke(this);

            PooledObject pooled = GetComponent<PooledObject>();
            if (pooled != null) pooled.ReturnToPool();
            else gameObject.SetActive(false);
        }

        public void OnSpawned()
        {
            collected = false;
            airborne = false;
            age = 0f;
            if (visual != null) visual.localPosition = Vector3.zero;
        }

        public void OnDespawned()
        {
            velocity = Vector2.zero;
        }
    }
}
