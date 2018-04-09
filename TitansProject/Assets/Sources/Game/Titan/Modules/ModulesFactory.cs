using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public static class ModulesFactory {
    public static ITitanModule CreateBuildModule(ModuleData module, int slotId) {
        return BuildModule.Create(module, slotId);
    }
    public static ITitanModule CreateTitanBuildModule(ModuleData titanModule) {
        return BuildTitanModule.Create(titanModule);
    }
    public static ITitanModule CreateTitanUpgradeModule(ModuleData upgradeModule) {
        return BuildUpgradeModule.Create(upgradeModule);
    }
    public static ITitanModule CreateRocketModule(ModuleData upgradeModule) {
        return BuildRocketModule.Create(upgradeModule);
    }

    public static ITitanModule CreateModule(ModuleData moduleData, TitanView titan) {
        var moduleName = "Prefabs/Modules/" + moduleData.Id;
        var modulePrefab = Resources.Load<GameObject>(moduleName);
        if (modulePrefab == null) {
            Debug.LogError("Cant find module " + moduleName);
            return null;
        }
        var module = GameObject.Instantiate(modulePrefab, titan.transform);
        module.transform.localPosition = Vector3.zero;
        return module.GetComponent<ITitanModule>();
    }
}
