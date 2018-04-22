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
 
    Vector2 prevPosition;
    private void Update () {
        if (Input.touchSupported) {
            if (Input.touchCount == 2) {
                var touchA = Input.GetTouch(0);
                var touchB = Input.GetTouch(1);
                Vector2 prevTouchAPosition = touchA.position;
                Vector2 prevTouchBPosition = touchB.position;
                if (touchA.phase == TouchPhase.Moved) {
                    prevTouchAPosition = touchA.position - touchA.deltaPosition;
                }
                if (touchB.phase == TouchPhase.Moved) {
                    prevTouchBPosition = touchB.position - touchB.deltaPosition;
                }

                RotateByChangeDoubleTouchAngle(
                    touchA.position,
                    touchB.position,
                    prevTouchAPosition,
                    prevTouchBPosition);
                ScaleByChangeDoubleTouchAngle(
                    touchA.position,
                    touchB.position,
                    prevTouchAPosition,
                    prevTouchBPosition);
            }
            if (Input.touchCount == 1) {
                var touchA = Input.GetTouch(0);
                prevPosition = touchA.position;
                if (touchA.phase == TouchPhase.Moved) {
                    prevPosition = touchA.position - touchA.deltaPosition;
                }
                const float unitScaleFactor = 30f;
                var delta = (touchA.position - prevPosition);
                rotationInertionVelocity.x = delta.x / Screen.height * unitScaleFactor;
                rotationInertionVelocity.y = delta.y / Screen.height * unitScaleFactor;
                prevPosition = touchA.position;
            } else {
                rotationInertionVelocity.Scale(Vector2.one - inertionAttenuation * Time.deltaTime);
            }
        } else {
            if (Input.GetMouseButtonDown(0)) {
                prevPosition = Input.mousePosition;
            }
            var ctrl = Input.GetKey(KeyCode.LeftControl);
            if (Input.GetMouseButton(0)) {
                if (ctrl) {
                    RotateByChangeDoubleTouchAngle(
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        Input.mousePosition,
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        prevPosition);
                    ScaleByChangeDoubleTouchAngle(
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        Input.mousePosition,
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        prevPosition);
                } else {
                    const float unitScaleFactor = 30f;
                    var delta = ((Vector2)Input.mousePosition - prevPosition);
                    rotationInertionVelocity.x = delta.x / Screen.height * unitScaleFactor;
                    rotationInertionVelocity.y = delta.y / Screen.height * unitScaleFactor;
                }
                prevPosition = Input.mousePosition;
            } else {
                rotationInertionVelocity.Scale(Vector2.one - inertionAttenuation * Time.deltaTime);
            }
        }
        Rotate(Vector3.forward, rotationInertionVelocity.x);
        Rotate(Vector3.left, rotationInertionVelocity.y);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            Rotate(Vector3.left, -AngularSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            Rotate(Vector3.left, AngularSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Rotate(Vector3.forward, AngularSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Rotate(Vector3.forward, -AngularSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q)) {
            Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) {
            Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
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

    float prevDistance = 0;
    private void ScaleByChangeDoubleTouchAngle(Vector2 positionA, Vector2 positionB, Vector2 prevPositionA, Vector2 prevPositionB) {
        if (changeLevelTimer > 0)
            return;
        var distance = Vector2.Distance(positionA, positionB);
        if (positionA == prevPositionA && positionB == prevPositionB) {
            prevDistance = Vector2.Distance(prevPositionA, prevPositionB);
        }
        const float precision = 1.1f; // 10% of screen
        if (distance > prevDistance * precision) {
            prevDistance = distance;
            cameraLevel -= 1;
        }
        if (distance * precision < prevDistance) {
            prevDistance = distance;
            cameraLevel += 1;
        }
        if (cameraLevel < 0)
            cameraLevel = 0;
        if (cameraLevel >= levels.Length)
            cameraLevel = levels.Length - 1;
        SetLevel(levels[cameraLevel]);
    }

    private void RotateByChangeDoubleTouchAngle(Vector2 positionA, Vector2 positionB, Vector2 prevPositionA, Vector2 prevPositionB) {
        var currentAngle = Utils.GetAngle(positionA, positionB);
        var prevAngle = Utils.GetAngle(prevPositionA, prevPositionB);
        Rotate(Vector3.up, prevAngle - currentAngle);
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
        rotation = rotation * Quaternion.AngleAxis(angularSpeed, axe);
    }
}
