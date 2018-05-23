using UnityEngine;

namespace Model {
    public sealed class Shield {
        public int Capacity { get; private set; }
        public int MaxCapacity { get; private set; }

        public float NormalizedRestoreTime { get { return timer / reloadTime; } }

        private int restoreValue = 5;

        public const float reloadTime = 10f;
        private float timer = 0;

        private Action onShieldUpdate;

        public Shield(Action onUpdateAction) {
            Capacity = 0;
            MaxCapacity = 0;
            timer = reloadTime;
            onShieldUpdate = onUpdateAction;
        }

        public void Update(float deltaTime) {
            if (Capacity >= MaxCapacity)
                return;
            timer -= deltaTime;
            if (timer < 0) {
                Capacity = Mathf.Min(MaxCapacity, Capacity + restoreValue);
                timer = reloadTime;
                onShieldUpdate();
            }
        }

        public void AddCapacity(int count) {
            MaxCapacity += count;
            Capacity = Mathf.Min(MaxCapacity, Capacity);
        }
    }
}
