using System;
using System.Collections.Generic;
using LostDungeon.Core.Events;
using LostDungeon.Gameplay.Abilities;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAbilities : MonoBehaviour
    {
        [Serializable]
        public class AbilitySlot
        {
            [SerializeField] PlayerButton button = PlayerButton.Dash;
            [SerializeField] AbilityDefinition ability;

            public PlayerButton Button => button;
            public IAbility Ability => ability;
            public float CooldownRemaining { get; set; }

            public AbilitySlot() { }

            public AbilitySlot(PlayerButton button, AbilityDefinition ability)
            {
                this.button = button;
                this.ability = ability;
            }

            public void Replace(AbilityDefinition newAbility)
            {
                ability = newAbility;
                CooldownRemaining = 0f;
            }
        }

        [SerializeField] List<AbilitySlot> slots = new List<AbilitySlot>();

        PlayerController controller;

        public event Action<int, IAbility> AbilityUsed;
        public event Action<int, IAbility> AbilityChanged;

        public IReadOnlyList<AbilitySlot> Slots => slots;

        void Awake() => controller = GetComponent<PlayerController>();

        public float CooldownFraction(int index)
        {
            if (index < 0 || index >= slots.Count || slots[index].Ability == null) return 0f;
            float cooldown = slots[index].Ability.Cooldown;
            return cooldown > 0f ? Mathf.Clamp01(slots[index].CooldownRemaining / cooldown) : 0f;
        }

        public void SetAbility(int index, AbilityDefinition ability)
        {
            if (index < 0 || index >= slots.Count) return;
            slots[index].Replace(ability);
            AbilityChanged?.Invoke(index, ability);
        }

        public bool TryActivate(int index)
        {
            if (index < 0 || index >= slots.Count) return false;
            AbilitySlot slot = slots[index];
            IAbility ability = slot.Ability;
            if (ability == null || slot.CooldownRemaining > 0f || !ability.CanActivate(controller)) return false;

            ability.Activate(controller);
            slot.CooldownRemaining = ability.Cooldown;
            GameEvents.RaiseAbilityActivated(ability.Id, ability.Cooldown);
            AbilityUsed?.Invoke(index, ability);
            return true;
        }

        void Update()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                AbilitySlot slot = slots[i];
                if (slot.CooldownRemaining > 0f) slot.CooldownRemaining -= Time.deltaTime;
                if (controller.ControlsEnabled && controller.InputReader.Pressed(slot.Button)) TryActivate(i);
            }
        }
    }
}
