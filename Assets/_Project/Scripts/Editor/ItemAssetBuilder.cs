using LostDungeon.Gameplay.Items;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostDungeon.EditorTools
{
    public static class ItemAssetBuilder
    {
        const string ItemRoot = "Assets/_Project/ScriptableObjects/Items";
        const string LootRoot = "Assets/_Project/ScriptableObjects/LootTables";
        const string PrefabRoot = "Assets/_Project/Prefabs/Pickups";
        const string ItemSprites = "Assets/_Project/Art/Sprites/Items";
        const string PropSprites = "Assets/_Project/Art/Sprites/Props";

        public const string CatalogPath = ItemRoot + "/PickupCatalog.asset";
        public const string ChestLootPath = LootRoot + "/ChestLoot.asset";
        public const string EnemyBonusLootPath = LootRoot + "/EnemyBonusLoot.asset";

        [MenuItem("Lost Dungeon/Setup/Build Item Assets")]
        public static void BuildAll()
        {
            EditorFolders.Ensure(ItemRoot);
            EditorFolders.Ensure(LootRoot);
            EditorFolders.Ensure(PrefabRoot);

            Pickup coin = BuildPickup("Coin", ItemIds.Gold, $"{ItemSprites}/Item_coin.png", true);
            Pickup potion = BuildPickup("Potion", ItemIds.Potion, $"{ItemSprites}/Item_potion.png", false);
            Pickup key = BuildPickup("Key", ItemIds.Key, $"{ItemSprites}/Item_key.png", false);
            Pickup gem = BuildPickup("Gem", ItemIds.Gem, $"{PropSprites}/Item_crystal.png", false);

            var catalog = GetOrCreate<PickupCatalog>(CatalogPath);
            var catalogData = new SerializedObject(catalog);
            SerializedProperty entries = catalogData.FindProperty("entries");
            entries.ClearArray();
            AddCatalogEntry(entries, ItemIds.Gold, coin);
            AddCatalogEntry(entries, ItemIds.Potion, potion);
            AddCatalogEntry(entries, ItemIds.Key, key);
            AddCatalogEntry(entries, ItemIds.Gem, gem);
            catalogData.ApplyModifiedPropertiesWithoutUndo();

            BuildLootTable(ChestLootPath, 2,
                (ItemIds.Gold, 55f, 8, 18),
                (ItemIds.Potion, 25f, 1, 1),
                (ItemIds.Gem, 12f, 1, 1),
                (ItemIds.Gold, 8f, 40, 60));

            BuildLootTable(EnemyBonusLootPath, 1,
                (ItemIds.Potion, 60f, 1, 1),
                (ItemIds.Gem, 40f, 1, 1));

            AssetDatabase.SaveAssets();
            Debug.Log("[Lost Dungeon] Built pickups, pickup catalog and loot tables.");
        }

        public static PickupCatalog Catalog => AssetDatabase.LoadAssetAtPath<PickupCatalog>(CatalogPath);
        public static LootTable ChestLoot => AssetDatabase.LoadAssetAtPath<LootTable>(ChestLootPath);
        public static LootTable EnemyBonusLoot => AssetDatabase.LoadAssetAtPath<LootTable>(EnemyBonusLootPath);
        public static GameObject PickupPrefab(string name) => AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/{name}.prefab");

        static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static Pickup BuildPickup(string name, string itemId, string spritePath, bool spin)
        {
            var root = new GameObject(name);
            root.layer = LayerMask.NameToLayer("Pickup");
            root.AddComponent<CircleCollider2D>().radius = 0.45f;
            var pickup = root.AddComponent<Pickup>();

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            spriteRenderer.sortingLayerName = "Props";

            var data = new SerializedObject(pickup);
            data.FindProperty("itemId").stringValue = itemId;
            data.FindProperty("visual").objectReferenceValue = visual.transform;
            data.FindProperty("spin").boolValue = spin;
            data.FindProperty("groundMask").intValue = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Platform"));
            data.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, $"{PrefabRoot}/{name}.prefab");
            Object.DestroyImmediate(root);
            return prefab.GetComponent<Pickup>();
        }

        static void AddCatalogEntry(SerializedProperty entries, string itemId, Pickup prefab)
        {
            entries.InsertArrayElementAtIndex(entries.arraySize);
            SerializedProperty entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);
            entry.FindPropertyRelative("itemId").stringValue = itemId;
            entry.FindPropertyRelative("prefab").objectReferenceValue = prefab;
        }

        static void BuildLootTable(string path, int rolls, params (string itemId, float weight, int min, int max)[] rows)
        {
            var table = GetOrCreate<LootTable>(path);
            var data = new SerializedObject(table);
            data.FindProperty("rolls").intValue = rolls;
            SerializedProperty entries = data.FindProperty("entries");
            entries.ClearArray();
            foreach (var row in rows)
            {
                entries.InsertArrayElementAtIndex(entries.arraySize);
                SerializedProperty entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);
                entry.FindPropertyRelative("itemId").stringValue = row.itemId;
                entry.FindPropertyRelative("weight").floatValue = row.weight;
                entry.FindPropertyRelative("minAmount").intValue = row.min;
                entry.FindPropertyRelative("maxAmount").intValue = row.max;
            }
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
