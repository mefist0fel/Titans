using UnityEngine;

namespace Model {
    public sealed class RocketInteraction : AbstractInteraction {
        public readonly Titan TargetTitan;
        public readonly Damage Damage;
        
        private readonly float flyTime;
        private float timer;
        private float delayTimer;

        public RocketInteraction(Titan parentTitan, Titan targetTitan, Damage damage, float speed, float delay) : base(parentTitan) {
            TargetTitan = targetTitan;
            Damage = damage;
            flyTime = Vector3.Distance(parentTitan.Position, targetTitan.Position) / speed;
            timer = flyTime;
            delayTimer = delay;
        }

        public override bool IsEnded {
            get {
                return timer <= 0;
            }
        }

        public override void Update(float deltaTime) {
            if (delayTimer > 0) {
                delayTimer -= deltaTime;
                return;
            }
            timer -= deltaTime;
            if (timer > 0)
                return;
            TargetTitan.Hit(Damage);
        }

    }
}
