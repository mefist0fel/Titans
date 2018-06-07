using UnityEngine;
using UnityEngine.UI;
using View;

namespace UI {
    public sealed class UIMarkerController : MonoBehaviour {
        [SerializeField]
        private RectTransform controlArrow;
        [SerializeField]
        private UIScaleAnimator scaleAnimator;
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

        public void Init(TitanView titan, PlanetView planet) {
            controlledTitan = titan;
            controlledPlanet = planet;
            var image = GetComponentInChildren<Image>();
            if (image != null) {
                image.color = titan.Titan.Faction.ID == 0 ? Color.blue : Color.red;
            }
        }

        // TODO refactor it later
        private void Update() {
            if (controlledTitan == null)
                return;
            if (rectTransform == null)
                return;
            if (canvas == null)
                return;
            const float markerHeight = 0.35f;
            var camera = Camera.main;
            var radius = controlledPlanet.Planet.Radius + markerHeight;
            var planetPosition = controlledPlanet.transform.position;
            var markerPosition = controlledTitan.Titan.Position.normalized * radius;
            var directionMarkerPosition = markerPosition * 0.99f;
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
            Vector2 border = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            // Vector2 centerPosition = ((Vector2)camera.WorldToScreenPoint(Vector3.zero) - border) * (1f / canvas.scaleFactor);
            Vector2 position = ((Vector2)camera.WorldToScreenPoint(horizonMarkerPosition) - border) * (1f / canvas.scaleFactor);
            Vector2 angleStartPosition = ((Vector2)camera.WorldToScreenPoint(markerPosition) - border) * (1f / canvas.scaleFactor);
            Vector2 angleDirectionPosition = ((Vector2)camera.WorldToScreenPoint(directionMarkerPosition) - border) * (1f / canvas.scaleFactor);
            if (controlArrow != null) {
                controlArrow.localEulerAngles = new Vector3(0, 0, -Utils.GetAngle(angleStartPosition, angleDirectionPosition) + 180f);
            }

            float screenBorderPixels = 32f;
            float aspect = Screen.width / (float)Screen.height;
            bool isOnScreen = true;
            if (position.x > referenceScreenSize.x * aspect - screenBorderPixels) {
                position *= (referenceScreenSize.x * aspect - screenBorderPixels) / position.x;
                isOnScreen = false;
            }
            if (position.x < -(referenceScreenSize.x * aspect - screenBorderPixels)) {
                position *= -(referenceScreenSize.x * aspect - screenBorderPixels) / position.x;
                isOnScreen = false;
            }
            if (position.y > referenceScreenSize.y - screenBorderPixels) {
                position *= (referenceScreenSize.y - screenBorderPixels) / position.y;
                isOnScreen = false;
            }
            if (position.y < -(referenceScreenSize.y - screenBorderPixels)) {
                position *= -(referenceScreenSize.y - screenBorderPixels) / position.y;
                isOnScreen = false;
            }
            var needToShow = needShowArrowOnHorizon || !isOnScreen;
            ShowMarker(needToShow);
            rectTransform.anchoredPosition = position;
        }

        private void ShowMarker(bool needToShow) {
            scaleAnimator.Show(needToShow);
        }
    }

}