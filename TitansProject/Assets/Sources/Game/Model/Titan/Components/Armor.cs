using System;
using UnityEngine;

namespace Model {
    public sealed class Armor: Titan.IComponent {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        public float NormalizedRestoreTime { get { return timer / reloadTime; } }

        private int restoreValue = 0;

        public const float reloadTime = 5f;
        private float timer = 0;

        private Action onArmorUpdate;

        public Armor(Action onUpdateAction) {
            Value = 0;
            MaxValue = 0;
            timer = reloadTime;
            onArmorUpdate = onUpdateAction;
        }

        public void Update(float deltaTime) {
            if (Value >= MaxValue)
                return;
            if (restoreValue == 0)
                return;
            timer -= deltaTime;
            if (timer < 0) {
                Value = Mathf.Min(MaxValue, Value + restoreValue);
                timer = reloadTime;
                onArmorUpdate();
            }
        }

        public void OnHit(int damage) {
            Value = Mathf.Max(0, Value - damage);
        }

        public void OnAttach(ModuleData module) {
            MaxValue += module["armor"];
            Value += module["armor"];
            restoreValue += module["armor_restore"];
            onArmorUpdate();
        }

        public void OnDetach(ModuleData module) {
            MaxValue -= module["armor"];
            Value = Mathf.Min(Value, MaxValue);
            restoreValue -= module["armor_restore"];
            onArmorUpdate();
        }
    }
}
