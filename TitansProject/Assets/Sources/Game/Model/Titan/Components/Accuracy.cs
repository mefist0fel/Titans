namespace Model {
    public sealed class Accuracy : Titan.IComponent {

        public int Value { get; private set; }

        public Accuracy(int value = 0) {
            Value = value;
        }

        public void Update(float deltaTime) {}

        public void OnAttach(ModuleData module) {
            Value += module["accuracy"];
        }

        public void OnDetach(ModuleData module) {
            Value -= module["accuracy"];
        }
    }
}
