using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RunRich.Editor
{
    public static class UiSpriteImporter
    {
        private const string MenuRoot = "Tools/RunRich/Sprites/";
        private const string TextureFolder = "Assets/_Project/Art/Visual/Texture2D";
        private const string SpriteFolder = "Assets/_Project/Art/Visual/Sprite";
        private const string TextureExtension = ".png";

        private static readonly string[] AdditionalTextureNames =
        {
            "TryAgain",
            "jackpot",
            "currency_0",
            "UISprite"
        };

        private enum ImportMode
        {
            DryRun,
            Apply
        }

        [MenuItem(MenuRoot + "Log Sprite Import Candidates")]
        private static void LogSpriteImportCandidates()
        {
            Run(ImportMode.DryRun);
        }

        [MenuItem(MenuRoot + "Import Matching Textures As Sprites")]
        private static void ImportMatchingTexturesAsSprites()
        {
            Run(ImportMode.Apply);
        }

        private static void Run(ImportMode mode)
        {
            var wanted = CollectWantedTextureNames();
            var report = new StringBuilder();
            report.AppendLine(mode == ImportMode.Apply ? "Importing textures as sprites" : "Sprite import candidates (dry run)");

            int converted = 0;
            int alreadySprite = 0;

            foreach (var path in FindTexturePaths())
            {
                var name = Path.GetFileNameWithoutExtension(path);
                if (!wanted.Contains(name))
                    continue;

                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    continue;

                if (importer.textureType == TextureImporterType.Sprite)
                {
                    alreadySprite++;
                    continue;
                }

                report.AppendLine("  " + name);

                if (mode == ImportMode.Apply)
                    ApplySpriteSettings(importer);

                converted++;
            }

            if (mode == ImportMode.Apply)
                AssetDatabase.Refresh();

            report.AppendLine("candidates: " + converted + ", already sprites: " + alreadySprite + ", wanted names: " + wanted.Count);
            Debug.Log(report.ToString());
        }

        private static HashSet<string> CollectWantedTextureNames()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);

            foreach (var guid in AssetDatabase.FindAssets("t:Sprite", new[] { SpriteFolder }))
                names.Add(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)));

            foreach (var name in AdditionalTextureNames)
                names.Add(name);

            return names;
        }

        private static IEnumerable<string> FindTexturePaths()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { TextureFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(TextureExtension, StringComparison.OrdinalIgnoreCase))
                    yield return path;
            }
        }

        private static void ApplySpriteSettings(TextureImporter importer)
        {
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);

            settings.textureType = TextureImporterType.Sprite;
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.mipmapEnabled = false;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.alphaIsTransparency = true;
            settings.npotScale = TextureImporterNPOTScale.None;

            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
    }
}
