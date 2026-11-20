using System;
using System.Collections.Generic;
using LostDungeon.Core.Combat;
using LostDungeon.Core.Events;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCombat : MonoBehaviour
    {
        public const string SwordId = "sword";

        [SerializeField] LayerMask hittableMask;
        [SerializeField, Min(0f)] float activeStart = 0.06f;
        [SerializeField, Min(0f)] float activeEnd = 0.22f;

        PlayerController controller;
        PlayerConfig config;
        readonly HashSet<IDamageable> hitThisSwing = new HashSet<IDamageable>();
        readonly Collider2D[] hits = new Collider2D[8];
        readonly System.Random random = new System.Random();
        ContactFilter2D filter;
        float cooldownTimer;
        float swingTimer;
        int comboStep;

        public event Action<int> AttackStarted;
        public event Action<IDamageable, DamageInfo> HitLanded;

        public bool IsAttacking { get; private set; }
        public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

        void Awake()
        {
            controller = GetComponent<PlayerController>();
            filter = new ContactFilter2D { useLayerMask = true, layerMask = hittableMask, useTriggers = true };
        }

        void Start() => config = controller.Motor.Config;

        void Update()
        {
            cooldownTimer -= Time.deltaTime;

            if (controller.ControlsEnabled && controller.InputReader.Pressed(PlayerButton.Attack) && cooldownTimer <= 0f)
                StartSwing();

            if (!IsAttacking) return;

            swingTimer += Time.deltaTime;
            if (swingTimer >= activeStart && swingTimer <= activeEnd) DetectHits();
            if (swingTimer > activeEnd) EndSwing();
        }

        void StartSwing()
        {
            cooldownTimer = config.MeleeCooldown;
            swingTimer = 0f;
            IsAttacking = true;
            hitThisSwing.Clear();
            comboStep = comboStep == 1 ? 2 : 1;
            controller.Motor.CanTurn = false;
            controller.Animation.PlayAction("Attack" + comboStep);
            GameEvents.RaiseAbilityActivated(SwordId, config.MeleeCooldown);
            AttackStarted?.Invoke(comboStep);
        }

        void EndSwing()
        {
            IsAttacking = false;
            controller.Motor.CanTurn = true;
        }

        Vector2 HitboxCenter => (Vector2)transform.position + new Vector2(config.MeleeBoxOffset.x * controller.Facing, config.MeleeBoxOffset.y);

        void DetectHits()
        {
            int count = Physics2D.OverlapBox(HitboxCenter, config.MeleeBoxSize, 0f, filter, hits);
            for (int i = 0; i < count; i++)
            {
                IDamageable target = hits[i].GetComponentInParent<IDamageable>();
                if (target == null || !target.IsAlive || hitThisSwing.Contains(target)) continue;
                if (hits[i].transform.IsChildOf(transform)) continue;

                hitThisSwing.Add(target);
                bool critical = DamageCalculator.RollCritical(config.CriticalChance, random);
                int damage = DamageCalculator.Calculate(config.MeleeDamage, 1f, critical);
                var info = new DamageInfo(damage, DamageType.Melee, transform.position.x, transform.position.y, critical);
                target.TakeDamage(info);
                HitLanded?.Invoke(target, info);
            }
        }

        void OnDrawGizmosSelected()
        {
            PlayerConfig previewConfig = config;
            if (previewConfig == null)
            {
                var motor = GetComponent<PlayerMotor>();
                if (motor != null) previewConfig = motor.Config;
            }
            if (previewConfig == null) return;

            int facing = controller != null ? controller.Facing : 1;
            Gizmos.color = IsAttacking ? Color.red : new Color(1f, 0.6f, 0f, 0.6f);
            Vector2 center = (Vector2)transform.position + new Vector2(previewConfig.MeleeBoxOffset.x * facing, previewConfig.MeleeBoxOffset.y);
            Gizmos.DrawWireCube(center, previewConfig.MeleeBoxSize);
        }
    }
}
