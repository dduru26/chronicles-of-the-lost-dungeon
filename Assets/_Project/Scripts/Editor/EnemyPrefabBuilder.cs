using System;
using LostDungeon.Gameplay.Animation;
using LostDungeon.Gameplay.Combat;
using LostDungeon.Gameplay.Enemies;
using LostDungeon.Gameplay.Enemies.Behaviours;
using LostDungeon.Gameplay.Items;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostDungeon.EditorTools
{
    public static class EnemyPrefabBuilder
    {
        const string ConfigRoot = "Assets/_Project/ScriptableObjects/Enemies";
        const string BehaviourRoot = "Assets/_Project/ScriptableObjects/Enemies/Behaviours";
        const string PrefabRoot = "Assets/_Project/Prefabs/Enemies";
        const string ProjectileRoot = "Assets/_Project/Prefabs/Projectiles";
        const string AnimationRoot = "Assets/_Project/ScriptableObjects/Animations";

        public const string WarriorPrefabPath = PrefabRoot + "/Warrior.prefab";
        public const string ArcherPrefabPath = PrefabRoot + "/Archer.prefab";
        public const string GuardianPrefabPath = PrefabRoot + "/Guardian.prefab";

        [MenuItem("Lost Dungeon/Setup/Build Enemy Prefabs")]
        public static void BuildAll()
        {
            EditorFolders.Ensure(ConfigRoot);
            EditorFolders.Ensure(BehaviourRoot);
            EditorFolders.Ensure(PrefabRoot);
            EditorFolders.Ensure(ProjectileRoot);
            ItemAssetBuilder.BuildAll();

            var melee = GetOrCreate<MeleeChaserBehaviour>($"{BehaviourRoot}/MeleeChaser.asset");
            var ranged = GetOrCreate<RangedKiterBehaviour>($"{BehaviourRoot}/RangedKiter.asset");
            var guardian = GetOrCreate<ShieldGuardianBehaviour>($"{BehaviourRoot}/ShieldGuardian.asset");
            Projectile eyeBolt = BuildEyeBolt();

            EnemyConfig warriorConfig = BuildConfig("WarriorConfig", data =>
            {
                Set(data, "id", "warrior"); Set(data, "displayName", "Goblin Warrior");
                Set(data, "maxHealth", 5); Set(data, "damage", 1); Set(data, "goldDrop", 2);
                Set(data, "patrolSpeed", 1.6f); Set(data, "chaseSpeed", 3.2f); Set(data, "patrolRadius", 4f);
                Set(data, "sightRange", 7.5f); Set(data, "verticalSightRange", 2.5f);
                Set(data, "attackRange", 1.4f); Set(data, "attackHeight", 2f);
                Set(data, "attackWindup", 0.38f); Set(data, "attackRecover", 0.4f);
                Set(data, "moveClip", "Run"); Set(data, "blockClip", "");
            });

            EnemyConfig archerConfig = BuildConfig("ArcherConfig", data =>
            {
                Set(data, "id", "archer"); Set(data, "displayName", "Flying Eye");
                Set(data, "maxHealth", 3); Set(data, "damage", 1); Set(data, "goldDrop", 2);
                Set(data, "isFlying", true); Set(data, "patrolSpeed", 1.5f); Set(data, "chaseSpeed", 2.6f); Set(data, "patrolRadius", 3f);
                Set(data, "sightRange", 11f); Set(data, "verticalSightRange", 7f);
                Set(data, "attackWindup", 0.45f); Set(data, "projectileSpeed", 9f); Set(data, "fireInterval", 1.6f); Set(data, "keepDistance", 5f);
                data.FindProperty("projectile").objectReferenceValue = eyeBolt;
                data.FindProperty("knockback").vector2Value = new Vector2(4f, 0f);
                Set(data, "idleClip", "Flight"); Set(data, "moveClip", "Flight"); Set(data, "blockClip", "");
            });

            EnemyConfig guardianConfig = BuildConfig("GuardianConfig", data =>
            {
                Set(data, "id", "guardian"); Set(data, "displayName", "Skeleton Guardian");
                Set(data, "maxHealth", 9); Set(data, "damage", 2); Set(data, "goldDrop", 4);
                Set(data, "patrolSpeed", 1.2f); Set(data, "chaseSpeed", 1.75f); Set(data, "patrolRadius", 3f);
                Set(data, "sightRange", 7.5f); Set(data, "verticalSightRange", 2.5f);
                Set(data, "attackRange", 1.8f); Set(data, "attackHeight", 2.6f);
                Set(data, "attackWindup", 0.5f); Set(data, "attackRecover", 0.6f); Set(data, "lungeSpeed", 7.5f);
                Set(data, "stunDuration", 1.4f);
                data.FindProperty("knockback").vector2Value = new Vector2(2f, 0f);
                Set(data, "moveClip", "Walk"); Set(data, "blockClip", "Shield");
            });

            BuildEnemyPrefab(WarriorPrefabPath, "Warrior", warriorConfig, melee, "GoblinAnimations", "Idle", root =>
            {
                var capsule = root.AddComponent<CapsuleCollider2D>();
                capsule.offset = new Vector2(0f, 1.05f);
                capsule.size = new Vector2(1f, 2.1f);
            });

            BuildEnemyPrefab(ArcherPrefabPath, "Archer", archerConfig, ranged, "FlyingEyeAnimations", "Flight", root =>
            {
                root.AddComponent<CircleCollider2D>().radius = 0.75f;
            });

            BuildEnemyPrefab(GuardianPrefabPath, "Guardian", guardianConfig, guardian, "SkeletonAnimations", "Idle", root =>
            {
                var capsule = root.AddComponent<CapsuleCollider2D>();
                capsule.offset = new Vector2(0f, 1.45f);
                capsule.size = new Vector2(1.2f, 2.9f);
            });

            AssetDatabase.SaveAssets();
            Debug.Log("[Lost Dungeon] Built Warrior, Archer and Guardian prefabs.");
        }

        static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static EnemyConfig BuildConfig(string name, Action<SerializedObject> configure)
        {
            EnemyConfig config = GetOrCreate<EnemyConfig>($"{ConfigRoot}/{name}.asset");
            var data = new SerializedObject(config);
            configure(data);
            data.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            return config;
        }

        static void Set(SerializedObject data, string field, string value) => data.FindProperty(field).stringValue = value;
        static void Set(SerializedObject data, string field, int value) => data.FindProperty(field).intValue = value;
        static void Set(SerializedObject data, string field, float value) => data.FindProperty(field).floatValue = value;
        static void Set(SerializedObject data, string field, bool value) => data.FindProperty(field).boolValue = value;

        static Projectile BuildEyeBolt()
        {
            var root = new GameObject("EyeBolt");
            root.layer = LayerMask.NameToLayer("EnemyAttack");
            root.AddComponent<Rigidbody2D>();
            root.AddComponent<CircleCollider2D>().radius = 0.25f;
            var projectile = root.AddComponent<Projectile>();

            var visual = CreateVisual(root.transform, "FX", "FlyingEyeAnimations", "Projectile");
            var data = new SerializedObject(projectile);
            data.FindProperty("visual").objectReferenceValue = visual;
            data.FindProperty("blockingMask").intValue = 1 << LayerMask.NameToLayer("Ground");
            data.FindProperty("alignToVelocity").boolValue = true;
            data.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, $"{ProjectileRoot}/EyeBolt.prefab");
            Object.DestroyImmediate(root);
            return prefab.GetComponent<Projectile>();
        }

        static void BuildEnemyPrefab(string path, string name, EnemyConfig config, EnemyBehaviour behaviour, string animationSet, string startingClip, Action<GameObject> addCollider)
        {
            var root = new GameObject(name);
            root.layer = LayerMask.NameToLayer("Enemy");
            root.AddComponent<Rigidbody2D>();
            addCollider(root);
            root.AddComponent<Health>();
            var controller = root.AddComponent<EnemyController>();
            var dropper = root.AddComponent<EnemyLootDropper>();
            var dropperData = new SerializedObject(dropper);
            dropperData.FindProperty("catalog").objectReferenceValue = ItemAssetBuilder.Catalog;
            dropperData.FindProperty("bonusLoot").objectReferenceValue = ItemAssetBuilder.EnemyBonusLoot;
            dropperData.ApplyModifiedPropertiesWithoutUndo();

            SpriteRenderer visual = CreateVisual(root.transform, "Characters", animationSet, startingClip);

            var data = new SerializedObject(controller);
            data.FindProperty("config").objectReferenceValue = config;
            data.FindProperty("behaviour").objectReferenceValue = behaviour;
            data.FindProperty("animator").objectReferenceValue = visual.GetComponent<SpriteAnimator>();
            data.FindProperty("spriteRenderer").objectReferenceValue = visual;
            data.FindProperty("groundMask").intValue = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Platform"));
            data.FindProperty("playerMask").intValue = 1 << LayerMask.NameToLayer("Player");
            data.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        static SpriteRenderer CreateVisual(Transform parent, string sortingLayer, string animationSet, string startingClip)
        {
            var visual = new GameObject("Visual");
            visual.transform.SetParent(parent, false);
            var spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = sortingLayer;
            var animator = visual.AddComponent<SpriteAnimator>();
            var data = new SerializedObject(animator);
            data.FindProperty("animationSet").objectReferenceValue = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>($"{AnimationRoot}/{animationSet}.asset");
            data.FindProperty("startingClip").stringValue = startingClip;
            data.ApplyModifiedPropertiesWithoutUndo();
            return spriteRenderer;
        }
    }
}
