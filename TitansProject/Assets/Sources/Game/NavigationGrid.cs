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
    [SerializeField]
    public Transform startObject; // Set from editor
    [SerializeField]
    public Transform endObject; // Set from editor
    [SerializeField]
    public Camera hitCamera; // Set from editor
    [SerializeField]
    public Collider baseCollider; // Set from editor

    private sealed class NavigationBuildPoint {
        public int Id;
        public bool IsBorderPoint;
        public Vector3 Position;
        public Vector3 UpNormal;
        public int PositionHash;
        public List<NavigationBuildPoint> Neigbhors = new List<NavigationBuildPoint>();
        public List<float> Distances = new List<float>();
    }

    private NavigationBuildPoint[] points;

    private NavigationBuildPoint startPoint;
    private NavigationBuildPoint endPoint;
    private List<Vector3> path = new List<Vector3>();

    private NavigationGraph graph;

    [ContextMenu("Regenerate grid")]
	private void Start () {
        points = GenerateNavGrid(radius);
        graph = new NavigationGraph(FinalizeGraph(points));

    }

    private NavigationGraph.Node[] FinalizeGraph(NavigationBuildPoint[] points) {
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

    private NavigationBuildPoint[] GenerateNavGrid(float radius) {
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
                        PositionHash = (int)(cell.x * (size + 1)) * (size + 1) + (int)(cell.y * (size + 1)) + (int)cell.z
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
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i + 1, j    );
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i + 1, j + 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i    , j + 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i - 1, j + 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i - 1, j);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i - 1, j - 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i    , j - 1);
                    AddLinks(ref grid, ref grid[i * size + j].Neigbhors, i + 1, j - 1);
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
        return points.ToArray();
    }

    private void MergePoints(NavigationBuildPoint point, NavigationBuildPoint merge) {
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

    private void Update () {
        if (Input.GetMouseButtonDown(0)) {
            var ray = hitCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (baseCollider.Raycast(ray, out hit, 20)) {
                NavigationBuildPoint point = GetNearest(hit.point);
                startObject.position = point.Position;
                startPoint = point;
                TryFindPath();
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            var ray = hitCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (baseCollider.Raycast(ray, out hit, 20)) {
                NavigationBuildPoint point = GetNearest(hit.point);
                endObject.position = point.Position;
                endPoint = point;
                TryFindPath();
            }
        }
    }

    private NavigationBuildPoint GetNearest(Vector3 position) {
        NavigationBuildPoint nearest = null;
        float minDistance = float.MaxValue;
        foreach (var point in points) {
            var distance = Vector3.Distance(point.Position, position);
            if (distance > minDistance)
                continue;
            nearest = point;
            minDistance = distance;
        }
        return nearest;
    }

    private void TryFindPath() {
        if (startPoint == null)
            return;
        if (endPoint == null)
            return;
        //FindPath(startPoint, endPoint);
        path = graph.FindPath(startPoint.Id, endPoint.Id);
    }

    private void FindPath(NavigationBuildPoint startPoint, NavigationBuildPoint endPoint) {
        Queue<NavigationBuildPoint> frontier = new Queue<NavigationBuildPoint>();
        Dictionary<NavigationBuildPoint, NavigationBuildPoint> cameFrom = new Dictionary<NavigationBuildPoint, NavigationBuildPoint>();
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, null);
        NavigationBuildPoint current = null;

        while (frontier.Count > 0) {
            current = frontier.Dequeue();

            if (current == endPoint)
                break;
            foreach (var neigbhor in current.Neigbhors) {
                if (!cameFrom.ContainsKey(neigbhor)) {
                    frontier.Enqueue(neigbhor);
                    cameFrom.Add(neigbhor, current);
                }
            }
        }
        if (current == endPoint) {
            path.Clear();
            path.Add(endPoint.Position);
            while (current != startPoint) {
                current = cameFrom[current];
                path.Add(current.Position);
            }
            path.Add(startPoint.Position);
            Debug.LogError("ended " + path.Count);
        }
    }

#if UNITY_EDITOR
    [SerializeField]
    private bool showPath = true;
    [SerializeField]
    private bool showLines = true;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (points != null) {
            foreach (var point in points) {
                Gizmos.DrawWireSphere(point.Position, 0.02f);
                if (point.Neigbhors != null && showLines) {
                    foreach (var neigbhor in point.Neigbhors) {
                        Gizmos.DrawLine(point.Position, neigbhor.Position);
                    }
                }
            }
        }
        Gizmos.color = Color.red;
        // show path
        if (path != null) {
            for (int i = 0; i < path.Count - 1; i++) {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
#endif
}
