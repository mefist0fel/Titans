using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Configs;
using System;

/// <summary>
/// Module get build config, looki for time and cost and after - build selected module in titan slot instead self
/// </summary>
public sealed class BuildModule : MonoBehaviour, ITitanModule {
    public float FullTime { get; private set; }
    public float NormalizedTime { get { return timer / FullTime; } }
    public ModuleData ConstructionModule { get; private set; }

    private float timer;
    private int slotId;
    private TitanView controlledTitan;

    public static BuildModule Create (ModuleData buildModule, int slot) {
        GameObject buildObject = new GameObject();
        var module = buildObject.AddComponent<BuildModule>();
        module.ConstructionModule = buildModule;
        module.slotId = slot;
        module.FullTime = buildModule.BuildTime;
        module.timer = buildModule.BuildTime;
        return module;
    }

    public void OnAttach(TitanView titan) {
        controlledTitan = titan;
    }

    public void OnDetach() {
        controlledTitan = null;
        Destroy(gameObject);
    }

    private void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            BuildSelectedModule(ConstructionModule);
        }
    }

    private void BuildSelectedModule(ModuleData module) {
        controlledTitan.Attach(ModulesFactory.CreateModule(module, controlledTitan), slotId);
    }
}
