using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public sealed class ModulesFactory : MonoBehaviour {
        public static IModule CreateBuildModule(ModuleData module, ModuleSlot slot) {
            return new BuilderModule(module, slot);
        }

        public static IModule CreateModule(ModuleData moduleData, Titan titan) {
            //switch
            //var moduleName = "Prefabs/Modules/" + moduleData.Id;
            Debug.LogError("Create module test " + moduleData.Id);
            return null;
        }
    }
}