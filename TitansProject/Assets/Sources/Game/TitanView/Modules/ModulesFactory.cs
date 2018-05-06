using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public static class ModulesFactory {
    public static ITitanModule CreateBuildModule(ModuleDataOld module, int slotId) {
        return BuildModule.Create(module, slotId);
    }
    public static ITitanModule CreateTitanBuildModule(ModuleDataOld titanModule) {
        return BuildTitanModule.Create(titanModule);
    }
    public static ITitanModule CreateTitanUpgradeModule(ModuleDataOld upgradeModule) {
        return BuildUpgradeModule.Create(upgradeModule);
    }
    public static ITitanModule CreateRocketModule(ModuleDataOld upgradeModule) {
        return BuildRocketModule.Create(upgradeModule);
    }

    public static ITitanModule CreateModule(ModuleDataOld moduleData, TitanViewOld titan) {
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
