using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketAimView : MonoBehaviour {
    private static RocketAimView instance;

    [SerializeField]
    private PlanetView planet; // Set from editor
    [SerializeField]
    private LineRendererRing fireZoneRing; // Set from editor
    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private Camera mainCamera; // Set from editor

    private bool needShow = false;

    private Action<Vector3> onFire;

    public void Awake () {
        instance = this;
        gameObject.SetActive(false);
    }

    public static void Show(Action<Vector3> onSelectFireAction, float fireRadius) {
        instance.onFire = onSelectFireAction;
        instance.gameObject.SetActive(true);
        instance.fireZoneRing.gameObject.SetActive(false);
        instance.radius = fireRadius;
        instance.needShow = true;
    }

    public static void Hide() {
        instance.gameObject.SetActive(false);
    }

    public void Update () {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 surfacePosition;
        if (planet.RaycastSurfacePoint(ray, out surfacePosition)) {
            transform.position = surfacePosition;
            transform.rotation = Quaternion.LookRotation(surfacePosition) * Quaternion.Euler(90, 0, 0);
            fireZoneRing.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(1)) {
                if (onFire != null) {
                    onFire(surfacePosition);
                }
            }
            if (needShow) {
                needShow = false;
                fireZoneRing.SetRadius(0);
                Timer.Add(0.4f,
                    (anim) => {
                        if (this == null)
                            return;
                        fireZoneRing.SetRadius(showCurve.Evaluate(anim) * radius);
                    }, () => {
                        if (this == null)
                            return;
                        fireZoneRing.SetRadius(radius);
                    });
            }
        } else {
            fireZoneRing.gameObject.SetActive(false);
        }
    }
}
