﻿using Model;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

namespace View {
    public sealed class PlanetView : MonoBehaviour {
        [SerializeField]
        private Planet planet;
        [SerializeField]
        private SphereMesh sphereMesh; // Set from editor
        [SerializeField]
        private SphereCollider planetCollider; // Set from editor
        [SerializeField]
        private ResourcePointView resourcePointViewPrefab; // Set from editor

        public Planet Planet { get { return planet; } }

        private List<ResourcePointView> resourcePoints = new List<ResourcePointView>();

        public void Init(Planet controlPlanet) {
            planet = controlPlanet;
            sphereMesh.Regenerate(planet.Radius);
            planetCollider.radius = planet.Radius;
            GenerateResourcePoints(planet.Resources);
        }

        private void GenerateResourcePoints(ResourcePoint[] resources) {
            foreach (var resource in resources) {
                resourcePoints.Add(CreateResourcePoint(resource));
            }
        }

        private ResourcePointView CreateResourcePoint(ResourcePoint point) {
            var pointView = Instantiate(resourcePointViewPrefab, transform);
            pointView.transform.localPosition = point.Position;
            pointView.transform.localRotation = Quaternion.LookRotation(point.Position) * Quaternion.Euler(90, 0, 0);
            pointView.Init(point);
            return pointView;
        }


        [SerializeField]
        private bool debugShowNavigationPoints = false;
        [SerializeField]
        private bool debugShowNavigationConnections = false;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (planet == null)
                return;
            Gizmos.color = Color.green;
            if (debugShowNavigationPoints)
                DrawNavPoints(planet.Graph.Nodes);
            Gizmos.color = Color.blue;
            if (debugShowNavigationConnections)
                DrawNavConnections(planet.Graph.Nodes);
        }

        private void DrawNavPoints(NavigationGraph.Node[] nodes) {
            foreach (var node in nodes) {
                Gizmos.DrawWireSphere(node.Position, 0.02f);
            }
        }

        private void DrawNavConnections(NavigationGraph.Node[] nodes) {
            foreach (var node in nodes) {
                foreach (var neigbhor in node.Neigbhors) {
                    Gizmos.DrawLine(node.Position, nodes[neigbhor].Position);
                }
            }
        }
#endif
    }
}
