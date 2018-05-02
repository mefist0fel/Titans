using UnityEngine;

namespace Model {
    public sealed class Planet {
        public readonly float Radius;

        public readonly ResourcePoint[] Resources;

        public bool FindNearestResourcePoint(Vector3 position, out ResourcePoint resourcePoint, float maxDistance = 0.5f) {
            resourcePoint = null;
            var nearestDistance = 0f;
            foreach (var point in Resources) {
                if (point == null || point.Count == 0)
                    continue;
                var distance = Vector3.Distance(point.Position, position);
                if (distance < maxDistance && (distance < nearestDistance || resourcePoint == null)) {
                    nearestDistance = distance;
                    resourcePoint = point;
                }
            }
            return resourcePoint != null;
        }

        public Planet(float radius = 10f, int resourcePointsCount = 20) {
            Radius = radius;
            Resources = new ResourcePoint[resourcePointsCount];
            for (int i = 0; i < Resources.Length; i++) {
                Resources[i] = new ResourcePoint(GetRandomPosition(), 10);
            }
        }

        public Vector3 GetRandomPosition() {
            Vector3 point;
            do {
                point = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f));
            } while (point.magnitude > 1f);
            return point.normalized * Radius;
        }
    }
}