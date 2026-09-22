using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace RunRich.Editor
{
    public static class RoadMeshBuilder
    {
        private const string MenuPath = "Tools/RunRich/Level/Bake Road Mesh";
        private const string OutputFolder = "Assets/_Project/Levels";
        private const string MeshAssetName = "Level_01_Road";
        private const float HalfWidth = 2.9f;
        private const float SampleStep = 0.5f;
        private const float SurfaceOffset = -0.02f;
        private const float TileLength = 8f;

        [MenuItem(MenuPath, true)]
        private static bool CanBake()
        {
            return Selection.activeGameObject != null
                && Selection.activeGameObject.GetComponent<SplineContainer>() != null;
        }

        [MenuItem(MenuPath)]
        private static void Bake()
        {
            var container = Selection.activeGameObject.GetComponent<SplineContainer>();
            var mesh = Build(container.Spline);

            if (!AssetDatabase.IsValidFolder(OutputFolder))
                AssetDatabase.CreateFolder("Assets/_Project", "Levels");

            var path = OutputFolder + "/" + MeshAssetName + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null)
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
            else
            {
                existing.Clear();
                existing.SetVertices(new List<Vector3>(mesh.vertices));
                existing.SetUVs(0, new List<Vector2>(mesh.uv));
                existing.SetTriangles(mesh.triangles, 0);
                existing.RecalculateNormals();
                existing.RecalculateBounds();
                EditorUtility.SetDirty(existing);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Road mesh baked: " + path + ", vertices " + mesh.vertexCount);
        }

        private static Mesh Build(Spline spline)
        {
            float length = spline.GetLength();
            int segments = Mathf.Max(1, Mathf.CeilToInt(length / SampleStep));
            int samples = segments + 1;

            var vertices = new Vector3[samples * 2];
            var uv = new Vector2[samples * 2];
            var triangles = new int[segments * 6];

            for (int i = 0; i < samples; i++)
            {
                float distance = Mathf.Min(i * SampleStep, length);
                float t = SplineUtility.GetNormalizedInterpolation(spline, distance, PathIndexUnit.Distance);
                SplineUtility.Evaluate(spline, t, out float3 position, out float3 tangent, out float3 upVector);

                Vector3 forward = ((Vector3)tangent).normalized;
                Vector3 up = ((Vector3)upVector).normalized;
                Vector3 right = Vector3.Cross(up, forward).normalized;
                Vector3 center = (Vector3)position + up * SurfaceOffset;

                vertices[i * 2] = center - right * HalfWidth;
                vertices[i * 2 + 1] = center + right * HalfWidth;

                float v = distance / TileLength;
                uv[i * 2] = new Vector2(0f, v);
                uv[i * 2 + 1] = new Vector2(1f, v);
            }

            for (int i = 0; i < segments; i++)
            {
                int vertex = i * 2;
                int index = i * 6;

                triangles[index] = vertex;
                triangles[index + 1] = vertex + 2;
                triangles[index + 2] = vertex + 1;
                triangles[index + 3] = vertex + 1;
                triangles[index + 4] = vertex + 2;
                triangles[index + 5] = vertex + 3;
            }

            var mesh = new Mesh { name = MeshAssetName };
            mesh.indexFormat = samples * 2 > 65000
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
