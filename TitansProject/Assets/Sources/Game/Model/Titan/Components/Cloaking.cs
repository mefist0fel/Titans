namespace Model {
    public sealed class Cloaking : Titan.IComponent {

        public int Value { get; private set; }

        public Cloaking(int value = 0) {
            Value = value;
        }

        public void Update(float deltaTime) {}

        public void OnAttach(ModuleData module) {
            Value += module["cloaking"];
        }

        public void OnDetach(ModuleData module) {
            Value -= module["cloaking"];
        }
    }
}
