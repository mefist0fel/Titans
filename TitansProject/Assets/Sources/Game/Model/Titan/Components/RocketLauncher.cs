using UnityEngine;

namespace Model {
    public sealed class RocketLauncher : Titan.IComponent {
        private readonly Battle battle;
        private readonly Titan titan;

        private const float fireRadius = 5f;
        private int damage = 2;
        private int countInSalvo = 0;
        private const float delay = 0.12f;
        private readonly ReloadTimer timer = new ReloadTimer(2f, 0.5f);

        private Titan target = null;

        public float Radius { get { return fireRadius; } }

        public bool IsActive { get { return countInSalvo > 0; } }

        public RocketLauncher(Titan parentTitan, Battle battleController) {
            titan = parentTitan;
            battle = battleController;
        }

        public void OnAttach(ModuleData module) {
            countInSalvo += module["rocket_salvo"];
        }

        public void OnDetach(ModuleData module) {
            countInSalvo -= module["rocket_salvo"];
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
            for(var i = 0; i < countInSalvo; i++) {
                var rocketDamage = new Damage(DamageType.Heat, damage);
                var rocket = new RocketInteraction(titan, enemyTitan, rocketDamage);
                battle.AddInteraction(new DelayedInteraction(titan, i * delay, rocket));
            }
            timer.Reload();
        }
    }
}
