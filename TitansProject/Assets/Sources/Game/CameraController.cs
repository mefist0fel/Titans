using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CameraController : MonoBehaviour {
    private static CameraController Instance;

    [SerializeField]
    private float moveSpeed = 0.8f;
    [SerializeField]
    private float rotationSpeed = 120f;
    [SerializeField]
    private PlanetView planet; // Set from editor
    [SerializeField]
    private Camera mainCamera; // Set from editor
    [SerializeField]
    private Quaternion cameraViewRotation = Quaternion.Euler(60, 0, 0);
    [SerializeField]
    private VisualLevelData[] levels = new VisualLevelData[] {
        new VisualLevelData() { Distance = 5 }
    };
    [SerializeField]
    private float timeToChangeLevel = 0.6f;

    [Serializable]
    public sealed class VisualLevelData {
        public float Distance = 10;
        public Vector3 Rotation = new Vector3(60, 0, 0);
        public float FieldOfView = 70;
    }

    private int cameraLevel = 1;
    private float changeLevelTimer = 0;
    private float needCameraDistance = 5;
    private Vector3 needCameraRotation = new Vector3(60, 0, 0);
    private Vector3 currentCameraRotation = new Vector3(60, 0, 0);
    private float currentFieldOfView = 70;
    private float needFieldOfView = 70;

    [SerializeField]
    private Vector2 inertionAttenuation = new Vector2(4f, 4f); // Per second
    private Vector2 rotationInertionVelocity = Vector2.zero;

    private Quaternion rotation = Quaternion.identity;
    private float AngularSpeed {
        get {
            return moveSpeed * 2 * Mathf.PI * planet.Radius;
        }
    }

    public static void SetViewToTitan(Vector3 position) {
        if (Instance != null) {
            Instance.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
            Instance.SetRotation();
        }
    }

    public static void MoveToTitan(Vector3 endPosition) {
        if (Instance != null) {
            var startPosition = Instance.transform.position;
            var startRotation = Instance.rotation;
            var moveAxe = -Utils.GetNormal(endPosition, startPosition, Vector3.zero);
            var angle = Vector3.Angle(endPosition, startPosition);

            Timer.Add(0.5f,
                (anim) => {
                    Instance.rotation = Quaternion.AngleAxis(angle * anim, moveAxe) * startRotation;
                });
        }
    }

    private void Awake () {
        Instance = this;
    }

    private void Start() {
        SetLevel(levels[1]);
    }
 
    Vector3 prevPosition;
    private void Update () {
        if (Input.GetMouseButtonDown(0)) {
            prevPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            const float unitScaleFactor = 2000f;
            var delta = (Input.mousePosition - prevPosition);
            rotationInertionVelocity.x = delta.x / Screen.height * unitScaleFactor;
            rotationInertionVelocity.y = delta.y / Screen.height * unitScaleFactor;
            prevPosition = Input.mousePosition;
        } else {
            rotationInertionVelocity.Scale(Vector2.one - inertionAttenuation * Time.deltaTime);
        }
        Rotate(Vector3.forward, rotationInertionVelocity.x);
        Rotate(Vector3.left, rotationInertionVelocity.y);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            Rotate(Vector3.left, -AngularSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            Rotate(Vector3.left, AngularSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Rotate(Vector3.forward, AngularSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Rotate(Vector3.forward, -AngularSpeed);
        }
        if (Input.GetKey(KeyCode.Q)) {
            Rotate(Vector3.up, -rotationSpeed);
        }
        if (Input.GetKey(KeyCode.E)) {
            Rotate(Vector3.up, rotationSpeed);
        }
        if (changeLevelTimer > 0) {
            changeLevelTimer -= Time.deltaTime;
        } else {
            if (Input.mouseScrollDelta != Vector2.zero) {
                if (Input.mouseScrollDelta.y > 0) {
                    cameraLevel -= 1;
                }
                if (Input.mouseScrollDelta.y < 0) {
                    cameraLevel += 1;
                }
                if (cameraLevel < 0)
                    cameraLevel = 0;
                if (cameraLevel >= levels.Length)
                    cameraLevel = levels.Length - 1;
                SetLevel(levels[cameraLevel]);
            }
        }
        mainCamera.transform.localPosition = new Vector3(0, 0, -needCameraDistance * 0.05f + mainCamera.transform.localPosition.z * 0.95f);
        currentCameraRotation = Vector3.Lerp(currentCameraRotation, needCameraRotation, 0.05f);
        cameraViewRotation = Quaternion.Euler(currentCameraRotation);
        currentFieldOfView = needFieldOfView * 0.05f + currentFieldOfView * 0.95f;
        mainCamera.fieldOfView = currentFieldOfView;
        SetRotation();
    }

    private void SetLevel(VisualLevelData visualLevelData) {
        changeLevelTimer = timeToChangeLevel;
        needCameraDistance = visualLevelData.Distance;
        needCameraRotation = visualLevelData.Rotation;
        needFieldOfView = visualLevelData.FieldOfView;
    }

    private void SetRotation() {
        transform.position = rotation * Vector3.up * planet.Radius;
        transform.rotation = rotation * cameraViewRotation;
    }

    private void Rotate(Vector3 axe, float angularSpeed) {
        rotation = rotation * Quaternion.AngleAxis(angularSpeed * Time.deltaTime, axe);
    }
}
