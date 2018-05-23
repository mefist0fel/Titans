using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public sealed class ModulesFactory : MonoBehaviour {
        public static IModule CreateBuildModule(ModuleData module, ModuleSlot slot) {
            return new BuilderModule(module, slot);
        }

        public static IModule CreateModule(ModuleData moduleData, Titan titan) {
            Debug.Log("Try create module " + moduleData.Id);
            switch (moduleData.Id) {
                case "laser":
                    return new LaserModule(moduleData);
                case "rocket":
                    return null;
                case "shield":
                    return new ShieldCapacitorModule(moduleData);
                case "anti_air":
                    return null;
            }
            Debug.LogError("Error creating module " + moduleData.Id);
            return null;
        }
    }
}