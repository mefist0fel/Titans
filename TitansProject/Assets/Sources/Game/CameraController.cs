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
    
    Vector3 prevPosition;
    private void Update () {
        if (Input.GetMouseButtonDown(0)) {
            prevPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            Vector3 prevClickPosition;
            Vector3 clickPosition;
            if (
                planet.RaycastSurfacePoint(mainCamera.ScreenPointToRay(Input.mousePosition), out clickPosition) &&
                planet.RaycastSurfacePoint(mainCamera.ScreenPointToRay(prevPosition), out prevClickPosition)) {
                var moveAxe = -Utils.GetNormal(prevClickPosition, clickPosition, Vector3.zero);
                var angle = Vector3.Angle(prevClickPosition, clickPosition);
                rotation = Quaternion.AngleAxis(angle, moveAxe) * rotation;
            }
            prevPosition = Input.mousePosition;
        }

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
        SetRotation();
    }

    private void SetRotation() {
        transform.position = rotation * Vector3.up * planet.Radius;
        transform.rotation = rotation * cameraViewRotation;
    }

    private void Rotate(Vector3 axe, float angularSpeed) {
        rotation = rotation * Quaternion.AngleAxis(angularSpeed * Time.deltaTime, axe);
    }
}
