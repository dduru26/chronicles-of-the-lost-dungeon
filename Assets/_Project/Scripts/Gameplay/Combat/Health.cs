using System;
using LostDungeon.Core.Combat;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] int maxHealth = 10;
        [SerializeField, Min(0f)] float invulnerabilityDuration;

        int current;
        float invulnerableTimer;

        public event Action<int, int> Changed;
        public event Action<DamageInfo> Damaged;
        public event Action<DamageInfo> Blocked;
        public event Action<int> Healed;
        public event Action Died;

        public Func<DamageInfo, bool> BlockFilter { get; set; }

        public int Current => current;
        public int Max => maxHealth;
        public float Normalized => maxHealth > 0 ? (float)current / maxHealth : 0f;
        public bool IsAlive => current > 0;
        public bool IsInvulnerable => invulnerableTimer > 0f;

        public float InvulnerabilityDuration
        {
            get => invulnerabilityDuration;
            set => invulnerabilityDuration = Mathf.Max(0f, value);
        }

        void Awake() => current = maxHealth;

        void Update()
        {
            if (invulnerableTimer > 0f) invulnerableTimer -= Time.deltaTime;
        }

        public void Initialize(int max, bool refill = true)
        {
            maxHealth = Mathf.Max(1, max);
            current = refill ? maxHealth : Mathf.Min(current, maxHealth);
            Changed?.Invoke(current, maxHealth);
        }

        public void TakeDamage(DamageInfo info)
        {
            if (!IsAlive || IsInvulnerable || info.Amount <= 0) return;

            if (BlockFilter != null && BlockFilter(info))
            {
                Blocked?.Invoke(info);
                return;
            }

            current = Mathf.Max(0, current - info.Amount);
            invulnerableTimer = invulnerabilityDuration;
            Damaged?.Invoke(info);
            Changed?.Invoke(current, maxHealth);
            if (current == 0) Died?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0 || current == maxHealth) return;
            int before = current;
            current = Mathf.Min(maxHealth, current + amount);
            Healed?.Invoke(current - before);
            Changed?.Invoke(current, maxHealth);
        }

        public void SetInvulnerable(float duration) => invulnerableTimer = Mathf.Max(invulnerableTimer, duration);

        public void Revive()
        {
            current = maxHealth;
            invulnerableTimer = 0f;
            Changed?.Invoke(current, maxHealth);
        }
    }
}
