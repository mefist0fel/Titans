﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketView : MonoBehaviour {
    [SerializeField]
    public float Speed = 20f;

    private float radius;
    private int damage;

    private float timer = 0;
    private float fullTime = 0;
    private float angle;
    private float maxHeightProportional = 0;
    private Vector3 endPosition;
    private Vector3 moveAxe;
    private PlanetView planet;

    public static void Fire(Vector3 startPosition, Vector3 endPosition, PlanetView planet, int damage, float radius) {
        var rocket = Instantiate(Resources.Load<RocketView>("Prefabs/Rocket"));
        rocket.Init(startPosition, endPosition, planet, damage, radius);
    }

    private void Init(Vector3 startPosition, Vector3 endPosition, PlanetView controlPlanet, int damage, float radius) {
        planet = controlPlanet;
        this.damage = damage;
        this.radius = radius;
        var start = startPosition;
        this.endPosition = endPosition;
        moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
        angle = Vector3.Angle(startPosition, endPosition);
        maxHeightProportional = Mathf.Min(0.3f, angle / 360f * 1f);
        float distance = angle / 180f * Mathf.PI * 2f * planet.Radius;
        fullTime = distance / Speed;
        timer = fullTime;
        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
            trail.Clear();
    }
	
	void Update () {
        if (timer <= 0)
            return;
        timer -= Time.deltaTime;
        if (timer < 0) {
            timer = 0;
            Explode();
        }
        AnimateMove(timer / fullTime);
    }

    private void Explode() {
        Destroy(gameObject, 0.3f);
        ExplosionView.Explode(endPosition, radius, damage);
    }

    private void AnimateMove(float backTime) {
        float trajectory = (backTime * 2f - 1f);
        trajectory = trajectory * trajectory * -1 + 1f;
        var rotation = Quaternion.AngleAxis(angle * backTime, moveAxe);
        transform.localPosition = rotation * (endPosition * (1f + maxHeightProportional * trajectory));
        transform.rotation = Quaternion.LookRotation(rotation * endPosition) * Quaternion.Euler(90, 0, 0);
    }
}
