using System.Collections.Generic;
using System.IO;
using System.Linq;
using LostDungeon.Gameplay.Animation;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace LostDungeon.EditorTools
{
    public static class SpriteSheetSlicer
    {
        const string SpritesRoot = "Assets/_Project/Art/Sprites";
        const string TilesRoot = "Assets/_Project/Art/Tiles";
        const string AnimationRoot = "Assets/_Project/ScriptableObjects/Animations";

        readonly struct SliceRule
        {
            public readonly string Match;
            public readonly int FrameWidth;
            public readonly int FrameHeight;
            public readonly Vector2 Pivot;

            public SliceRule(string match, int frameWidth, int frameHeight, Vector2 pivot)
            {
                Match = match;
                FrameWidth = frameWidth;
                FrameHeight = frameHeight;
                Pivot = pivot;
            }
        }

        static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        const float EnemyFeet = 49f / 150f;

        static readonly SliceRule[] Rules =
        {
            new SliceRule("Characters/Knight/", 112, 96, new Vector2(54f / 112f, 11f / 96f)),
            new SliceRule("_Projectile", 0, 0, Center),
            new SliceRule("_Bomb", 0, 0, Center),
            new SliceRule("Enemies/FlyingEye/", 0, 0, new Vector2(0.515f, 0.5f)),
            new SliceRule("Enemies/Skeleton/", 0, 0, new Vector2(0.52f, EnemyFeet)),
            new SliceRule("Enemies/", 0, 0, new Vector2(0.5f, EnemyFeet)),
            new SliceRule("FX_EnemyDeath", 81, 66, new Vector2(0.5f, 0.3f)),
            new SliceRule("FX/", 0, 0, Center),
            new SliceRule("Tiles/GothicVania/GothicVania_Tileset", 16, 16, Center)
        };

        static readonly string[] Skip = { "Skeleton_Sword" };
        static readonly HashSet<string> LoopingClips = new HashSet<string> { "Idle", "Run", "Walk", "Flight", "Fireball", "Projectile", "Bomb" };

        [MenuItem("Lost Dungeon/Art/Slice Sprite Sheets and Build Animations")]
        public static void SliceAndBuild()
        {
            List<string> paths = AssetDatabase.FindAssets("t:Texture2D", new[] { SpritesRoot, TilesRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToList();

            int sliced = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string path in paths)
                    if (Slice(path)) sliced++;
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();
            int sets = BuildAnimationSets();
            Debug.Log($"[Lost Dungeon] Sliced {sliced} sprite sheets and built {sets} animation sets.");
        }

        static bool Slice(string path)
        {
            string unixPath = path.Replace('\\', '/');
            if (Skip.Any(s => unixPath.Contains(s))) return false;

            SliceRule? match = null;
            foreach (SliceRule rule in Rules)
            {
                if (!unixPath.Contains(rule.Match)) continue;
                match = rule;
                break;
            }
            if (match == null) return false;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return false;

            importer.GetSourceTextureWidthAndHeight(out int textureWidth, out int textureHeight);
            SliceRule chosen = match.Value;
            int frameWidth = chosen.FrameWidth > 0 ? chosen.FrameWidth : textureHeight;
            int frameHeight = chosen.FrameHeight > 0 ? chosen.FrameHeight : textureHeight;
            int columns = textureWidth / frameWidth;
            int rows = textureHeight / frameHeight;
            if (columns == 0 || rows == 0) return false;

            importer.spriteImportMode = SpriteImportMode.Multiple;

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();

            SpriteRect[] existing = provider.GetSpriteRects();
            string baseName = Path.GetFileNameWithoutExtension(path);
            var rects = new List<SpriteRect>();
            int index = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    string spriteName = $"{baseName}_{index}";
                    SpriteRect previous = existing.FirstOrDefault(r => r.name == spriteName);
                    rects.Add(new SpriteRect
                    {
                        name = spriteName,
                        rect = new Rect(column * frameWidth, textureHeight - (row + 1) * frameHeight, frameWidth, frameHeight),
                        alignment = SpriteAlignment.Custom,
                        pivot = chosen.Pivot,
                        spriteID = previous != null ? previous.spriteID : GUID.Generate()
                    });
                    index++;
                }
            }

            provider.SetSpriteRects(rects.ToArray());
            var nameIds = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            nameIds?.SetNameFileIdPairs(rects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
            return true;
        }

        static int BuildAnimationSets()
        {
            EnsureFolder(AnimationRoot);

            var folders = new List<string>();
            folders.AddRange(Directory.GetDirectories(SpritesRoot + "/Characters"));
            folders.AddRange(Directory.GetDirectories(SpritesRoot + "/Enemies"));
            folders.Add(SpritesRoot + "/FX");

            int count = 0;
            foreach (string rawFolder in folders)
            {
                string folder = rawFolder.Replace('\\', '/');
                string setName = Path.GetFileName(folder);
                string assetPath = $"{AnimationRoot}/{setName}Animations.asset";

                var set = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(assetPath);
                if (set == null)
                {
                    set = ScriptableObject.CreateInstance<SpriteAnimationSet>();
                    AssetDatabase.CreateAsset(set, assetPath);
                }

                var clips = new List<SpriteAnimationClip>();
                foreach (string rawFile in Directory.GetFiles(folder, "*.png").OrderBy(f => f))
                {
                    string file = rawFile.Replace('\\', '/');
                    if (Skip.Any(s => file.Contains(s))) continue;

                    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(file)
                        .OfType<Sprite>()
                        .OrderBy(s => FrameIndex(s.name))
                        .ToArray();
                    if (sprites.Length == 0) continue;

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string clipName = fileName.Contains("_") ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                    SpriteAnimationClip previous = set.Find(clipName);

                    clips.Add(new SpriteAnimationClip(
                        clipName,
                        sprites,
                        previous?.FramesPerSecond ?? DefaultFramesPerSecond(clipName),
                        previous?.Loop ?? LoopingClips.Contains(clipName)));
                }

                set.SetClips(clips);
                EditorUtility.SetDirty(set);
                count++;
            }

            AssetDatabase.SaveAssets();
            return count;
        }

        static int FrameIndex(string spriteName)
        {
            int split = spriteName.LastIndexOf('_');
            return split >= 0 && int.TryParse(spriteName.Substring(split + 1), out int value) ? value : 0;
        }

        static float DefaultFramesPerSecond(string clipName)
        {
            if (clipName.StartsWith("Attack")) return 16f;
            if (clipName == "Death") return 10f;
            return 12f;
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
