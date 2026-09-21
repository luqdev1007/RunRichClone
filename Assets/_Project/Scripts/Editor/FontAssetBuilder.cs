using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace RunRich.Editor
{
    public static class FontAssetBuilder
    {
        private const string MenuPath = "Tools/RunRich/Fonts/Build Game Font Asset";
        private const string OutputFolder = "Assets/_Project/Fonts";
        private const string SourceFontPath = OutputFolder + "/Rubik-Black.ttf";
        private const string FontAssetName = "Rubik-Black SDF";
        private const string ShaderName = "TextMeshPro/Distance Field";

        private const int SamplingPointSize = 64;
        private const int AtlasPadding = 11;
        private const int AtlasWidth = 1024;
        private const int AtlasHeight = 1024;

        private const string OutlineKeyword = "OUTLINE_ON";
        private const string UnderlayKeyword = "UNDERLAY_ON";
        private const float FaceDilate = 0f;
        private const float OutlineSoftness = 0f;
        private const float WeightNormal = 0f;
        private const float WeightBold = 0.75f;

        private static readonly Vector2Int[] CharacterRanges =
        {
            new Vector2Int(0x0020, 0x007E),
            new Vector2Int(0x00D7, 0x00D7),
            new Vector2Int(0x0400, 0x045F)
        };

        private static readonly MaterialPreset[] Presets =
        {
            new MaterialPreset("Shadow", false, true,
                Color.black, 0f,
                new Color(0f, 0f, 0f, 0.682f), 0.167f, -0.244f, -0.338f, 0.48f),

            new MaterialPreset("BlackOut", true, false,
                Color.black, 0.086f,
                new Color(0f, 0f, 0f, 0.243f), 0.35f, -1f, -1f, 0.29f),

            new MaterialPreset("Win", true, true,
                Color.white, 0.06f,
                new Color(0.882f, 0.882f, 0.882f, 1f), -0.092f, -0.181f, 0.066f, 0.014f),

            new MaterialPreset("GameOver", true, true,
                Color.white, 0.03f,
                new Color(0.855f, 0.855f, 0.855f, 1f), 0f, -0.293f, 0f, 0f)
        };

        [MenuItem(MenuPath)]
        private static void BuildGameFontAsset()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);
            if (font == null)
            {
                Debug.LogError("Source font not found: " + SourceFontPath);
                return;
            }

            var shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                Debug.LogError("Shader not found: " + ShaderName + ". Import TMP essential resources first.");
                return;
            }

            EnsureOutputFolder();
            DeletePreviousOutput();

            var fontAsset = TMP_FontAsset.CreateFontAsset(font, SamplingPointSize, AtlasPadding, GlyphRenderMode.SDFAA,
                AtlasWidth, AtlasHeight, AtlasPopulationMode.Dynamic, true);
            fontAsset.name = FontAssetName;

            var characters = BuildCharacterSet();
            string missing;
            fontAsset.TryAddCharacters(characters, out missing);

            fontAsset.atlasPopulationMode = AtlasPopulationMode.Static;
            fontAsset.ReadFontAssetDefinition();

            SaveFontAsset(fontAsset, shader);
            var createdPresets = CreatePresets(fontAsset);

            Debug.Log(BuildReport(fontAsset, characters, missing, createdPresets));
            Selection.activeObject = fontAsset;
        }

        private static void EnsureOutputFolder()
        {
            if (!AssetDatabase.IsValidFolder(OutputFolder))
                AssetDatabase.CreateFolder("Assets/_Project", "Fonts");
        }

        private static void DeletePreviousOutput()
        {
            AssetDatabase.DeleteAsset(FontAssetPath());
            foreach (var preset in Presets)
                AssetDatabase.DeleteAsset(PresetPath(preset));
        }

        private static string FontAssetPath()
        {
            return OutputFolder + "/" + FontAssetName + ".asset";
        }

        private static string PresetPath(MaterialPreset preset)
        {
            return OutputFolder + "/" + FontAssetName + " - " + preset.Name + ".mat";
        }

        private static string BuildCharacterSet()
        {
            var builder = new StringBuilder();
            foreach (var range in CharacterRanges)
            {
                for (int code = range.x; code <= range.y; code++)
                    builder.Append((char)code);
            }

            return builder.ToString();
        }

        private static void SaveFontAsset(TMP_FontAsset fontAsset, Shader shader)
        {
            AssetDatabase.CreateAsset(fontAsset, FontAssetPath());

            for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
            {
                var atlas = fontAsset.atlasTextures[i];
                atlas.name = FontAssetName + " Atlas" + (i == 0 ? string.Empty : " " + i);
                atlas.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(atlas, fontAsset);
            }

            fontAsset.material.shader = shader;
            fontAsset.material.name = FontAssetName + " Material";
            fontAsset.material.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
        }

        private static List<string> CreatePresets(TMP_FontAsset fontAsset)
        {
            var created = new List<string>();

            foreach (var preset in Presets)
            {
                var material = new Material(fontAsset.material);
                material.name = FontAssetName + " - " + preset.Name;

                material.SetColor(ShaderUtilities.ID_FaceColor, Color.white);
                material.SetFloat(ShaderUtilities.ID_FaceDilate, FaceDilate);
                material.SetFloat(ShaderUtilities.ID_WeightNormal, WeightNormal);
                material.SetFloat(ShaderUtilities.ID_WeightBold, WeightBold);

                material.SetColor(ShaderUtilities.ID_OutlineColor, preset.OutlineColor);
                material.SetFloat(ShaderUtilities.ID_OutlineWidth, preset.OutlineWidth);
                material.SetFloat(ShaderUtilities.ID_OutlineSoftness, OutlineSoftness);

                material.SetColor(ShaderUtilities.ID_UnderlayColor, preset.UnderlayColor);
                material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, preset.UnderlayOffsetX);
                material.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, preset.UnderlayOffsetY);
                material.SetFloat(ShaderUtilities.ID_UnderlayDilate, preset.UnderlayDilate);
                material.SetFloat(ShaderUtilities.ID_UnderlaySoftness, preset.UnderlaySoftness);

                SetKeyword(material, OutlineKeyword, preset.UseOutline);
                SetKeyword(material, UnderlayKeyword, preset.UseUnderlay);

                AssetDatabase.CreateAsset(material, PresetPath(preset));
                created.Add(material.name);
            }

            AssetDatabase.SaveAssets();
            return created;
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (enabled)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        private static string BuildReport(TMP_FontAsset fontAsset, string characters, string missing, List<string> presets)
        {
            var report = new StringBuilder();
            report.AppendLine("Font asset built: " + FontAssetPath());
            report.AppendLine("  source: " + SourceFontPath);
            report.AppendLine("  sampling point size: " + SamplingPointSize + ", padding: " + AtlasPadding);
            report.AppendLine("  atlas: " + AtlasWidth + "x" + AtlasHeight + ", textures: " + fontAsset.atlasTextures.Length);
            report.AppendLine("  population mode: " + fontAsset.atlasPopulationMode);
            report.AppendLine("  requested characters: " + characters.Length);
            report.AppendLine("  character table: " + fontAsset.characterTable.Count);
            report.AppendLine("  glyph table: " + fontAsset.glyphTable.Count);
            report.AppendLine("  missing: " + (string.IsNullOrEmpty(missing) ? 0 : missing.Length));
            report.AppendLine("  presets: " + string.Join(", ", presets.ToArray()));
            return report.ToString();
        }

        private sealed class MaterialPreset
        {
            public readonly string Name;
            public readonly bool UseOutline;
            public readonly bool UseUnderlay;
            public readonly Color OutlineColor;
            public readonly float OutlineWidth;
            public readonly Color UnderlayColor;
            public readonly float UnderlayOffsetX;
            public readonly float UnderlayOffsetY;
            public readonly float UnderlayDilate;
            public readonly float UnderlaySoftness;

            public MaterialPreset(string name, bool useOutline, bool useUnderlay,
                Color outlineColor, float outlineWidth,
                Color underlayColor, float underlayOffsetX, float underlayOffsetY,
                float underlayDilate, float underlaySoftness)
            {
                Name = name;
                UseOutline = useOutline;
                UseUnderlay = useUnderlay;
                OutlineColor = outlineColor;
                OutlineWidth = outlineWidth;
                UnderlayColor = underlayColor;
                UnderlayOffsetX = underlayOffsetX;
                UnderlayOffsetY = underlayOffsetY;
                UnderlayDilate = underlayDilate;
                UnderlaySoftness = underlaySoftness;
            }
        }
    }
}
