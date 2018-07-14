using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation {
    public sealed class ExcludeVolume {
        public readonly Vector3 Position;
        public readonly float Radius;

        public ExcludeVolume(Vector3 position, float radius) {
            Position = position;
            Radius = radius;
        }
    }

    public static class NavigationGridBuilder {

        private static readonly float sd = 1f / ((1f + Mathf.Sqrt(5)) / 2f); // side
        private static readonly Vector3[] icosaedronVertices = new Vector3[] {
            new Vector3(-sd, 1f, 0f),
            new Vector3( sd, 1f, 0f),
            new Vector3( sd,-1f, 0f),
            new Vector3(-sd,-1f, 0f),
            new Vector3( 1f, 0f,-sd),
            new Vector3( 1f, 0f, sd),
            new Vector3(-1f, 0f, sd),
            new Vector3(-1f, 0f,-sd),
            new Vector3( 0f,-sd, 1f),
            new Vector3( 0f, sd, 1f),
            new Vector3( 0f, sd,-1f),
            new Vector3( 0f,-sd,-1f)
        };


        private static readonly int[] icosaedronFaces = new int[] {
            0, 1, 9,
            0, 10, 1,
            1, 4, 5,
            2, 5, 4,
            2, 3, 8,
            2, 11, 3,
            3, 7, 6,
            0, 6, 7,
            5, 8, 9,
            6, 9, 8,
            7, 11, 10,
            4, 10, 11,

            1, 5, 9,
            0, 9, 6,
            0, 7, 10,
            1, 10, 4,

            2, 8, 5,
            3, 6, 8,
            3, 11, 7,
            2, 11, 4
        };

        private sealed class NavigationBuildPoint {
            public int Id;
            public bool IsBorderPoint;
            public Vector3 Position;
            public int PositionHash;
            public List<NavigationBuildPoint> Neigbhors = new List<NavigationBuildPoint>();
            public List<float> Distances = new List<float>();
        }

        public static NavigationGraph Generate(float radius, int size, List<ExcludeVolume> excludeVolumes = null) {
            excludeVolumes = excludeVolumes ?? new List<ExcludeVolume>();
            var pointsList = GenerateNavGridIcosaedron(radius, size);
            ExcludePathPoins(pointsList, excludeVolumes);
            return new NavigationGraph(FinalizeGraph(pointsList.ToArray()));
        }

        private static List<NavigationBuildPoint> GenerateNavGridIcosaedron(float radius, int size) {
            List<NavigationBuildPoint> points = new List<NavigationBuildPoint>();
            List<NavigationBuildPoint> borderPoins = new List<NavigationBuildPoint>();
            NavigationBuildPoint[] grid = new NavigationBuildPoint[size * size];
            for (int d = 0; d < icosaedronFaces.Length / 3; d++) {
                var A = icosaedronVertices[icosaedronFaces[d * 3 + 0]];
                var B = icosaedronVertices[icosaedronFaces[d * 3 + 1]];
                var C = icosaedronVertices[icosaedronFaces[d * 3 + 2]];
                var BA = B - A;
                var CA = C - A;
                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < size; j++) {
                        if (i + j >= size)
                            continue;
                        Vector3 cellPosition = A + BA * (i / (float)(size - 1)) + CA * (j / (float)(size - 1));
                        Vector3 normal = cellPosition.normalized;
                        Vector3 cell = (normal + Vector3.one * 2f) * size * 10f;
                        bool isBorderPoint = (i == 0 || j == 0 || i + j == size - 1);
                        var point = new NavigationBuildPoint() {
                            Position = normal * radius,
                            IsBorderPoint = isBorderPoint,
                            PositionHash = (int)(cell.x * (size + 2) * (size + 2)) + (int)(cell.y * (size + 2)) + (int)(cell.z)
                        };
                        if (isBorderPoint)
                            borderPoins.Add(point);
                        points.Add(point);
                        grid[i * size + j] = point;
                    }
                }
                // add links
                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < size; j++) {
                        if (i + j >= size)
                            continue;
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i + 1, j);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i - 1, j);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i, j + 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i, j - 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i - 1, j + 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i + 1, j - 1);

                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i + 1, j + 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i - 1, j - 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i - 1, j + 2);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i - 2, j + 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i + 2, j - 1);
                        AddLinksHex(ref grid, ref grid[i * size + j].Neigbhors, size, i + 1, j - 2);
                    }
                }
            }
            // merge borders
            foreach (var point in borderPoins) {
                if (point.Neigbhors == null)
                    continue;
                foreach (var merge in borderPoins) {
                    if (merge == point)
                        continue;
                    if (merge.Neigbhors == null)
                        continue;
                    if (merge.PositionHash == point.PositionHash) {
                        MergePoints(point, merge);
                        break;
                    }
                }
            }
            points.RemoveAll(point => point.Neigbhors == null);
            foreach (var point in points) {
                point.Distances = new List<float>(point.Neigbhors.Count);
                for (int i = 0; i < point.Neigbhors.Count; i++) {
                    var distance = Vector3.Distance(point.Position, point.Neigbhors[i].Position);
                    point.Distances.Add(distance);
                }
            }
            return points;
        }

        private static void AddLinksHex(ref NavigationBuildPoint[] grid, ref List<NavigationBuildPoint> neigbhors, int size, int x, int y) {
            if (x < 0)
                return;
            if (y < 0)
                return;
            if (x + y >= size)
                return;
            neigbhors.Add(grid[x * size + y]);
        }

        private static void MergePoints(NavigationBuildPoint point, NavigationBuildPoint merge) {
            foreach (var neigbhor in merge.Neigbhors) {
                if (neigbhor.Neigbhors == null)
                    continue;
                for (int i = 0; i < neigbhor.Neigbhors.Count; i++) {
                    if (neigbhor.Neigbhors[i] == merge) {
                        neigbhor.Neigbhors[i] = point;
                    }
                }
            }
            point.Neigbhors.AddRange(merge.Neigbhors);
            merge.Neigbhors = null;
        }

        private static void ExcludePathPoins(List<NavigationBuildPoint> points, List<ExcludeVolume> excludeVolumes) {
            foreach (var point in points) {
                if (point.Neigbhors == null)
                    continue;
                foreach (var volume in excludeVolumes) {
                    if (Vector3.Distance(point.Position, volume.Position) <= volume.Radius) {
                        foreach (var neigbhor in point.Neigbhors) {
                            neigbhor.Neigbhors.Remove(point);
                        }
                        point.Neigbhors = null;
                        break;
                    }
                }
            }
            points.RemoveAll(point => point.Neigbhors == null);
        }

        private static NavigationGraph.Node[] FinalizeGraph(NavigationBuildPoint[] points) {
            var nodes = new NavigationGraph.Node[points.Length];
            for (int i = 0; i < points.Length; i++) {
                points[i].Id = i;
            }
            for (int i = 0; i < points.Length; i++) {
                nodes[i] = new NavigationGraph.Node() {
                    Position = points[i].Position,
                    Neigbhors = points[i].Neigbhors.Select(Neibghor => Neibghor.Id).ToArray()
                };
            }
            return nodes;
        }
    }
}