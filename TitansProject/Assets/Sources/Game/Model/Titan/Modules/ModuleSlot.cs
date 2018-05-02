using System;

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

        public void Attach(IModule module = null) {
            if (Module != null) {
                Module.OnDetach();
            }
            Module = module;
            if (Module != null) {
                Module.OnAttach(titan);
            }
        }

        public void Update(float deltaTime) {
            if (Module != null)
                Module.Update(deltaTime);
        }
    }
}
