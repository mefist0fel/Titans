﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ExplosionViewOld : MonoBehaviour {
    [SerializeField]
    private float defaultRadius = 1f;
    [SerializeField]
    private float explosionSpeed = 0.4f;
    [SerializeField]
    private AnimationCurve explosionCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    private float radius = 1f;


    public static ExplosionViewOld Explode(Vector3 position, float radius, int damage = 0) {
        if (damage > 0) {
            HitUnits(position, radius, damage);
            StressCamera.Stress();
        }
        var explosion = Instantiate(Resources.Load<ExplosionViewOld>("Prefabs/Explosion"));
        explosion.Init(position, radius);
        return explosion;
    }

    private static void HitUnits(Vector3 position, float radius, int damage) {
        foreach (var faction in Game.Instance.Factions) {
            foreach (var unit in faction.Units) {
                if (unit == null || !unit.IsAlive)
                    continue;
                if (Vector3.Distance(unit.Position, position) < radius) {
                    unit.Hit(damage);
                }
            }
        }
    }

    private void Init(Vector3 position, float maxRadius) {
        transform.position = position;
        radius = maxRadius;
        transform.localScale = Vector3.zero;
        Timer.Add(explosionSpeed * radius,
            (anim) => {
                transform.localScale = Vector3.one * explosionCurve.Evaluate(anim) * defaultRadius * radius;
            },
            () => {
                Destroy(this.gameObject);
            });
    }
}