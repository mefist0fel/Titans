using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    public TitanViewOld controlledTitan; // Set from editor
    [SerializeField]
    public PlanetViewOld controlledPlanet; // Set from editor
    [SerializeField]
    public List<string> projectsList = new List<string>() {
        "weapon",
        "shield",
        "rocket",
        "anti_air",
        "enemy_titan",
        "titan_upgrade",
        "shield",
        "rocket",
        "anti_air"
    };
    // weapon
    // rocket
    // shield
    // anti_air
    // enemy_titan
    // titan_upgrade
    // add_rocket

    public enum BaseTarget {
        CollectResources,
        KillAllEnemy,
        DoNothing
    }

    public BaseTarget target = BaseTarget.CollectResources;
    public ResourcePointViewOld NearestResource = null;
    public TitanViewOld NearestEnemy = null;

    private static List<ResourcePointViewOld> CollectedResources = new List<ResourcePointViewOld>();

    public float timer = 0;
    [SerializeField]
    public float updateTime = 1f;

	void Start () {
        controlledPlanet = Game.Instance.Planet;
    }
	
	public void Update () {
        timer -= Time.deltaTime;
        if (timer < 0) {
            timer = updateTime;
            UpdateTarget();
            UpdateBuildList();
        }
	}

    private void UpdateBuildList() {
        if (projectsList.Count == 0) {
            target = BaseTarget.KillAllEnemy;
            return;
        }
        var module = ConfigOld.Modules[projectsList[0]];
        if (module == null) {
            Debug.Log("Error module " + projectsList[0] + " - to next step");
            projectsList.RemoveAt(0);
            return;
        }
        if (controlledTitan.ResourceUnits >= module.Cost) {
            TryBuildModule(module);
            projectsList.RemoveAt(0);
        }
    }

    private void TryBuildModule(ModuleDataOld module) {
        if (module.Id == "enemy_titan") {
            Debug.Log("Build titan");
            controlledTitan.BuildTitan(module);
            return;
        }
        if (module.Id == "titan_upgrade") {
            controlledTitan.BuildUpgrade(module);
            return;
        }
        if (module.Id == "add_rocket") {
            controlledTitan.BuildRocket(module);
            return;
        }
        int emptySlotId = FindEmptySlotId();
        if (emptySlotId >= 0) {
            controlledTitan.BuildModule(module, emptySlotId);
            Debug.Log("Build module " + module.Id);
        }
    }

    private int FindEmptySlotId() {
        for (int i = 0; i < 12; i++) { // todo 12!! to modules count
            if (controlledTitan.Modules[i] == null) {
                return i;
            }
        }
        return -1;
    }

    private void UpdateTarget() {
        switch (target) {
            case BaseTarget.CollectResources:
                TryCollectResources();
                break;
            case BaseTarget.KillAllEnemy:
                Debug.Log("Kill all humans!");
                TryKillEnemy();
                break;
            case BaseTarget.DoNothing:
                break;
        }
    }

    private void TryKillEnemy() {
        if (!controlledTitan.IsAlive) {
            target = BaseTarget.DoNothing;
            return;
        }
        if (NearestEnemy == null || !NearestEnemy.IsAlive) {
            NearestEnemy = FindNearestEnemy();
        }
        MoveTo(NearestEnemy);
    }

    private void MoveTo(TitanViewOld nearestEnemy) {
        if (nearestEnemy == null)
            return;
        var delta = (controlledTitan.Position - nearestEnemy.Position).normalized;
        var needPosition = (nearestEnemy.Position + delta).normalized * controlledPlanet.Radius;
        controlledTitan.ClearTasks();
        controlledTitan.AddMoveTask(needPosition);
    }

    private TitanViewOld FindNearestEnemy() {
        TitanViewOld nearest = null;
        float nearestDistance = 0;
        foreach (var enemy in Game.Instance.Factions[0].Units) {
            if (enemy != null && enemy.IsAlive) {
                var distance = Vector3.Distance(controlledTitan.Position, enemy.Position);
                if (nearest == null || nearestDistance > distance) {
                    nearest = enemy;
                    nearestDistance = distance;
                }
            }
        }
        return nearest;
    }

    private void TryCollectResources() {
        if (NearestResource == null || NearestResource.Count == 0) {
            ResourcePointViewOld res;
            if (FindNearestResource(transform.position, out res)) {
                if (NearestResource != res) {
                    Debug.Log("To Next Point!");
                    NearestResource = res;
                }
            } else {
                Debug.Log("AAAAAA! No resources!");
                target = BaseTarget.KillAllEnemy;
                return;
            }
        }
        if (NearestResource != null && NearestResource.Count > 0) {
            controlledTitan.AddResourceTask(NearestResource);
            CollectedResources.Add(NearestResource);
            CollectedResources.RemoveAll((res) => res == null);
        } else {
            NearestResource = null;
            Debug.Log("To Next Point!");
        }
    }

    public bool FindNearestResource(Vector3 titanPosition, out ResourcePointViewOld resourcePoint) {
        resourcePoint = null;
        var nearestDistance = 0f;
        foreach (var point in controlledPlanet.ResourcePoins) {
            if (point == null || point.Count == 0)
                continue; // TODO remove empty points
            if (CollectedResources.Contains(point))
                continue;
            var distance = Vector3.Distance(point.transform.position, titanPosition);
            if (distance < nearestDistance || resourcePoint == null) {
                nearestDistance = distance;
                resourcePoint = point;
            }
        }
        return resourcePoint != null;
    }
}
