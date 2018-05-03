namespace Model {
    /// <summary>
    /// Module get build config, looki for time and cost and after - build selected module in titan slot instead self
    /// </summary>
    public sealed class BuilderModule : IModule {
        public float FullTime { get; private set; }
        public float NormalizedTime { get { return timer / FullTime; } }
        public ModuleData ConstructionModule { get; private set; }

        private float timer;
        private ModuleSlot slot;
        private Titan controlledTitan;

        public BuilderModule(ModuleData buildModule, ModuleSlot buildSlot) {
            ConstructionModule = buildModule;
            slot = buildSlot;
            FullTime = buildModule.BuildTime;
            timer = buildModule.BuildTime;
        }

        public void OnAttach(Titan titan) {
            controlledTitan = titan;
        }

        public void OnDetach() {
            controlledTitan = null;
        }

        public void Update(float deltaTime) {
            timer -= deltaTime;
            if (timer <= 0) {
                BuildSelectedModule(ConstructionModule);
            }
        }

        private void BuildSelectedModule(ModuleData module) {
            var buildedModule = ModulesFactory.CreateModule(module, controlledTitan);
            slot.Attach(buildedModule);
        }
    }
}
