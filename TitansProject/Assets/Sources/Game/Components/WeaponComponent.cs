﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponComponent : MonoBehaviour, ITitanComponent {
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float accuracy = 0.5f;
    [SerializeField]
    private float reloadTime = 1f;
    [SerializeField]
    private float fireRadius = 2f;
    [SerializeField]
    private float aimTime = 1f;

    private TitanView enemyTitan;
    private TitanView parentTitan;

    private float timer = 0;

    public bool IsReady { get { return timer <= 0; } }

    public void Attach(TitanView titan) {
        parentTitan = titan;
    }

    public void Detach() {
    }

    public void Fire(TitanView enemyTitan) {
        Vector3 up = (parentTitan.Position + enemyTitan.Position).normalized;
        BulletController.Fire(parentTitan.GetFirePosition(), enemyTitan.GetHitPosition(), up, () => {
            if (UnityEngine.Random.Range(0f, 1f) < accuracy) {
                enemyTitan.Hit(damage);
            }
        });
        timer = reloadTime;
    }

    void Start () {
	}

	private void Update () {
        ProcessReload();
        if (!IsReady)
            return;
        UpdateAimTitan();
        if (enemyTitan == null)
            return;
        Fire(enemyTitan);
	}

    private void UpdateAimTitan() {
        if (enemyTitan != null && enemyTitan.IsAlive) {
            if (Vector3.Distance(enemyTitan.Position, parentTitan.Position) < fireRadius) {
                return;
            }
        }
        enemyTitan = FindInFireRange(parentTitan.SelfFaction.EnemyFaction.Units);
    }

    private TitanView FindInFireRange(List<TitanView> enemyTitans) {
        var position = parentTitan.Position;
        TitanView nearestEnemy = null;
        float nearestDistance = 0;
        foreach (var enemyTitan in enemyTitans) {
            if (enemyTitan == null)
                continue; // TODO remove empty points
            var distance = Vector3.Distance(enemyTitan.Position, position);
            if (distance < fireRadius && enemyTitan.IsAlive && (distance < nearestDistance || nearestEnemy == null)) {
                nearestDistance = distance;
                nearestEnemy = enemyTitan;
            }
        }
        return nearestEnemy;
    }

    private void ProcessReload() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        }
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }
}