﻿using Configs;
using UnityEngine;

public class BuildUpgradeModule : MonoBehaviour, ITitanModule {
    public float FullTime { get; private set; }
    public float NormalizedTime { get { return timer / FullTime; } }

    private float timer;
    private TitanView controlledTitan;
    private ModuleData titanData;

    public static BuildUpgradeModule Create(ModuleData upgradeModule) {
        GameObject buildObject = new GameObject();
        var module = buildObject.AddComponent<BuildUpgradeModule>();
        module.FullTime = upgradeModule.BuildTime;
        module.timer = upgradeModule.BuildTime;
        module.titanData = upgradeModule;
        return module;
    }

    public void OnAttach(TitanView titan) {
        controlledTitan = titan;
    }

    public void OnDetach() {
        controlledTitan = null;
        Destroy(gameObject);
    }

    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            BuildTitanUpgrade();
        }
    }

    private void BuildTitanUpgrade() {
        controlledTitan.Level += 1;
        controlledTitan.Attach(null, 13);
    }
}