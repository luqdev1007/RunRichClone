using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace RunRich.Editor
{
    public static class RoadRebuilder
    {
        private const string MenuPath = "Tools/RunRich/Level/Rebuild Road With Corners";
        private const string LevelPrefabPath = "Assets/_Project/Prefabs/Level_01.prefab";
        private const string PlaneMeshPath = "Assets/_Project/Art/Visual/Mesh/Plane.asset";

        private const float CornerRadius = 6f;
        private const float ArcHandle = 3.3137085f;
        private const float StraightOne = 20.3f;
        private const float StraightTwo = 38.6f;
        private const float StraightThree = 46.4f;
        private const int NearestSamples = 4000;
        private const float WaterMargin = 60f;

        private static readonly string[] ReprojectedGroups = { "Pickups", "Gates", "FlagZones" };

        private struct Placement
        {
            public Transform Target;
            public float Distance;
            public float Lateral;
            public float Height;
        }

        [MenuItem(MenuPath)]
        private static void Rebuild()
        {
            var root = PrefabUtility.LoadPrefabContents(LevelPrefabPath);
            try
            {
                var container = root.GetComponentInChildren<SplineContainer>(true);
                var placements = Capture(root, container);
                WriteCorneredShape(container.Spline);
                Reproject(container, placements);
                RoadMeshBuilder.Bake(container);
                FitWater(root, container);
                PrefabUtility.SaveAsPrefabAsset(root, LevelPrefabPath);

                Debug.Log("Road rebuilt: length " + container.Spline.GetLength().ToString("F2")
                    + ", reprojected " + placements.Count + " objects");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static List<Transform> CollectTargets(GameObject root)
        {
            var targets = new List<Transform>();

            foreach (var group in ReprojectedGroups)
            {
                var parent = root.transform.Find(group);
                if (parent == null)
                    continue;

                foreach (Transform child in parent)
                    targets.Add(child);
            }

            var corridor = root.transform.Find("FinishCorridor");
            if (corridor != null)
            {
                foreach (Transform child in corridor)
                {
                    if (child.name.StartsWith("FinishDoor"))
                        targets.Add(child);
                }
            }

            var spawn = root.transform.Find("PlayerSpawn");
            if (spawn != null)
                targets.Add(spawn);

            return targets;
        }

        private static List<Placement> Capture(GameObject root, SplineContainer container)
        {
            var spline = container.Spline;
            float length = spline.GetLength();
            var placements = new List<Placement>();

            foreach (var target in CollectTargets(root))
            {
                float bestDistance = 0f;
                float bestSqr = float.MaxValue;

                for (int i = 0; i <= NearestSamples; i++)
                {
                    float distance = length * i / NearestSamples;
                    Evaluate(spline, distance, out var sample, out var sampleForward, out var sampleUp);
                    float sqr = (sample - target.position).sqrMagnitude;
                    if (sqr < bestSqr)
                    {
                        bestSqr = sqr;
                        bestDistance = distance;
                    }
                }

                Evaluate(spline, bestDistance, out var onRoad, out var forward, out var up);
                Vector3 right = Vector3.Cross(up, forward).normalized;
                Vector3 offset = target.position - onRoad;

                placements.Add(new Placement
                {
                    Target = target,
                    Distance = bestDistance,
                    Lateral = Vector3.Dot(offset, right),
                    Height = offset.y
                });
            }

            return placements;
        }

        private static void Reproject(SplineContainer container, List<Placement> placements)
        {
            var spline = container.Spline;

            foreach (var placement in placements)
            {
                Evaluate(spline, placement.Distance, out var position, out var forward, out var up);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                placement.Target.SetPositionAndRotation(
                    position + right * placement.Lateral + Vector3.up * placement.Height,
                    Quaternion.LookRotation(forward, up));
            }
        }

        private static void Evaluate(Spline spline, float distance, out Vector3 position,
            out Vector3 forward, out Vector3 up)
        {
            float t = SplineUtility.GetNormalizedInterpolation(spline, distance, PathIndexUnit.Distance);
            SplineUtility.Evaluate(spline, t, out float3 rawPosition, out float3 rawTangent, out float3 rawUp);
            position = rawPosition;
            forward = ((Vector3)rawTangent).normalized;
            up = ((Vector3)rawUp).normalized;
        }

        private static void WriteCorneredShape(Spline spline)
        {
            spline.Clear();

            float firstCornerZ = StraightOne;
            float secondCornerX = CornerRadius + StraightTwo;
            float secondCornerZ = firstCornerZ + CornerRadius;
            float endZ = secondCornerZ + CornerRadius + StraightThree;

            Add(spline, new Vector3(0f, 0f, 0f),
                Vector3.forward, StraightOne / 3f, Vector3.forward, StraightOne / 3f);

            Add(spline, new Vector3(0f, 0f, firstCornerZ),
                Vector3.forward, StraightOne / 3f, Vector3.forward, ArcHandle);

            Add(spline, new Vector3(CornerRadius, 0f, firstCornerZ + CornerRadius),
                Vector3.right, ArcHandle, Vector3.right, StraightTwo / 3f);

            Add(spline, new Vector3(secondCornerX, 0f, secondCornerZ),
                Vector3.right, StraightTwo / 3f, Vector3.right, ArcHandle);

            Add(spline, new Vector3(secondCornerX + CornerRadius, 0f, secondCornerZ + CornerRadius),
                Vector3.forward, ArcHandle, Vector3.forward, StraightThree / 3f);

            Add(spline, new Vector3(secondCornerX + CornerRadius, 0f, endZ),
                Vector3.forward, StraightThree / 3f, Vector3.forward, StraightThree / 3f);
        }

        private static void Add(Spline spline, Vector3 position,
            Vector3 inDirection, float inLength, Vector3 outDirection, float outLength)
        {
            spline.Add(new BezierKnot(position,
                -(float3)(inDirection * inLength),
                (float3)(outDirection * outLength),
                quaternion.identity));
        }

        private static void FitWater(GameObject root, SplineContainer container)
        {
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(PlaneMeshPath);
            if (mesh == null)
                return;

            var spline = container.Spline;
            float length = spline.GetLength();
            var min = new Vector3(float.MaxValue, 0f, float.MaxValue);
            var max = new Vector3(float.MinValue, 0f, float.MinValue);

            for (int i = 0; i <= NearestSamples; i++)
            {
                Evaluate(spline, length * i / NearestSamples, out var position, out var forward, out var up);
                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min + Vector3.one * (WaterMargin * 2f);
            Vector3 meshCenter = mesh.bounds.center;

            foreach (var name in new[] { "Water", "WaterBase" })
            {
                var plane = root.transform.Find(name);
                if (plane == null)
                    continue;

                var scale = new Vector3(size.x / mesh.bounds.size.x, 1f, size.z / mesh.bounds.size.z);
                plane.localScale = scale;
                plane.position = new Vector3(
                    center.x - meshCenter.x * scale.x,
                    plane.position.y,
                    center.z - meshCenter.z * scale.z);
            }
        }
    }
}
