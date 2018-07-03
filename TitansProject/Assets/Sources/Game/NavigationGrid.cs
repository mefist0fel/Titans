using System;
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
        public Vector3 Position;
        public Vector3 UpNormal;
        public List<NavigationBuildPoint> Neigbhors = new List<NavigationBuildPoint>();
    }

    private NavigationPoint[] points;

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
        NavigationBuildPoint[] grid = new NavigationBuildPoint[size * size];
        for (int d = 0; d < directions.Length; d++) {
            var direction = directions[d];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Vector3 normal = direction * new Vector3(-1 + 2f * (i / (float)(size - 1)), -1 + 2f * (j / (float)(size - 1)), 1).normalized;
                    var point = new NavigationBuildPoint() {
                        Position = normal * radius,
                        UpNormal = normal
                    };
                    points.Add(point);
                    grid[i * size + j] = point;
                }
            }
            // add links
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    AddLinks(ref grid, grid[i * size + j].Neigbhors, i, j);
                }
            }
        }
        // merge borders
        // var grid = new NavigationPoint[points.Length];
        // for (int i = 0; i < points.Length; i++) {
        //     grid[i] = new NavigationPoint() { Position = points[i], UpNormal = normals[i], Neigbhors = neigbhors[i].ToArray()};
        // }
        return null;
    }

    private void AddLinks(ref NavigationBuildPoint[] grid, List<NavigationBuildPoint> neigbhors, int x, int y) {
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
        if (points == null)
            return;
        Gizmos.color = Color.blue;
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
