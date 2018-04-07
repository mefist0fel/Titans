using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public sealed class LineRendererRing : MonoBehaviour {

    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private int pointsPerUnit = 7;
    [SerializeField]
    private LineRenderer line; // Set from editor or create on start

    public void SetRadius(float ringRaduis) {
        radius = ringRaduis;
        SetRadius();
    }

	private void Awake () {
        if (line == null)
            line = GetComponent<LineRenderer>();
        SetRadius();
    }

    public void SetColor(Color color) {
        if (line == null)
            return;
        line.startColor = color;
        line.endColor = color;
    }

    [ContextMenu("Set radius")]
    private void SetRadius() {
        if (line == null)
            return;
        line.loop = true;
        int pointsCount = Mathf.CeilToInt(2f * Mathf.PI * radius * pointsPerUnit);
        var points = new Vector3[pointsCount];
        float segmentAngle = 360f / pointsCount;
        for (int i = 0; i < pointsCount; i++) {
            points[i] = Quaternion.Euler(0, i * segmentAngle, 0) * new Vector3(radius, 0, 0);
        }
        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}
