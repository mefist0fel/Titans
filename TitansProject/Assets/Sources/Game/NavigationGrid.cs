using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class NavigationGrid : MonoBehaviour {
    [SerializeField]
    public float radius = 10;
    
    public struct NavigationPoint {
        public Vector3 Position;
        public Vector3 UpNormal;
        public int[] Neigbhors;
    }

    private NavigationPoint[] points;

	private void Start () {
        points = GenerateNavGrid(radius);
	}

    private NavigationPoint[] GenerateNavGrid(float radius) {
        List<NavigationPoint> grid = new List<NavigationPoint>();
        // top
        const int density = 20;
        for (int i = 0; i < density; i++) {
            for (int j = 0; j < density; j++) {
                Vector3 normale = new Vector3(-1 + 2f * (i / (float) density), -1 + 2f * (i / (float)density), 1).normalized;
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
