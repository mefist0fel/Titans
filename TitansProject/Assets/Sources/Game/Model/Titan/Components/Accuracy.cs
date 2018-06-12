using UnityEngine;
using Random = UnityEngine.Random;

namespace Model {
    public sealed class Accuracy : Titan.IComponent {
        private const float minimalMissChance = 0.1f; // 10%
        private const float minimalCriticalChance = 0.1f; // 10%

        public int Value { get; private set; }

        public Accuracy(int value = 0) {
            Value = value;
        }

        public bool GetHitChance(Cloaking cloaking) {
            var accuracyAspect = (float)Value / (float)cloaking.Value;
            var missChance = 1f - Mathf.Min(1f, accuracyAspect);
            missChance = missChance * (1f - minimalMissChance) + minimalMissChance;
            return Random.Range(0f, 1f) > missChance;
        }

        public bool GetCriticalChance(Cloaking cloaking) {
            var cloakAspect = (float)cloaking.Value / (float)Value;
            var critChance = 1f - Mathf.Min(1f, cloakAspect);
            critChance = critChance * (1f - minimalCriticalChance) + minimalCriticalChance;
            return Random.Range(0f, 1f) < critChance;
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
