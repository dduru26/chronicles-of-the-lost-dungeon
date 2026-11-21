using LostDungeon.Gameplay.Items;
using UnityEngine;

namespace LostDungeon.Gameplay.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyLootDropper : MonoBehaviour
    {
        [SerializeField] PickupCatalog catalog;
        [SerializeField] LootTable bonusLoot;
        [SerializeField, Range(0f, 1f)] float bonusChance = 0.25f;
        [SerializeField] Vector2 dropOffset = new Vector2(0f, 1f);

        EnemyController enemy;
        readonly System.Random random = new System.Random();

        void Awake() => enemy = GetComponent<EnemyController>();
        void OnEnable() => enemy.Defeated += HandleDefeated;
        void OnDisable() => enemy.Defeated -= HandleDefeated;

        void HandleDefeated(EnemyController defeated)
        {
            if (catalog == null) return;
            Vector2 origin = (Vector2)transform.position + dropOffset;

            if (defeated.Config.GoldDrop > 0) catalog.Spawn(ItemIds.Gold, defeated.Config.GoldDrop, origin);

            if (bonusLoot != null && random.NextDouble() < bonusChance)
            {
                (string itemId, int amount) = bonusLoot.Roll(random);
                if (itemId != null) catalog.Spawn(itemId, amount, origin);
            }
        }
    }
}
