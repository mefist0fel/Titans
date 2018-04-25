using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIMarkerController : MonoBehaviour {
    [SerializeField]
    private GameObject controlArrow;
    private TitanView controlledTitan;
    private PlanetView controlledPlanet;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 referenceScreenSize;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        var scaler = GetComponentInParent<CanvasScaler>();
        referenceScreenSize = scaler.referenceResolution * 0.5f;
    }

    public void Init (TitanView titan, PlanetView planet) {
        controlledTitan = titan;
        controlledPlanet = planet;
        var image = GetComponentInChildren<Image>();
        if (image != null) {
            image.color = titan.SelfFaction.ID == 0 ? Color.blue : Color.red;
        }
	}

    Vector3 pos;
    private void OnDrawGizmos() {
        Gizmos.DrawCube(pos, Vector3.one);
    }

    private void Update () {
        if (controlledTitan == null)
            return;
        if (rectTransform == null)
            return;
        if (canvas == null)
            return;
        var camera = Camera.main;
        var radius = controlledPlanet.Radius;
        var planetPosition = controlledPlanet.transform.position;
        var markerPosition = controlledTitan.GetMarkerPosition();
        var cameraPosition = camera.transform.position;
        var normalToCamera = Vector3.Normalize(cameraPosition - markerPosition);
        Plane markerPlane = new Plane(normalToCamera, markerPosition);
        
        var distanceFromPlaneToPlanetCenter = markerPlane.GetDistanceToPoint(planetPosition); // signed distance to point
        var needShowArrowOnHorizon = distanceFromPlaneToPlanetCenter > 0;

        var distanceFromCameraToPlanetCenter = Vector3.Distance(cameraPosition, planetPosition);
        var angleCameraCenterMarker = Vector3.Angle(cameraPosition - planetPosition, markerPosition - planetPosition);


        Vector3 horizonMarkerPosition = markerPosition;
        if (needShowArrowOnHorizon) {
            if (distanceFromCameraToPlanetCenter > radius) {
                var horizonPlanetCameraCos = radius / distanceFromCameraToPlanetCenter;
                var horizonPlanetCameraAngle = Mathf.Acos(horizonPlanetCameraCos) / Mathf.PI * 180f;
                var horizonPlanetMarkerAngle = angleCameraCenterMarker - horizonPlanetCameraAngle;
                var horizonPlanetMarkerCos = Mathf.Cos(horizonPlanetMarkerAngle / 180f * Mathf.PI);
                horizonMarkerPosition = planetPosition + (markerPosition - planetPosition).normalized * (radius / horizonPlanetMarkerCos);
            }
        }
        pos = horizonMarkerPosition;
        // if (controlArrow != null) {
        //     controlArrow.SetActive(needShowArrow);
        // }
        Vector2 border = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 centerPosition = ((Vector2)camera.WorldToScreenPoint(Vector3.zero) - border) * (1f / canvas.scaleFactor);
        Vector2 position = ((Vector2)camera.WorldToScreenPoint(horizonMarkerPosition) - border) * (1f / canvas.scaleFactor);
        float screenBorderPixels = 32f;
        float aspect = Screen.width / (float)Screen.height;
        if (position.x > referenceScreenSize.x * aspect - screenBorderPixels) {
            position *= (referenceScreenSize.x * aspect - screenBorderPixels) / position.x;
        }
        if (position.x < -(referenceScreenSize.x * aspect - screenBorderPixels)) {
            position *= -(referenceScreenSize.x * aspect - screenBorderPixels) / position.x;
        }
        if (position.y > referenceScreenSize.y - screenBorderPixels) {
            position *= (referenceScreenSize.y - screenBorderPixels) / position.y;
        }
        if (position.y < -(referenceScreenSize.y - screenBorderPixels)) {
            position *= -(referenceScreenSize.y - screenBorderPixels) / position.y;
        }
        rectTransform.anchoredPosition = position;
	}
}
