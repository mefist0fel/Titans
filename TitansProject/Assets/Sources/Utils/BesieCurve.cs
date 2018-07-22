using UnityEngine;
using System.Collections;

public class BesieCurve {
    private const int defaultPointsInCurve = 7;

    // Curve controls
    public float Lenght { get { return fullLenght; } }

    public Vector3[] pathPoints = null;
    private float[] pathLenghts = null;
    private float[] pathDistances = null;
    private float fullLenght = 0;

    public Vector3 GetPositionOnCurve(float distance) {
        if (distance <= 0) {
            return pathPoints[0];
        }
        if (distance >= fullLenght) {
            return pathPoints[pathPoints.Length - 1];
        }
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            if (distance > pathDistances[i] && distance <= pathDistances[i + 1]) {
                float normalizedLocalDistance = (distance - pathDistances[i]) / pathLenghts[i];
                return Utils.Lerp(pathPoints[i], pathPoints[i + 1], normalizedLocalDistance);
            }
        }
        return pathPoints[pathPoints.Length - 1];
    }

    public BesieCurve(Vector3[] controlPoints, int pointsInCurveTurn = defaultPointsInCurve) {
        GeneratePath(controlPoints, pointsInCurveTurn);
        FindPointDistancesParams();
    }

    private void GeneratePath(Vector3[] controlPoints, int pointsBetweenControlPoints = defaultPointsInCurve) {
        Vector3[] middleControlPoints = new Vector3[controlPoints.Length - 1];
        middleControlPoints[0] = controlPoints[0]; // first point
        middleControlPoints[middleControlPoints.Length - 1] = controlPoints[controlPoints.Length - 1]; // last point
        for (int i = 0; i < middleControlPoints.Length; i++) {
            middleControlPoints[i] = (controlPoints[i] + controlPoints[i + 1]) * 0.5f;
        }
        pathPoints = new Vector3[(middleControlPoints.Length - 1) * pointsBetweenControlPoints + 3];
        pathPoints[0] = controlPoints[0];
        for (int i = 0; i < middleControlPoints.Length - 1; i++) {
            for (int segment = 0; segment < pointsBetweenControlPoints; segment++) {
                pathPoints[i * pointsBetweenControlPoints + 1 + segment] = GetBesiePoint(
                    middleControlPoints[i],
                    middleControlPoints[i + 1],
                    controlPoints[i + 1],
                    (float)segment / (float)(pointsBetweenControlPoints));
            }
        }
        pathPoints[pathPoints.Length - 2] = middleControlPoints[middleControlPoints.Length - 1];
        pathPoints[pathPoints.Length - 1] = controlPoints[controlPoints.Length - 1];
    }

    private void FindPointDistancesParams() {
        pathLenghts = new float[pathPoints.Length];
        pathDistances = new float[pathPoints.Length];
        fullLenght = 0;
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            pathLenghts[i] = (pathPoints[i + 1] - pathPoints[i]).magnitude;
            pathDistances[i] = fullLenght;
            fullLenght += pathLenghts[i];
        }
        pathDistances[pathDistances.Length - 1] = fullLenght;
    }

    public static Vector3 GetBesiePoint(Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint, float t) {
        Vector3 startControlLine = (startPoint + (controlPoint - startPoint) * t);
        Vector3 controlEndLine = (controlPoint + (endPoint - controlPoint) * t);
        return (startControlLine + (controlEndLine - startControlLine) * t);
    }
}