using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    public TitanView controlledTitan; // Set from editor
    [SerializeField]
    public PlanetView controlledPlanet; // Set from editor

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
    }

    private void UpdateTarget() {
        switch (target) {
            case BaseTarget.CollectResources:
                TryCollectResources();
                break;
            case BaseTarget.KillAllEnemy:
                Debug.LogError("Kill al humans!");
                break;
        }
    }

    private void TryCollectResources() {
        if (NearestResource == null || NearestResource.Count == 0) {
            ResourcePointView res;
            if (controlledPlanet.FindResourcePointClick(transform.position, out res, 5)) {
                if (NearestResource != res) {
                    Debug.LogError("To Next Point!");
                    NearestResource = res;
                }
            } else {
                Debug.LogError("AAAAAA! No resources!");
                target = BaseTarget.KillAllEnemy;
                return;
            }
        }
        if (NearestResource != null && NearestResource.Count > 0) {
            controlledTitan.AddResourceTask(NearestResource);
        } else {
            NearestResource = null;
            Debug.LogError("To Next Point!");
        }
    }
}
