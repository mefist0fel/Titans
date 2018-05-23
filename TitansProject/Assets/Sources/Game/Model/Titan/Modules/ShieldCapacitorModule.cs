using System;


namespace Model {
    public sealed class ShieldCapacitorModule : IModule {
        public string Id { get; private set; }
        private Titan titan;

        public ShieldCapacitorModule(ModuleData data) {
            Id = data.Id;
        }

        public void OnAttach(Titan controlTitan) {
            titan = controlTitan;
            titan.Shield.AddCapacity(10);
        }

        public void OnDetach() {
            titan.Shield.AddCapacity(-10);
        }

        public void Update(float deltaTime) {}
	}
}
