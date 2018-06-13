using UnityEngine;
using Random = UnityEngine.Random;

namespace Model {
    public sealed class AntiAirDefence : Titan.IComponent {

        private readonly ReloadTimer timer = new ReloadTimer(2f);
        private const float interceptChance = 0.5f;

        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        public bool HasBullet {
            get {
                return Value > 0;
            }
        }

        public AntiAirDefence(int value = 0) {
            Value = value;
            MaxValue = value;
        }

        public bool Intercept() {
            if (Value <= 0) {
                return false;
            }
            Value -= 1; // Spent anti-air charge
            var isIntercepted = Random.Range(0f, 1f) > interceptChance;
            return isIntercepted;
        }

        public void Update(float deltaTime) {
            if (Value >= MaxValue)
                return;
            timer.Update(deltaTime);
            if (timer.IsReady)
                ReloadAntiAir();
        }

        private void ReloadAntiAir() {
            UnityEngine.Debug.LogError("Reload AA");
            timer.Reload();
            Value = MaxValue;
        }

        public void OnAttach(ModuleData module) {
            Value += module["anti_air"];
            MaxValue += module["anti_air"];
        }

        public void OnDetach(ModuleData module) {
            Value = Mathf.Max(0, Value - module["anti_air"]);
            MaxValue -= module["anti_air"];
        }
	}
}
