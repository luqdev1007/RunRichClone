using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RunRich.Editor
{
    public static class BrokenMaterialRemapper
    {
        private const string MenuRoot = "Tools/RunRich/Materials/";
        private const string TargetShaderName = "Universal Render Pipeline/Simple Lit";
        private const string TextureFolder = "Assets/_Project/Art/Visual/Texture2D";
        private const string PropsMaterialPrefix = "props";
        private const string PropsAtlasTextureName = "atlas";
        private const string ForcedTransparentMaterialName = "Water";
        private const float ForcedTransparentAlpha = 0.74f;
        private const float FallbackCutoff = 0.5f;

        private static readonly string[] UiShaderMarkers = { "_ClipRect", "_Stencil" };

        private static readonly Dictionary<string, string> BaseMapByMaterialName =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Carpet", "carpet" },
                { "stairs", "stairs" },
                { "Water", "Water" },
                { "winedrop", "winedrop" },
                { "Dollar_Red", "Dollar_Red" }
            };

        private enum RemapMode
        {
            DryRun,
            Apply
        }

        private enum SurfaceMode
        {
            Opaque,
            AlphaClip,
            Transparent
        }

        [MenuItem(MenuRoot + "Log Broken Materials")]
        private static void LogBrokenMaterials()
        {
            Run(RemapMode.DryRun);
        }

        [MenuItem(MenuRoot + "Remap Broken Materials To Simple Lit")]
        private static void RemapBrokenMaterials()
        {
            Run(RemapMode.Apply);
        }

        private static void Run(RemapMode mode)
        {
            var target = Shader.Find(TargetShaderName);
            if (target == null)
            {
                Debug.LogError("Shader not found: " + TargetShaderName);
                return;
            }

            var report = new StringBuilder();
            report.AppendLine(mode == RemapMode.Apply ? "Remapping broken materials" : "Broken materials (dry run)");

            int affected = 0;
            int skippedUi = 0;

            foreach (var path in FindMaterialPaths())
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null || IsShaderUsable(material))
                    continue;

                var saved = ReadSavedProperties(material);

                if (IsUiShaderMaterial(saved))
                {
                    skippedUi++;
                    continue;
                }

                report.AppendLine(Describe(material, saved));

                if (mode == RemapMode.Apply)
                    ApplySimpleLit(material, saved, target);

                affected++;
            }

            if (mode == RemapMode.Apply)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            report.AppendLine("materials: " + affected + ", skipped as ui: " + skippedUi);
            Debug.Log(report.ToString());
        }

        private static IEnumerable<string> FindMaterialPaths()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                    yield return path;
            }
        }

        private static bool IsShaderUsable(Material material)
        {
            return material.shader != null && material.shader.isSupported;
        }

        private static bool IsUiShaderMaterial(SavedProperties saved)
        {
            foreach (var marker in UiShaderMarkers)
            {
                if (!saved.Has(marker))
                    return false;
            }

            return true;
        }

        private static void ApplySimpleLit(Material material, SavedProperties saved, Shader target)
        {
            var surface = ResolveSurfaceMode(material, saved);
            bool receiveShadows = !saved.Keywords.Contains("_RECEIVE_SHADOWS_OFF");

            material.shader = target;
            material.shaderKeywords = new string[0];

            ApplyBaseMap(material, saved);
            ApplyBaseColor(material, saved, surface);
            ApplyNormalMap(material, saved);
            ApplyEmission(material, saved);
            ApplySpecular(material, saved);
            ApplySurface(material, saved, surface);

            material.SetFloat("_Cull", saved.GetFloat("_Cull", (float)CullMode.Back));
            material.SetFloat("_ReceiveShadows", receiveShadows ? 1f : 0f);
            if (!receiveShadows)
                material.EnableKeyword("_RECEIVE_SHADOWS_OFF");

            EditorUtility.SetDirty(material);
        }

        private static void ApplyBaseMap(Material material, SavedProperties saved)
        {
            var texture = ResolveBaseMap(material, saved);
            if (texture == null)
                return;

            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", saved.GetTextureScale("_BaseMap", saved.GetTextureScale("_MainTex", Vector2.one)));
            material.SetTextureOffset("_BaseMap", saved.GetTextureOffset("_BaseMap", saved.GetTextureOffset("_MainTex", Vector2.zero)));
        }

        private static Texture ResolveBaseMap(Material material, SavedProperties saved)
        {
            var existing = saved.GetTexture("_BaseMap") ?? saved.GetTexture("_MainTex");
            if (existing != null)
                return existing;

            string textureName;
            if (BaseMapByMaterialName.TryGetValue(material.name, out textureName))
                return LoadTexture(textureName);

            if (material.name.StartsWith(PropsMaterialPrefix, StringComparison.OrdinalIgnoreCase))
                return LoadTexture(PropsAtlasTextureName);

            return null;
        }

        private static void ApplyBaseColor(Material material, SavedProperties saved, SurfaceMode surface)
        {
            var color = saved.GetColor("_BaseColor") ?? saved.GetColor("_Color") ?? Color.white;

            if (surface == SurfaceMode.Transparent && IsForcedTransparent(material))
                color.a = ForcedTransparentAlpha;

            material.SetColor("_BaseColor", color);
        }

        private static void ApplyNormalMap(Material material, SavedProperties saved)
        {
            var bump = saved.GetTexture("_BumpMap");
            if (bump == null)
                return;

            material.SetTexture("_BumpMap", bump);
            material.SetFloat("_BumpScale", saved.GetFloat("_BumpScale", 1f));
            material.EnableKeyword("_NORMALMAP");
        }

        private static void ApplyEmission(Material material, SavedProperties saved)
        {
            bool enabled = saved.Keywords.Contains("_EMISSION") || saved.GetFloat("_UseEmission", 0f) > 0f;
            if (!enabled)
            {
                material.SetColor("_EmissionColor", Color.black);
                material.DisableKeyword("_EMISSION");
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                return;
            }

            var map = saved.GetTexture("_EmissionMap");
            if (map != null)
                material.SetTexture("_EmissionMap", map);

            material.SetColor("_EmissionColor", saved.GetColor("_EmissionColor") ?? Color.black);
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        private static void ApplySpecular(Material material, SavedProperties saved)
        {
            var specularColor = saved.GetColor("_SpecColor") ?? saved.GetColor("_SpecularColor");
            if (specularColor.HasValue)
                material.SetColor("_SpecColor", specularColor.Value);

            var specularMap = saved.GetTexture("_SpecGlossMap");
            if (specularMap == null)
                return;

            material.SetTexture("_SpecGlossMap", specularMap);
            material.EnableKeyword("_SPECGLOSSMAP");
        }

        private static void ApplySurface(Material material, SavedProperties saved, SurfaceMode surface)
        {
            material.SetFloat("_Cutoff", saved.GetFloat("_Cutoff", FallbackCutoff));

            switch (surface)
            {
                case SurfaceMode.AlphaClip:
                    SetRenderState(material, BlendMode.One, BlendMode.Zero, true);
                    material.SetFloat("_Surface", 0f);
                    material.SetFloat("_Blend", 0f);
                    material.SetFloat("_AlphaClip", 1f);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.renderQueue = (int)RenderQueue.AlphaTest;
                    break;

                case SurfaceMode.Transparent:
                    SetRenderState(material, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha, false);
                    material.SetFloat("_Surface", 1f);
                    material.SetFloat("_Blend", 0f);
                    material.SetFloat("_AlphaClip", 0f);
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;

                default:
                    SetRenderState(material, BlendMode.One, BlendMode.Zero, true);
                    material.SetFloat("_Surface", 0f);
                    material.SetFloat("_Blend", 0f);
                    material.SetFloat("_AlphaClip", 0f);
                    material.renderQueue = -1;
                    break;
            }
        }

        private static void SetRenderState(Material material, BlendMode source, BlendMode destination, bool zWrite)
        {
            material.SetFloat("_SrcBlend", (float)source);
            material.SetFloat("_DstBlend", (float)destination);
            material.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            material.SetFloat("_DstBlendAlpha", (float)destination);
            material.SetFloat("_ZWrite", zWrite ? 1f : 0f);
        }

        private static SurfaceMode ResolveSurfaceMode(Material material, SavedProperties saved)
        {
            if (IsForcedTransparent(material))
                return SurfaceMode.Transparent;

            if (saved.Keywords.Contains("_ALPHATEST_ON") || saved.GetFloat("_UseAlphaTest", 0f) > 0f)
                return SurfaceMode.AlphaClip;

            if (saved.Has("_Surface") && saved.GetFloat("_Surface", 0f) > 0f)
                return SurfaceMode.Transparent;

            if (saved.Has("_Mode"))
            {
                float mode = saved.GetFloat("_Mode", 0f);
                if (mode >= 2f)
                    return SurfaceMode.Transparent;

                return mode >= 1f ? SurfaceMode.AlphaClip : SurfaceMode.Opaque;
            }

            bool blends = saved.GetFloat("_SrcBlend", (float)BlendMode.One) != (float)BlendMode.One
                          || saved.GetFloat("_DstBlend", (float)BlendMode.Zero) != (float)BlendMode.Zero;

            return blends ? SurfaceMode.Transparent : SurfaceMode.Opaque;
        }

        private static bool IsForcedTransparent(Material material)
        {
            return string.Equals(material.name, ForcedTransparentMaterialName, StringComparison.OrdinalIgnoreCase);
        }

        private static Texture2D LoadTexture(string textureName)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D " + textureName, new[] { TextureFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(path), textureName, StringComparison.OrdinalIgnoreCase))
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            return null;
        }

        private static string Describe(Material material, SavedProperties saved)
        {
            var color = saved.GetColor("_BaseColor") ?? saved.GetColor("_Color") ?? Color.white;
            var keywords = new List<string>(saved.Keywords);

            return "  " + material.name
                   + " | surface " + ResolveSurfaceMode(material, saved)
                   + " | color " + color.ToString("F2")
                   + " | cull " + saved.GetFloat("_Cull", (float)CullMode.Back)
                   + " | keywords " + (keywords.Count == 0 ? "-" : string.Join(" ", keywords.ToArray()));
        }

        private static SavedProperties ReadSavedProperties(Material material)
        {
            var result = new SavedProperties();
            var serialized = new SerializedObject(material);
            var saved = serialized.FindProperty("m_SavedProperties");

            var textures = saved.FindPropertyRelative("m_TexEnvs");
            for (int i = 0; i < textures.arraySize; i++)
            {
                var entry = textures.GetArrayElementAtIndex(i);
                var name = entry.FindPropertyRelative("first").stringValue;
                var value = entry.FindPropertyRelative("second");

                result.Textures[name] = value.FindPropertyRelative("m_Texture").objectReferenceValue as Texture;
                result.TextureScales[name] = value.FindPropertyRelative("m_Scale").vector2Value;
                result.TextureOffsets[name] = value.FindPropertyRelative("m_Offset").vector2Value;
            }

            var floats = saved.FindPropertyRelative("m_Floats");
            for (int i = 0; i < floats.arraySize; i++)
            {
                var entry = floats.GetArrayElementAtIndex(i);
                result.Floats[entry.FindPropertyRelative("first").stringValue] = entry.FindPropertyRelative("second").floatValue;
            }

            var colors = saved.FindPropertyRelative("m_Colors");
            for (int i = 0; i < colors.arraySize; i++)
            {
                var entry = colors.GetArrayElementAtIndex(i);
                result.Colors[entry.FindPropertyRelative("first").stringValue] = entry.FindPropertyRelative("second").colorValue;
            }

            ReadKeywords(serialized.FindProperty("m_ValidKeywords"), result.Keywords);
            ReadKeywords(serialized.FindProperty("m_InvalidKeywords"), result.Keywords);

            return result;
        }

        private static void ReadKeywords(SerializedProperty property, HashSet<string> target)
        {
            if (property == null)
                return;

            for (int i = 0; i < property.arraySize; i++)
                target.Add(property.GetArrayElementAtIndex(i).stringValue);
        }

        private sealed class SavedProperties
        {
            public readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
            public readonly Dictionary<string, Vector2> TextureScales = new Dictionary<string, Vector2>();
            public readonly Dictionary<string, Vector2> TextureOffsets = new Dictionary<string, Vector2>();
            public readonly Dictionary<string, float> Floats = new Dictionary<string, float>();
            public readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>();
            public readonly HashSet<string> Keywords = new HashSet<string>();

            public bool Has(string name)
            {
                return Textures.ContainsKey(name) || Floats.ContainsKey(name) || Colors.ContainsKey(name);
            }

            public Texture GetTexture(string name)
            {
                Texture value;
                return Textures.TryGetValue(name, out value) ? value : null;
            }

            public Vector2 GetTextureScale(string name, Vector2 fallback)
            {
                Vector2 value;
                return TextureScales.TryGetValue(name, out value) ? value : fallback;
            }

            public Vector2 GetTextureOffset(string name, Vector2 fallback)
            {
                Vector2 value;
                return TextureOffsets.TryGetValue(name, out value) ? value : fallback;
            }

            public float GetFloat(string name, float fallback)
            {
                float value;
                return Floats.TryGetValue(name, out value) ? value : fallback;
            }

            public Color? GetColor(string name)
            {
                Color value;
                return Colors.TryGetValue(name, out value) ? value : (Color?)null;
            }
        }
    }
}
