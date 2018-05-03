using UnityEngine;

namespace Model {
    public sealed class LaserModule : IModule {
        private Titan hostTitan;
        private int damage = 3;
        private float fireRadius = 2f;
        private float accuracy = 0.5f;
        private float reloadTime = 1.5f;
        private float reloadTimeRandomShift = 0.2f;
        private float timer = 0f;
        private Titan target = null;

        public bool IsReady { get { return timer <= 0 && hostTitan != null && hostTitan.IsAlive; } }

        public void OnAttach(Titan titan) {
            Debug.LogError("Build laser!");
            hostTitan = titan;
        }

        public void OnDetach() {
            hostTitan = null;
        }

        public void Update(float deltaTime) {
            ProcessReload(deltaTime);
            if (!IsReady)
                return;
            target = FindTarget(target);
            if (target == null)
                return;
            Fire(target);
        }

        public void Fire(Titan enemyTitan) {
            if (UnityEngine.Random.Range(0f, 1f) < accuracy) {
                enemyTitan.Hit(damage);
            }
            timer = reloadTime + UnityEngine.Random.Range(0f, reloadTimeRandomShift);
        }

        private Titan FindTarget(Titan currentTarget) {
            if (currentTarget != null && currentTarget.IsAlive) {
                if (Vector3.Distance(currentTarget.Position, hostTitan.Position) < fireRadius) {
                    return currentTarget;
                }
            }
            return FindInFireRange();
        }

        private Titan FindInFireRange() {
            var position = hostTitan.Position;
            Titan nearestEnemy = null;
            float nearestDistance = 0;
            foreach (var enemyFaction in hostTitan.Faction.EnemyFactions) {
                foreach (var enemyTitan in enemyFaction.Units) {
                    if (enemyTitan == null)
                        continue;
                    if (!enemyTitan.IsAlive)
                        continue;
                    var distance = Vector3.Distance(enemyTitan.Position, position);
                    if (distance < fireRadius && (distance < nearestDistance || nearestEnemy == null)) {
                        nearestDistance = distance;
                        nearestEnemy = enemyTitan;
                    }
                }
            }
            return nearestEnemy;
        }

        private void ProcessReload(float deltaTime) {
            if (timer > 0) {
                timer -= deltaTime;
            }
        }
    }
}
