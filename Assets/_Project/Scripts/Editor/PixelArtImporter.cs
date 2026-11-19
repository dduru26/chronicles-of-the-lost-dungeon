using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace LostDungeon.EditorTools
{
    public class PixelArtImporter : AssetPostprocessor
    {
        public const string ArtRoot = "Assets/_Project/Art/";
        public const int DefaultPixelsPerUnit = 16;
        static readonly Regex PpuTag = new Regex(@"_ppu(\d+)", RegexOptions.IgnoreCase);

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot)) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = ResolvePixelsPerUnit(assetPath);
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteExtrude = 0;
            importer.SetTextureSettings(settings);
        }

        public static int ResolvePixelsPerUnit(string path)
        {
            var match = PpuTag.Match(path);
            return match.Success ? int.Parse(match.Groups[1].Value) : DefaultPixelsPerUnit;
        }
    }
}
