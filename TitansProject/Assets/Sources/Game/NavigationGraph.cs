using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NavigationGraph {
    public struct Node {
        public Vector3 Position;
        public int[] Neigbhors;
    }

    public readonly Node[] Nodes;
    private readonly float[][] distanceToNeigbhors;
    private readonly int[] cameFromId;
    private readonly float[] distanceToId;
    private readonly float[] cachedHeuristicDistancesToTarget;
    private readonly PriorityQueueCustom<int, float> frontier = new PriorityQueueCustom<int, float>();

    public NavigationGraph(Node[] graphNodes) {
        Nodes = graphNodes;
        distanceToNeigbhors = CalculateDistances(Nodes);
        cameFromId = new int[Nodes.Length];
        distanceToId = new float[Nodes.Length];
        cachedHeuristicDistancesToTarget = new float[Nodes.Length];
    }

    private float[][] CalculateDistances(Node[] nodes) {
        var nodeDistances = new float[nodes.Length][];
        for (int i = 0; i < nodes.Length; i++) {
            var neigbhors = nodes[i].Neigbhors;
            var neigbhorDistances = new float[neigbhors.Length];
            for (int n = 0; n < neigbhors.Length; n++) {
                neigbhorDistances[n] = Vector3.Distance(nodes[i].Position, nodes[neigbhors[n]].Position);
            }
            nodeDistances[i] = neigbhorDistances;
        }
        return nodeDistances;
    }

    public List<Vector3> FindPath(int startNodeId, int endNodeId) {

        var watch = System.Diagnostics.Stopwatch.StartNew();
        float count = 10;
        for (int i = 0; i < count; i++) {
            FindPathDijkstra(startNodeId, endNodeId);
        }
        watch.Stop();
        Debug.Log("Find path Dijkstra " + count + " times for " + (watch.ElapsedMilliseconds / 1000f));
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < count; i++) {
            FindPathAStarOld(startNodeId, endNodeId);
        }
        watch.Stop();
        Debug.Log("Find path AStar " + count + " times for " + (watch.ElapsedMilliseconds / 1000f));
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < count; i++) {
            FindPathAStar(startNodeId, endNodeId);
        }
        watch.Stop();
        Debug.Log("Find path AStar* " + count + " times for " + (watch.ElapsedMilliseconds / 1000f));

        return FindPathAStar(startNodeId, endNodeId);
    }

    public List<Vector3> FindPathDijkstra(int startNodeId, int endNodeId) {
        frontier.Clear();
        frontier.Enqueue(startNodeId, 0);
        cameFromId[startNodeId] = -1;
        int currentId = 0;


        for (int j = 0; j < distanceToId.Length; j++) {
            distanceToId[j] = float.MaxValue;
        }
        distanceToId[startNodeId] = 0;
        int i;
        float distance;
        int neigbhorId;
        while (frontier.Count > 0) {
            currentId = frontier.Dequeue();

            if (currentId == endNodeId) {
                break;
            }
            for (i = 0; i < Nodes[currentId].Neigbhors.Length; i++) {
                distance = distanceToId[currentId] + distanceToNeigbhors[currentId][i];
                neigbhorId = Nodes[currentId].Neigbhors[i];
                if (distance < distanceToId[neigbhorId]) {
                    frontier.Enqueue(neigbhorId, distance);
                    cameFromId[neigbhorId] = currentId;
                    distanceToId[neigbhorId] = distance;
                }
            }
        }
        var path = new List<Vector3>();
        if (currentId == endNodeId) {
            path.Add(Nodes[currentId].Position);
            while (currentId != startNodeId) {
                currentId = cameFromId[currentId];
                path.Add(Nodes[currentId].Position);
            }
            path.Add(Nodes[startNodeId].Position);
        }
        return path;
    }

    public List<Vector3> FindPathAStar(int startNodeId, int endNodeId) {
        frontier.Clear();
        frontier.Enqueue(startNodeId, 0);
        cameFromId[startNodeId] = -1;
        int currentId = 0;


        for (int j = 0; j < distanceToId.Length; j++) {
            distanceToId[j] = float.MaxValue;
        }
        distanceToId[startNodeId] = 0;
        int i;
        float distance;
        int neigbhorId;
        while (frontier.Count > 0) {
            currentId = frontier.Dequeue();

            if (currentId == endNodeId) {
                break;
            }
            for (i = 0; i < Nodes[currentId].Neigbhors.Length; i++) {
                distance = distanceToId[currentId] + distanceToNeigbhors[currentId][i];
                neigbhorId = Nodes[currentId].Neigbhors[i];
                if (distance < distanceToId[neigbhorId]) {
                    if (distanceToId[neigbhorId] == float.MaxValue)
                        cachedHeuristicDistancesToTarget[neigbhorId] = HeuristicDistance(Nodes[neigbhorId], Nodes[endNodeId]);
                    frontier.Enqueue(neigbhorId, distance + cachedHeuristicDistancesToTarget[neigbhorId]);
                    cameFromId[neigbhorId] = currentId;
                    distanceToId[neigbhorId] = distance;
                }
            }
        }
        var path = new List<Vector3>();
        if (currentId == endNodeId) {
            path.Add(Nodes[currentId].Position);
            while (currentId != startNodeId) {
                currentId = cameFromId[currentId];
                path.Add(Nodes[currentId].Position);
            }
            path.Add(Nodes[startNodeId].Position);
        }
        return path;
    }

    public List<Vector3> FindPathAStarOld(int startNodeId, int endNodeId) {
        frontier.Clear();
        frontier.Enqueue(startNodeId, 0);
        cameFromId[startNodeId] = -1;
        int currentId = 0;


        for (int j = 0; j < distanceToId.Length; j++) {
            distanceToId[j] = float.MaxValue;
        }
        distanceToId[startNodeId] = 0;
        int i;
        float distance;
        int neigbhorId;
        while (frontier.Count > 0) {
            currentId = frontier.Dequeue();

            if (currentId == endNodeId) {
                break;
            }
            for (i = 0; i < Nodes[currentId].Neigbhors.Length; i++) {
                distance = distanceToId[currentId] + distanceToNeigbhors[currentId][i];
                neigbhorId = Nodes[currentId].Neigbhors[i];
                if (distance < distanceToId[neigbhorId]) {
                    frontier.Enqueue(neigbhorId, distance + HeuristicDistance(Nodes[neigbhorId], Nodes[endNodeId]));
                    cameFromId[neigbhorId] = currentId;
                    distanceToId[neigbhorId] = distance;
                }
            }
        }
        var path = new List<Vector3>();
        if (currentId == endNodeId) {
            path.Add(Nodes[currentId].Position);
            while (currentId != startNodeId) {
                currentId = cameFromId[currentId];
                path.Add(Nodes[currentId].Position);
            }
            path.Add(Nodes[startNodeId].Position);
        }
        return path;
    }

    private float HeuristicDistance(Node nodeFrom, Node nodeTo) {
        // var delta = nodeFrom.Position - nodeTo.Position;
        // return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) + Mathf.Abs(delta.z);
        // return Vector3.SqrMagnitude(nodeFrom.Position - nodeTo.Position);
        return Vector3.Distance(nodeFrom.Position, nodeTo.Position);
    }
}
