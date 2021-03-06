﻿using UnityEngine;

namespace Model {
    /// <summary>
    /// class to control modules in slot
    /// </summary>
    public sealed class ModuleSlot {
        public IModule Module { get; private set; }
        private readonly Titan titan;

        public ModuleSlot(Titan hostTitan) {
            titan = hostTitan;
        }

        public bool CanBuild(ModuleData moduleData) {
            return moduleData.Cost <= titan.ResourceUnits;
        }

        public void Build(ModuleData moduleData) {
            if (CanBuild(moduleData)) {
                titan.ChangeResourceCount(-moduleData.Cost);
                var module = new BuilderModule(moduleData, this);
                Attach(module);
            } else {
                Debug.LogError("Not enough resources for module: " + moduleData.Id);
            }
        }

        public void Attach(IModule module = null) {
            if (Module != null) {
                Module.OnDetach(titan);
            }
            Module = module;
            if (Module != null) {
                Module.OnAttach(titan);
            }
            titan.UpdateModules();
        }

        public void Update(float deltaTime) {
            if (Module != null)
                Module.Update(deltaTime);
        }
    }
}
