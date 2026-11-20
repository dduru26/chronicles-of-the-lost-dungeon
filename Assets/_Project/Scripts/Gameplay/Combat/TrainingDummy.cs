using LostDungeon.Core.Combat;
using LostDungeon.Gameplay.Animation;
using UnityEngine;

namespace LostDungeon.Gameplay.Combat
{
    [RequireComponent(typeof(Health))]
    public class TrainingDummy : MonoBehaviour
    {
        [SerializeField] SpriteAnimator animator;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField, Min(0f)] float reviveDelay = 2f;
        [SerializeField] Color hitTint = new Color(1f, 0.45f, 0.45f);

        Health health;
        float flashTimer;

        void Awake()
        {
            health = GetComponent<Health>();
            if (animator == null) animator = GetComponentInChildren<SpriteAnimator>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void OnEnable()
        {
            health.Damaged += HandleDamaged;
            health.Died += HandleDied;
        }

        void OnDisable()
        {
            health.Damaged -= HandleDamaged;
            health.Died -= HandleDied;
        }

        void HandleDamaged(DamageInfo info)
        {
            Debug.Log($"[Training Dummy] {info.Type} hit for {info.Amount}{(info.IsCritical ? " (critical)" : string.Empty)}. Health {health.Current}/{health.Max}");
            flashTimer = 0.12f;
            if (health.IsAlive) animator.Play("TakeHit", true);
        }

        void HandleDied()
        {
            animator.Play("Death", true);
            Invoke(nameof(Revive), reviveDelay);
        }

        void Revive()
        {
            health.Revive();
            animator.Play("Idle", true);
        }

        void Update()
        {
            if (flashTimer > 0f) flashTimer -= Time.deltaTime;
            spriteRenderer.color = flashTimer > 0f ? hitTint : Color.white;
            if (health.IsAlive && animator.IsFinished && animator.CurrentClipName == "TakeHit") animator.Play("Idle");
        }
    }
}
