using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class SelectionMarkerView : MonoBehaviour {
    [SerializeField]
    private LineRendererRing selectionRing; // Set from editor
    [SerializeField]
    private LineRendererRing fireRing; // Set from editor
    [SerializeField]
    private float defaultRadius = 0.54f;
    [SerializeField]
    private float showTime = 0.2f;
    [SerializeField]
    private AnimationCurve showAnimation = AnimationCurve.EaseInOut(0, 0.7f, 1f, 1f);

    private float timer = 0;
    private float fireRadius = 2;
    private bool needShowWeaponRange = false;

    public void SelectTitan(TitanView titan) {
        timer = 0;
        gameObject.SetActive(true);
        needShowWeaponRange = titan.Modules.Any((module) => module is WeaponModule);
        transform.parent = titan.transform;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        fireRing.gameObject.SetActive(needShowWeaponRange);
    }

    public void HideSelection() {
        gameObject.SetActive(false);
        transform.parent = null;
    }

    private void Update() {
        if (timer < showTime) {
            timer += Time.deltaTime;
            if (timer > showTime) {
                timer = showTime;
            }
            UpdateMarker(timer / showTime);
        }
    }

    private void UpdateMarker(float anim) {
        Color color = new Color(1f, 1f, 1f, anim);
        selectionRing.SetColor(color);
        selectionRing.SetRadius(defaultRadius * showAnimation.Evaluate(anim));
        if (needShowWeaponRange) {
            fireRing.SetColor(color);
            fireRing.SetRadius(fireRadius * showAnimation.Evaluate(anim));
        }
    }
}
