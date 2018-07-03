using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public sealed class NavigationGrid : MonoBehaviour {
    [SerializeField]
    public float radius = 10;
    [SerializeField]
    public int size = 50;

    public class NavigationPoint {
        public Vector3 Position;
        public Vector3 UpNormal;
        public int[] Neigbhors;
    }

    private sealed class NavigationBuildPoint {
        public int Id;
        public bool IsBorderPoint;
        public Vector3 Position;
        public Vector3 UpNormal;
        public int PositionHash;
        public List<NavigationBuildPoint> Neigbhors = new List<NavigationBuildPoint>();
    }

    private NavigationPoint[] points;
    private NavigationBuildPoint[] buildPoins;

    [ContextMenu("Regenerate grid")]
	private void Start () {
        points = GenerateNavGrid(radius);
	}

    private NavigationPoint[] GenerateNavGrid(float radius) {
        var directions = new Quaternion[] {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 270, 0),
            Quaternion.Euler(90, 0, 0),
            Quaternion.Euler(-90, 0, 0),
        };
        List<NavigationBuildPoint> points = new List<NavigationBuildPoint>();
        List<NavigationBuildPoint> borderPoins = new List<NavigationBuildPoint>();
        NavigationBuildPoint[] grid = new NavigationBuildPoint[size * size];
        for (int d = 0; d < directions.Length; d++) {
            var direction = directions[d];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Vector3 cellPosition = direction * new Vector3(-0.5f + 1f * (i / (float)(size - 1)), -0.5f + 1f * (j / (float)(size - 1)), 0.5f);
                    Vector3 normal = cellPosition.normalized;
                    Vector3 cell = (cellPosition + Vector3.one) * size;
                    bool isBorderPoint = (i == 0 || j == 0 || i == size - 1 || j == size - 1);
                    var point = new NavigationBuildPoint() {
                        Position = normal * radius,
                        UpNormal = normal,
                        IsBorderPoint = isBorderPoint,
                        PositionHash = (int)cell.x * size * size + (int)cell.y * size + (int)cell.z
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
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i + 1, j);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i, j + 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i - 1, j);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i, j - 1);
                }
            }
        }
        // merge borders
        foreach (var point in borderPoins) {
            if (point.Neigbhors == null)
                continue;
            foreach (var merge in borderPoins) {
                if (merge.Neigbhors == null)
                    continue;
                if (merge.PositionHash == point.PositionHash) {
                    MergePoints(point, merge);
                    break;
                }
            }
        }
        Debug.Log("before " + points.Count);
        points.RemoveAll(point => point.Neigbhors == null);
        Debug.Log("after " + points.Count);
        buildPoins = points.ToArray();
        // merge borders
        // var grid = new NavigationPoint[points.Length];
        // for (int i = 0; i < points.Length; i++) {
        //     grid[i] = new NavigationPoint() { Position = points[i], UpNormal = normals[i], Neigbhors = neigbhors[i].ToArray()};
        // }
        return null;
    }

    private void MergePoints(NavigationBuildPoint point, NavigationBuildPoint merge) {
        point.Neigbhors.AddRange(merge.Neigbhors);
        foreach (var neigbhor in merge.Neigbhors) {
            if (neigbhor.Neigbhors == null)
                continue;
            for (int i = 0; i < neigbhor.Neigbhors.Count; i++) {
                if (neigbhor.Neigbhors[i] == merge) {
                    neigbhor.Neigbhors[i] = point;
                }
            }
        }
        merge.Neigbhors = null;
    }

    private void AddLinks(ref NavigationBuildPoint[] grid, ref List<NavigationBuildPoint> neigbhors, int x, int y) {
        if (x < 0)
            return;
        if (y < 0)
            return;
        if (x >= size)
            return;
        if (y >= size)
            return;
        neigbhors.Add(grid[x * size + y]);
    }

    private void Update () {}

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (buildPoins != null) {
            foreach (var point in buildPoins) {
                Gizmos.DrawWireSphere(point.Position, 0.02f);
                if (point.Neigbhors != null) {
                    foreach (var neigbhor in point.Neigbhors) {
                        Gizmos.DrawLine(point.Position, neigbhor.Position);
                    }
                }
            }
        }
        if (points == null)
            return;
        foreach (var point in points) {
            Gizmos.DrawWireSphere(point.Position, 0.02f);
            // Gizmos.DrawLine(point.Position, point.Position + point.UpNormal * 0.1f);
            if (point.Neigbhors != null) {
                foreach (var neigbhor in point.Neigbhors) {
                    Gizmos.DrawLine(point.Position, points[neigbhor].Position);
                }
            }
        }
    }
#endif
}
