using UnityEngine;
using Random = UnityEngine.Random;

namespace Model {
    public sealed class Laser: Titan.IComponent {
        private const float minimalMissChance = 0.1f; // 10%
        private const float minimalCriticalChance = 0.1f; // 10%

        private readonly Titan titan;
        private readonly Battle battle;
        private int damage = 0;
        private float fireRadius = 3f;
        private float reloadTime = 0.5f; // 1.7f;
        private float reloadTimeRandomShift = 0;//0.6f;

        private float timer = 0f;
        private Titan target = null;
        private Accuracy accuracy;

        public string Id { get; private set; }

        public bool IsReady { get { return timer <= 0 && titan != null && titan.IsAlive; } }

        public bool IsActive { get { return damage > 0; } }

        public float Radius { get { return fireRadius; } }

        public Laser(Titan parentTitan, Battle battleController, Accuracy titanAccuracy) {
            titan = parentTitan;
            battle = battleController;
            accuracy = titanAccuracy;
            // formulas test
           // var Accuracy = new Accuracy(10);
           // var Cloaking = new Cloaking(20);
           // const int allCount = 100000;
           // int countCrit = 0;
           // int countHit = 0;
           // for (int i = 0; i < allCount; i++) {
           //     if (GetCriticalChance(Accuracy, Cloaking)) {
           //         countCrit += 1;
           //     }
           //     if (GetHitChance(Accuracy, Cloaking)) {
           //         countHit += 1;
           //     }
           // }
           // int countAll = countCrit + countHit;
           // var crit = Mathf.RoundToInt(countCrit / (float)allCount * 100);
           // var hit = Mathf.RoundToInt(countHit / (float)allCount * 100);
           // var all = Mathf.RoundToInt(countAll / (float)allCount * 100);
           // Debug.LogError("crit " + crit + "% hit " + hit + "% all " + all + "%");
        }

        public void Update(float deltaTime) {
            if (!IsActive)
                return;
            ProcessReload(deltaTime);
            if (!IsReady)
                return;
            target = FindTarget(target);
            if (target == null)
                return;
            Fire(target);
        }

        public void Fire(Titan enemyTitan) {
            var isHit = GetHitChance(accuracy, enemyTitan.Cloaking);
            var isCritical = GetCriticalChance(accuracy, enemyTitan.Cloaking);
            var finalDamage = (isHit ? damage : 0) + (isCritical ? damage : 0);
            battle.AddInteraction(new LaserInteraction(titan, enemyTitan, new Damage(DamageType.Heat, finalDamage, isCritical && isHit)));
            timer = reloadTime + Random.Range(0f, reloadTimeRandomShift);
        }

        private bool GetCriticalChance(Accuracy accuracy, Cloaking cloaking) {
            var cloakAspect = (float)cloaking.Value / (float)accuracy.Value;
            var critChance = 1f - Mathf.Min(1f, cloakAspect);
            critChance = critChance * (1f - minimalCriticalChance) + minimalCriticalChance;
            return Random.Range(0f, 1f) < critChance;
        }

        private bool GetHitChance(Accuracy accuracy, Cloaking cloaking) {
            var accuracyAspect = (float)accuracy.Value / (float)cloaking.Value;
            var missChance = 1f - Mathf.Min(1f, accuracyAspect);
            missChance = missChance * (1f - minimalMissChance) + minimalMissChance;
            return Random.Range(0f, 1f) > missChance;
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

        public void OnAttach(ModuleData module) {
            damage += module["damage"];
        }

        public void OnDetach(ModuleData module) {
            damage -= module["damage"];
        }
    }
}