using System;
using UnityEngine;

namespace Model {
    public sealed class Shield: Titan.IComponent {
        public int Capacity { get; private set; }
        public int MaxValue { get; private set; }

        public float NormalizedRestoreTime { get { return timer / reloadTime; } }

        private int restoreValue = 0;

        public const float reloadTime = 5f;
        private float timer = 0;

        private Action onShieldUpdate;

        public Shield(Action onUpdateAction) {
            Capacity = 0;
            MaxValue = 0;
            timer = reloadTime;
            onShieldUpdate = onUpdateAction;
        }

        public void Update(float deltaTime) {
            if (Capacity >= MaxValue)
                return;
            timer -= deltaTime;
            if (timer < 0) {
                Capacity = Mathf.Min(MaxValue, Capacity + restoreValue);
                timer = reloadTime;
                onShieldUpdate();
            }
        }

        public int OnHit(int damage) {
            if (Capacity == 0)
                return damage;
            // Shield consume all damage
            Capacity = Mathf.Max(0, Capacity - damage);
            return 0;
        }

        public void OnAttach(ModuleData module) {
            MaxValue += module["shield"];
            restoreValue += module["shield_restore"];
            onShieldUpdate();
        }

        public void OnDetach(ModuleData module) {
            MaxValue -= module["shield"];
            restoreValue -= module["shield_restore"];
            onShieldUpdate();
        }
    }
}
