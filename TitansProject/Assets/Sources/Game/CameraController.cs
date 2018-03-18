using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private PlanetView planet; // Set from editor
    [SerializeField]
    private Quaternion cameraViewRotation = Quaternion.Euler(60, 0, 0);

    private Quaternion rotation = Quaternion.identity;
    private float AngularSpeed {
        get {
            return speed * 2 * Mathf.PI * planet.Radius;
        }
    }

    private void Start () {
		
	}
	
	private void Update () {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            Rotate(Vector3.left, -AngularSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow)) {
            Rotate(Vector3.left, AngularSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Rotate(Vector3.forward, AngularSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Rotate(Vector3.forward, -AngularSpeed);
        }
        if (Input.GetKey(KeyCode.Q)) {
            Rotate(Vector3.up, -AngularSpeed);
        }
        if (Input.GetKey(KeyCode.E)) {
            Rotate(Vector3.up, AngularSpeed);
        }
        transform.position = rotation * Vector3.up * planet.Radius;
        transform.rotation = rotation * cameraViewRotation;
    }

    private void Rotate(Vector3 axe, float angularSpeed) {
        rotation = rotation * Quaternion.AngleAxis(angularSpeed * Time.deltaTime, axe);
    }
}
