using System;
using LostDungeon.Gameplay.Items;
using UnityEngine;

namespace LostDungeon.Gameplay.Interaction
{
    public class Chest : Interactable
    {
        [SerializeField] LootTable lootTable;
        [SerializeField] PickupCatalog catalog;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite openedSprite;
        [SerializeField] Color openedTint = new Color(0.55f, 0.55f, 0.6f);
        [SerializeField] Vector2 spawnOffset = new Vector2(0f, 0.8f);

        readonly System.Random random = new System.Random();

        public event Action<Chest> Opened;

        public bool IsOpened { get; private set; }

        public override bool CanInteract(GameObject interactor) => base.CanInteract(interactor) && !IsOpened;

        public override void Interact(GameObject interactor)
        {
            if (IsOpened) return;
            IsOpened = true;

            if (spriteRenderer != null)
            {
                if (openedSprite != null) spriteRenderer.sprite = openedSprite;
                else spriteRenderer.color = openedTint;
            }

            if (lootTable != null && catalog != null)
            {
                Vector2 origin = (Vector2)transform.position + spawnOffset;
                for (int i = 0; i < lootTable.Rolls; i++)
                {
                    (string itemId, int amount) = lootTable.Roll(random);
                    if (itemId != null) catalog.Spawn(itemId, amount, origin);
                }
            }

            Opened?.Invoke(this);
        }
    }
}
