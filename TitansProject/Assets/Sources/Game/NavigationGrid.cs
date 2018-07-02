using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public sealed class NavigationGrid : MonoBehaviour {
    [SerializeField]
    public float radius = 10;
    
    public struct NavigationPoint {
        public Vector3 Position;
        public Vector3 UpNormal;
        public int[] Neigbhors;
    }

    private NavigationPoint[] points;

    [ContextMenu("Regenerate grid")]
	private void Start () {
        points = GenerateNavGrid(radius);
	}

    private NavigationPoint[] GenerateNavGrid(float radius) {
        List<NavigationPoint> grid = new List<NavigationPoint>();
        var directions = new Quaternion[] {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 270, 0),
            Quaternion.Euler(90, 0, 0),
            Quaternion.Euler(-90, 0, 0),
        };
        // top
        const int density = 20;
        for (int i = 0; i < density; i++) {
            for (int j = 0; j < density; j++) {
                foreach (var direction in directions) {
                    Vector3 normal = direction * new Vector3(-1 + 2f * ((i + 0.5f) / (float)density), -1 + 2f * ((j + 0.5f) / (float)density), 1).normalized;
                    grid.Add(new NavigationPoint() { Position = normal * radius, UpNormal = normal });
                }
            }
        }
        return grid.ToArray();
    }

    private void Update () {
		
	}
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (points == null)
            return;
        Gizmos.color = Color.blue;
        foreach (var point in points) {
            Gizmos.DrawWireSphere(point.Position, 0.1f);
            Gizmos.DrawLine(point.Position, point.Position + point.UpNormal);
        }
    }
#endif
}
