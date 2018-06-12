using System;
using UnityEngine;

namespace Model {
    public sealed class Laser: Titan.IComponent {

        private readonly Titan titan;
        private readonly Battle battle;
        private int damage = 0;
        private float fireRadius = 3f;

        private readonly ReloadTimer timer;

        private Titan target = null;
        private Accuracy accuracy;

        public bool IsActive { get { return damage > 0; } }

        public float Radius { get { return fireRadius; } }

        public Laser(Titan parentTitan, Battle battleController, Accuracy titanAccuracy) {
            titan = parentTitan;
            battle = battleController;
            accuracy = titanAccuracy;
            timer = new ReloadTimer(2f, 0.3f);
            // TestFormulas();
        }

        public void Update(float deltaTime) {
            if (!IsActive)
                return;
            timer.Update(deltaTime);
            if (!timer.IsReady)
                return;
            target = FindTarget(target);
            if (target == null)
                return;
            Fire(target);
        }

        private Titan FindTarget(Titan currentTarget) {
            if (currentTarget != null && currentTarget.IsAlive) {
                if (Vector3.Distance(currentTarget.Position, titan.Position) < fireRadius) {
                    return currentTarget;
                }
            }
            return titan.FindEnemyInRange(fireRadius);
        }

        public void Fire(Titan enemyTitan) {
            var isHit = accuracy.GetHitChance(enemyTitan.Cloaking);
            var isCritical = accuracy.GetCriticalChance(enemyTitan.Cloaking);
            var finalDamage = (isHit ? damage : 0) + (isCritical ? damage : 0);
            battle.AddInteraction(new LaserInteraction(titan, enemyTitan, new Damage(DamageType.Heat, finalDamage, isCritical && isHit)));
            timer.Reload();
        }

        public void OnAttach(ModuleData module) {
            damage += module["damage"];
        }

        public void OnDetach(ModuleData module) {
            damage -= module["damage"];
        }

        private void TestFormulas() {
            var Accuracy = new Accuracy(10);
            var Cloaking = new Cloaking(20);
            const int allCount = 100000;
            int countCrit = 0;
            int countHit = 0;
            for (int i = 0; i < allCount; i++) {
                if (Accuracy.GetCriticalChance(Cloaking)) {
                    countCrit += 1;
                }
                if (Accuracy.GetHitChance(Cloaking)) {
                    countHit += 1;
                }
            }
            int countAll = countCrit + countHit;
            var crit = Mathf.RoundToInt(countCrit / (float)allCount * 100);
            var hit = Mathf.RoundToInt(countHit / (float)allCount * 100);
            var all = Mathf.RoundToInt(countAll / (float)allCount * 100);
            Debug.LogError("crit " + crit + "% hit " + hit + "% all " + all + "%");
        }
    }
}