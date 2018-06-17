using System.Collections.Generic;
using UnityEngine;

namespace Model.AI {
    public sealed class CollectResourcesState : AbstractAIState {
        private readonly List<ResourcePoint> occupiedResourceList;
        private readonly Planet planet;

        public CollectResourcesState(TitanAI ai, Planet controlPlanet, List<ResourcePoint> occupiedResources) : base(ai) {
            occupiedResourceList = occupiedResources;
            planet = controlPlanet;
        }

        public override void Update(float deltaTime) {
            var currentTask = titanAI.Titan.GetCurrentTask();
            if (currentTask == null)
                TryCollectResources();
        }

        public override void OnStateEnter() {
            Debug.Log("Oh, i'm staring to collect resources");
        }

        private void TryCollectResources() {
            Debug.Log("Action is ended, try to find next resources");
            ResourcePoint nearestResourcePoint;
            if (!FindNearestResource(titanAI.Titan.Position, out nearestResourcePoint)) {
                Debug.Log("AAAAAA! No resources!");
                titanAI.SetState(titanAI.KillAllEnemy);
                return;
            }
            Debug.Log("Move to next point!");
            titanAI.Titan.AddResourceTask(nearestResourcePoint);
            occupiedResourceList.Add(nearestResourcePoint);
            occupiedResourceList.RemoveAll((res) => res == null || res.Count == 0);
        }

        public bool FindNearestResource(Vector3 titanPosition, out ResourcePoint resourcePoint) {
            resourcePoint = null;
            var nearestDistance = 0f;
            foreach (var point in planet.Resources) {
                if (point == null || point.Count == 0)
                    continue; // TODO remove empty points
                if (occupiedResourceList.Contains(point))
                    continue;
                var distance = Vector3.Distance(point.Position, titanPosition);
                if (distance < nearestDistance || resourcePoint == null) {
                    nearestDistance = distance;
                    resourcePoint = point;
                }
            }
            return resourcePoint != null;
        }
    }
}