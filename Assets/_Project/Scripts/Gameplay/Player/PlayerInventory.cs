using System;
using LostDungeon.Core.Events;
using LostDungeon.Core.Items;
using LostDungeon.Gameplay.Items;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField, Min(1)] int potionHealAmount = 4;
        [SerializeField, Min(0)] int startingPotions = 1;

        readonly Inventory inventory = new Inventory();
        PlayerController controller;

        public event Action<string, int> ItemChanged;

        public Inventory Items => inventory;
        public int Gold => inventory.Count(ItemIds.Gold);

        void Awake()
        {
            controller = GetComponent<PlayerController>();
            inventory.ItemChanged += HandleItemChanged;
        }

        void Start()
        {
            if (startingPotions > 0 && !inventory.Has(ItemIds.Potion)) inventory.Add(ItemIds.Potion, startingPotions);
            GameEvents.RaiseGoldChanged(Gold);
            GameEvents.RaiseInventoryChanged();
        }

        void OnDestroy() => inventory.ItemChanged -= HandleItemChanged;

        void HandleItemChanged(string id, int total)
        {
            if (id == ItemIds.Gold) GameEvents.RaiseGoldChanged(total);
            GameEvents.RaiseInventoryChanged();
            ItemChanged?.Invoke(id, total);
        }

        public void AddItem(string id, int amount)
        {
            inventory.Add(id, amount);
            GameEvents.RaiseItemCollected(id, inventory.Count(id));
        }

        public bool TryConsume(string id, int amount = 1) => inventory.TryRemove(id, amount);

        void Update()
        {
            if (controller.ControlsEnabled && controller.InputReader.Pressed(PlayerButton.Potion)) TryDrinkPotion();
        }

        public bool TryDrinkPotion()
        {
            var health = controller.Health;
            if (!health.IsAlive || health.Current >= health.Max) return false;
            if (!inventory.TryRemove(ItemIds.Potion)) return false;
            health.Heal(potionHealAmount);
            return true;
        }
    }
}
