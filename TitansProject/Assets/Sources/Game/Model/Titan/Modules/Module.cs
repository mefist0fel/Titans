namespace Model {
    public sealed class Module : IModule {
        public string Id { get { return data.Id; } }
        private readonly ModuleData data;

        public Module(ModuleData moduleData) {
            data = moduleData;
        }

        public void OnAttach(Titan titan) {
            titan.AddParams(data);
        }

        public void OnDetach(Titan titan) {
            titan.RemoveParams(data);
        }

        public void Update(float deltaTime) {}
    }
}
