using System.Collections.Generic;
using UnityEngine;

namespace Navigation {
    public sealed class NavigationGraph {
        public struct Node {
            public Vector3 Position;
            public int[] Neigbhors;
        }

        public readonly Node[] Nodes;
        private readonly float[][] distanceToNeigbhors;
        private readonly int[] cameFromId;
        private readonly float[] distanceToId;
        private readonly PriorityQueueCustom<int, float> frontier = new PriorityQueueCustom<int, float>();

        public NavigationGraph(Node[] graphNodes) {
            Nodes = graphNodes;
            distanceToNeigbhors = CalculateDistances(Nodes);
            cameFromId = new int[Nodes.Length];
            distanceToId = new float[Nodes.Length];
        }

        public int FindNearestId(Vector3 point) {
            return FindNearestIdBruteForce(point);
        }

        public int FindNearestIdBruteForce(Vector3 point) {
            int nearestId = 0;
            float nearestDistance = float.MaxValue;
            float distance;
            for (int i = 1; i < Nodes.Length; i++) {
                distance = Vector3.Distance(point, Nodes[i].Position);
                if (distance < nearestDistance) {
                    nearestDistance = distance;
                    nearestId = i;
                }
            }
            return nearestId;
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
            }
            return path;
        }

        private float HeuristicDistance(Node nodeFrom, Node nodeTo) {
            return Vector3.Distance(nodeFrom.Position, nodeTo.Position);
        }
    }
}