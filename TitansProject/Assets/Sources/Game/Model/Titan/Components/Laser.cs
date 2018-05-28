using UnityEngine;

namespace Model {
    public sealed class Laser {
        private readonly Titan titan;
        private readonly Battle battle;
        private int damage = 0;
        private float fireRadius = 2f;
        private float accuracy = 0.5f;
        private float reloadTime = 1.5f;
        private float reloadTimeRandomShift = 0.2f;

        private float timer = 0f;
        private Titan target = null;
        public string Id { get; private set; }

        public bool IsReady { get { return timer <= 0 && titan != null && titan.IsAlive; } }

        public Laser(Titan parentTitan, Battle battleController) {
            titan = parentTitan;
            battle = battleController;
        }

        public void Update(float deltaTime) {
            if (damage == 0)
                return;
            ProcessReload(deltaTime);
            if (!IsReady)
                return;
            target = FindTarget(target);
            if (target == null)
                return;
            Fire(target);
        }

        internal void SetDamate(int additionalDamage) {
            damage += additionalDamage;
        }

        public void Fire(Titan enemyTitan) {
            if (Random.Range(0f, 1f) < accuracy) {
                battle.AddInteraction(new LaserInteraction(titan, enemyTitan, new Damage(DamageType.Heat, damage)));
                // enemyTitan.Hit(damage);
            }
            timer = reloadTime + Random.Range(0f, reloadTimeRandomShift);
        }

        private Titan FindTarget(Titan currentTarget) {
            if (currentTarget != null && currentTarget.IsAlive) {
                if (Vector3.Distance(currentTarget.Position, titan.Position) < fireRadius) {
                    return currentTarget;
                }
            }
            return FindInFireRange();
        }

        private Titan FindInFireRange() {
            var position = titan.Position;
            Titan nearestEnemy = null;
            float nearestDistance = 0;
            foreach (var enemyFaction in titan.Faction.EnemyFactions) {
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