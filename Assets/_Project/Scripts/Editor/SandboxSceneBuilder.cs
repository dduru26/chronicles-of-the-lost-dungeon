using LostDungeon.Gameplay.Abilities;
using LostDungeon.Gameplay.Animation;
using LostDungeon.Gameplay.Combat;
using LostDungeon.Gameplay.Player;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace LostDungeon.EditorTools
{
    public static class SandboxSceneBuilder
    {
        const string ScenePath = "Assets/_Project/Scenes/Levels/Sandbox.unity";
        const string PlayerConfigPath = "Assets/_Project/ScriptableObjects/Player/PlayerConfig.asset";
        const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";
        const string KnightAnimationsPath = "Assets/_Project/ScriptableObjects/Animations/KnightAnimations.asset";
        const string FxAnimationsPath = "Assets/_Project/ScriptableObjects/Animations/FXAnimations.asset";
        const string SkeletonAnimationsPath = "Assets/_Project/ScriptableObjects/Animations/SkeletonAnimations.asset";
        const string FireballPrefabPath = "Assets/_Project/Prefabs/Projectiles/Fireball.prefab";
        const string DashAbilityPath = "Assets/_Project/ScriptableObjects/Abilities/Dash.asset";
        const string FireballAbilityPath = "Assets/_Project/ScriptableObjects/Abilities/Fireball.asset";
        const string RockSpriteRoot = "Assets/_Project/Art/Tiles/CraftPixRock";
        const string TileAssetRoot = "Assets/_Project/Art/Tiles/Palette";

        static readonly string[] Layout =
        {
            "############################################",
            "#..........................................#",
            "#..........................................#",
            "#..........................................#",
            "#..........................................#",
            "#..........................................#",
            "#.........................-----............#",
            "#..........................................#",
            "#..........................................#",
            "#.........-----.....-----..................#",
            "#...........................####...........#",
            "#.P........................######..........#",
            "############################################",
            "############################################"
        };

        [MenuItem("Lost Dungeon/Setup/Build Sandbox Scene")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            ProjectSetup.Run();
            if (AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(KnightAnimationsPath) == null) SpriteSheetSlicer.SliceAndBuild();

            EnsureFolder("Assets/_Project/ScriptableObjects/Player");
            EnsureFolder("Assets/_Project/Prefabs/Player");
            EnsureFolder(TileAssetRoot);
            EnsureFolder("Assets/_Project/Prefabs/Projectiles");
            EnsureFolder("Assets/_Project/ScriptableObjects/Abilities");

            if (AssetDatabase.LoadAssetAtPath<PlayerConfig>(PlayerConfigPath) == null)
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PlayerConfig>(), PlayerConfigPath);
                AssetDatabase.SaveAssets();
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PlayerConfig config = AssetDatabase.LoadAssetAtPath<PlayerConfig>(PlayerConfigPath);

            Projectile fireballPrefab = CreateFireballPrefab();
            AbilityDefinition dash = CreateDashAbility();
            AbilityDefinition fireball = CreateFireballAbility(fireballPrefab);

            CreateLighting();
            Vector3 spawn = CreateTilemaps();
            GameObject player = CreatePlayer(config, spawn, dash, fireball);
            CreateTrainingDummy(spawn + new Vector3(12f, 0f, 0f));
            CreateCameras(player.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Lost Dungeon] Sandbox scene built at " + ScenePath + ". Press Play to test the player.");
        }

        static void CreateLighting()
        {
            var lightObject = new GameObject("Global Light 2D");
            var light = lightObject.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 1f;
        }

        static Vector3 CreateTilemaps()
        {
            var grid = new GameObject("Grid", typeof(Grid));
            Tilemap ground = CreateTilemap(grid.transform, "Ground", "Ground", false);
            Tilemap platforms = CreateTilemap(grid.transform, "Platforms", "Platform", true);

            int height = Layout.Length;
            Vector3 spawn = new Vector3(2f, 3f, 0f);

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < Layout[row].Length; column++)
                {
                    char cell = Layout[row][column];
                    var position = new Vector3Int(column, height - 1 - row, 0);

                    if (cell == '#') ground.SetTile(position, GetTile(GroundSprite(row, column), Tile.ColliderType.Grid));
                    else if (cell == '-') platforms.SetTile(position, GetTile(PlatformSprite(row, column), Tile.ColliderType.Sprite));
                    else if (cell == 'P') spawn = new Vector3(column + 0.5f, height - 1 - row, 0f);
                }
            }

            return spawn;
        }

        static Tilemap CreateTilemap(Transform grid, string name, string layer, bool oneWay)
        {
            var tilemapObject = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
            tilemapObject.transform.SetParent(grid, false);
            tilemapObject.layer = LayerMask.NameToLayer(layer);
            tilemapObject.GetComponent<TilemapRenderer>().sortingLayerName = "World";

            var body = tilemapObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            var tilemapCollider = tilemapObject.AddComponent<TilemapCollider2D>();
            tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
            var composite = tilemapObject.AddComponent<CompositeCollider2D>();
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

            if (oneWay)
            {
                var effector = tilemapObject.AddComponent<PlatformEffector2D>();
                effector.useOneWay = true;
                effector.surfaceArc = 160f;
                composite.usedByEffector = true;
            }

            return tilemapObject.GetComponent<Tilemap>();
        }

        static bool Solid(int row, int column)
        {
            if (row < 0 || row >= Layout.Length || column < 0 || column >= Layout[row].Length) return true;
            return Layout[row][column] == '#';
        }

        static string GroundSprite(int row, int column)
        {
            bool airAbove = !Solid(row - 1, column);
            bool airLeft = !Solid(row, column - 1);
            bool airRight = !Solid(row, column + 1);

            if (airAbove)
            {
                if (airLeft) return "Rock_tile1";
                if (airRight) return "Rock_tile3";
                return "Rock_tile2";
            }

            if (airLeft) return "Rock_tile4";
            if (airRight) return "Rock_tile6";
            int variant = (row * 7 + column * 13) % 3;
            return variant == 0 ? "Rock_tile5" : variant == 1 ? "Rock_tile14" : "Rock_tile15";
        }

        static string PlatformSprite(int row, int column)
        {
            bool platformLeft = column > 0 && Layout[row][column - 1] == '-';
            bool platformRight = column < Layout[row].Length - 1 && Layout[row][column + 1] == '-';
            if (!platformLeft) return "Rock_tile27";
            if (!platformRight) return "Rock_tile30";
            return "Rock_tile28";
        }

        static TileBase GetTile(string spriteName, Tile.ColliderType colliderType)
        {
            string tilePath = $"{TileAssetRoot}/{spriteName}.asset";
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile != null) return tile;

            tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{RockSpriteRoot}/{spriteName}.png");
            tile.colliderType = colliderType;
            AssetDatabase.CreateAsset(tile, tilePath);
            return tile;
        }

        static GameObject CreatePlayer(PlayerConfig config, Vector3 spawn, AbilityDefinition dash, AbilityDefinition fireball)
        {
            var player = new GameObject("Player");
            player.layer = LayerMask.NameToLayer("Player");
            player.tag = "Player";
            player.transform.position = spawn;

            player.AddComponent<Rigidbody2D>();
            var capsule = player.AddComponent<CapsuleCollider2D>();
            capsule.offset = new Vector2(0f, 1.05f);
            capsule.size = new Vector2(1f, 2.1f);

            var motor = player.AddComponent<PlayerMotor>();
            player.AddComponent<PlayerInputReader>();
            player.AddComponent<Health>();
            player.AddComponent<PlayerController>();

            var combat = player.AddComponent<PlayerCombat>();
            var combatData = new SerializedObject(combat);
            combatData.FindProperty("hittableMask").intValue = 1 << LayerMask.NameToLayer("Enemy");
            combatData.ApplyModifiedPropertiesWithoutUndo();

            var abilities = player.AddComponent<PlayerAbilities>();
            var abilityData = new SerializedObject(abilities);
            SerializedProperty slots = abilityData.FindProperty("slots");
            slots.ClearArray();
            AddSlot(slots, PlayerButton.Dash, dash);
            AddSlot(slots, PlayerButton.Fire, fireball);
            abilityData.ApplyModifiedPropertiesWithoutUndo();

            var motorData = new SerializedObject(motor);
            motorData.FindProperty("config").objectReferenceValue = config;
            motorData.FindProperty("groundMask").intValue = 1 << LayerMask.NameToLayer("Ground");
            motorData.FindProperty("platformMask").intValue = 1 << LayerMask.NameToLayer("Platform");
            motorData.ApplyModifiedPropertiesWithoutUndo();

            var visual = new GameObject("Visual");
            visual.transform.SetParent(player.transform, false);
            var spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Characters";

            var animator = visual.AddComponent<SpriteAnimator>();
            var animatorData = new SerializedObject(animator);
            animatorData.FindProperty("animationSet").objectReferenceValue = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(KnightAnimationsPath);
            animatorData.ApplyModifiedPropertiesWithoutUndo();

            var driver = visual.AddComponent<PlayerAnimationDriver>();
            var driverData = new SerializedObject(driver);
            driverData.FindProperty("motor").objectReferenceValue = motor;
            driverData.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAssetAndConnect(player, PlayerPrefabPath, InteractionMode.AutomatedAction);
            return player;
        }

        static void CreateCameras(Transform target)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.07f, 0.06f, 0.1f);
            cameraObject.transform.position = new Vector3(target.position.x, target.position.y, -10f);
            cameraObject.AddComponent<UniversalAdditionalCameraData>();
            cameraObject.AddComponent<CinemachineBrain>();

            var followObject = new GameObject("Follow Camera");
            var followCamera = followObject.AddComponent<CinemachineCamera>();
            followCamera.Follow = target;
            LensSettings lens = followCamera.Lens;
            lens.OrthographicSize = 7f;
            followCamera.Lens = lens;
            var composer = followObject.AddComponent<CinemachinePositionComposer>();
            composer.CameraDistance = 10f;
            composer.Damping = new Vector3(0.6f, 0.8f, 0f);
            composer.TargetOffset = new Vector3(0f, 1.5f, 0f);
        }

        static void AddSlot(SerializedProperty slots, PlayerButton button, AbilityDefinition ability)
        {
            slots.InsertArrayElementAtIndex(slots.arraySize);
            SerializedProperty slot = slots.GetArrayElementAtIndex(slots.arraySize - 1);
            slot.FindPropertyRelative("button").enumValueIndex = (int)button;
            slot.FindPropertyRelative("ability").objectReferenceValue = ability;
        }

        static Projectile CreateFireballPrefab()
        {
            var root = new GameObject("Fireball");
            root.layer = LayerMask.NameToLayer("PlayerAttack");
            root.AddComponent<Rigidbody2D>();
            root.AddComponent<CircleCollider2D>().radius = 0.3f;
            var projectile = root.AddComponent<Projectile>();

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            var spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "FX";
            var animator = visual.AddComponent<SpriteAnimator>();
            var animatorData = new SerializedObject(animator);
            animatorData.FindProperty("animationSet").objectReferenceValue = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(FxAnimationsPath);
            animatorData.FindProperty("startingClip").stringValue = "Fireball";
            animatorData.ApplyModifiedPropertiesWithoutUndo();

            var projectileData = new SerializedObject(projectile);
            projectileData.FindProperty("visual").objectReferenceValue = spriteRenderer;
            projectileData.FindProperty("blockingMask").intValue = 1 << LayerMask.NameToLayer("Ground");
            projectileData.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, FireballPrefabPath);
            Object.DestroyImmediate(root);
            return prefab.GetComponent<Projectile>();
        }

        static AbilityDefinition CreateDashAbility()
        {
            var dash = AssetDatabase.LoadAssetAtPath<DashAbility>(DashAbilityPath);
            if (dash != null) return dash;
            dash = ScriptableObject.CreateInstance<DashAbility>();
            AssetDatabase.CreateAsset(dash, DashAbilityPath);
            var data = new SerializedObject(dash);
            data.FindProperty("id").stringValue = "dash";
            data.FindProperty("displayName").stringValue = "Dash";
            data.FindProperty("cooldown").floatValue = 1.1f;
            data.ApplyModifiedPropertiesWithoutUndo();
            return dash;
        }

        static AbilityDefinition CreateFireballAbility(Projectile prefab)
        {
            var fireball = AssetDatabase.LoadAssetAtPath<FireballAbility>(FireballAbilityPath);
            if (fireball == null)
            {
                fireball = ScriptableObject.CreateInstance<FireballAbility>();
                AssetDatabase.CreateAsset(fireball, FireballAbilityPath);
            }
            var data = new SerializedObject(fireball);
            data.FindProperty("id").stringValue = "fireball";
            data.FindProperty("displayName").stringValue = "Fireball";
            data.FindProperty("cooldown").floatValue = 1.6f;
            data.FindProperty("projectilePrefab").objectReferenceValue = prefab;
            data.FindProperty("targetMask").intValue = 1 << LayerMask.NameToLayer("Enemy");
            data.ApplyModifiedPropertiesWithoutUndo();
            return fireball;
        }

        static void CreateTrainingDummy(Vector3 position)
        {
            var dummy = new GameObject("Training Dummy");
            dummy.layer = LayerMask.NameToLayer("Enemy");
            dummy.transform.position = position;
            var box = dummy.AddComponent<BoxCollider2D>();
            box.offset = new Vector2(0f, 1.4f);
            box.size = new Vector2(1.4f, 2.8f);
            var health = dummy.AddComponent<Health>();
            var healthData = new SerializedObject(health);
            healthData.FindProperty("maxHealth").intValue = 20;
            healthData.ApplyModifiedPropertiesWithoutUndo();

            var visual = new GameObject("Visual");
            visual.transform.SetParent(dummy.transform, false);
            var spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Characters";
            spriteRenderer.flipX = true;
            var animator = visual.AddComponent<SpriteAnimator>();
            var animatorData = new SerializedObject(animator);
            animatorData.FindProperty("animationSet").objectReferenceValue = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(SkeletonAnimationsPath);
            animatorData.ApplyModifiedPropertiesWithoutUndo();

            dummy.AddComponent<TrainingDummy>();
        }

        static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
