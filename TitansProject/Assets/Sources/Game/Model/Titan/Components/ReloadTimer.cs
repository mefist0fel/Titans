using Random = UnityEngine.Random;

namespace Model {
    public sealed class ReloadTimer {
        private readonly float reloadTime;
        private readonly float randomDispertion;

        private float timer = 0f;

        public bool IsReady { get { return timer <= 0; } }

        public ReloadTimer(float fullReloadTime = 2f, float maxDispertion = 0) {
            reloadTime = fullReloadTime - maxDispertion;
            randomDispertion = maxDispertion * 2f;
        }

        public void Update(float deltaTime) {
            if (IsReady)
                return;
            timer -= deltaTime;
        }

        public void Reload() {
            timer = reloadTime + Random.Range(0f, randomDispertion);
        }
    }
}