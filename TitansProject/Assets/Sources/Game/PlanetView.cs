using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class PlanetView : MonoBehaviour {
    [SerializeField]
    private float radius = 10;
    [SerializeField]
    private SphereCollider sphereCollider;
    [SerializeField]
    private int resourcePointsCount = 60;
    [SerializeField]
    private List<ResourcePointView> resourcePoints = new List<ResourcePointView>();
    [SerializeField]
    private List<GenerationFrequency> generationPreset = new List<GenerationFrequency>();

    [System.Serializable]
    public sealed class GenerationFrequency {
        public int Frequency = 1;
        public ResourcePointView ResourcePointPrefab = null;
    }

    public float Radius { get { return radius; } }

    private void Start() {
        Random.InitState(System.DateTime.UtcNow.Millisecond);
        SpawnPoints(resourcePointsCount);
        if (sphereCollider == null) {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.radius = Radius;
    }

    public bool RaycastSurfacePoint(Ray ray, out Vector3 clickPosition) {
        RaycastHit hit;
        clickPosition = Vector3.zero;
        if (sphereCollider.Raycast(ray, out hit, 20)) {
            clickPosition = hit.point;
            return true;
        }
        return false;
    }

    public bool FindResourcePointClick(Vector3 clickPosition, out ResourcePointView resourcePoint) {
        const float pointMaxDistance = 0.3f;
        resourcePoint = null;
        var nearestDistance = 0f;
        foreach (var point in resourcePoints) {
            if (point == null)
                continue; // TODO remove empty points
            var distance = Vector3.Distance(point.transform.position, clickPosition);
            if (distance < pointMaxDistance && (distance < nearestDistance || resourcePoint == null)) {
                nearestDistance = distance;
                resourcePoint = point;
            }
        }
        return resourcePoint != null;
    }

    private void SpawnPoints(int count) {
        foreach (var point in resourcePoints) {
            if (point != null) {
                Destroy(point.gameObject);
            }
        }
        resourcePoints.Clear();
        int maxFrequency;
        var pointsDistribution = GetPointsDistribution(generationPreset, out maxFrequency);
        for (int i = 0; i < count; i++) {
            resourcePoints.Add(SpawnPoint(GeneratePoint(pointsDistribution, maxFrequency)));
        }
    }

    private ResourcePointView SpawnPoint(ResourcePointView resourcePointPrefab) {
        var point = Instantiate(resourcePointPrefab);
        point.transform.parent = transform;
        point.transform.localPosition = GetRandomPosition();
        point.transform.localRotation = Quaternion.LookRotation(point.transform.localPosition) * Quaternion.Euler(90, 0, 0);
        return point;
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

    private Dictionary<int, ResourcePointView> GetPointsDistribution(List<GenerationFrequency> generationPreset, out int maxFrequency) {
        var distribution = new Dictionary<int, ResourcePointView>();
        maxFrequency = 0;
        foreach (var freq in generationPreset) {
            maxFrequency += freq.Frequency;
            distribution.Add(maxFrequency, freq.ResourcePointPrefab);
        }
        return distribution;
    }

    private ResourcePointView GeneratePoint(Dictionary<int, ResourcePointView> distribution, int maxFrequency) {
        var randomPoint = Random.Range(0, maxFrequency);
        ResourcePointView point = null;
        foreach (var freq in distribution) {
            point = freq.Value;
            if (randomPoint < freq.Key)
                return point;
        }
        return point;
    }
}
