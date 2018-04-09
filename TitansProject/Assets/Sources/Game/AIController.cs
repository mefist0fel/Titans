using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    public TitanView controlledTitan; // Set from editor
    [SerializeField]
    public PlanetView controlledPlanet; // Set from editor
    [SerializeField]
    public List<string> projectsList = new List<string>() {
        "weapon",
        "shield",
        "rocket",
        "anti_air",
        "enemy_titan",
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
        KillAllEnemy
    }

    public BaseTarget target = BaseTarget.CollectResources;
    public ResourcePointView NearestResource = null;

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
        var module = Config.Modules[projectsList[0]];
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

    private void TryBuildModule(ModuleData module) {
        if (module.Id == "enemy_titan") {
            Debug.Log("Build titan");
            controlledTitan.BuildTitan(module);
            return;
        }
        if (module.Id == "titan_upgrade") {
            Debug.Log("Build titan_upgrade - not implemented");
            return;
        }
        if (module.Id == "add_rocket") {
            Debug.Log("Build roclet - not implemented");
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
                Debug.Log("Kill al humans!");
                break;
        }
    }

    private void TryCollectResources() {
        if (NearestResource == null || NearestResource.Count == 0) {
            ResourcePointView res;
            if (controlledPlanet.FindResourcePointClick(transform.position, out res, 5)) {
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
        } else {
            NearestResource = null;
            Debug.Log("To Next Point!");
        }
    }
}
