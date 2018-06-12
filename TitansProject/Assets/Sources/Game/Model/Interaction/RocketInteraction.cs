using UnityEngine;

namespace Model {
    public sealed class RocketInteraction : AbstractInteraction {
        public readonly Titan TargetTitan;
        public readonly Damage Damage;
        
        private readonly float flyTime;
        private float timer;

        public override bool IsEnded {
            get {
                return timer <= 0;
            }
        }

        public float NormalizedTime {
            get {
                return timer / flyTime;
            }
        }

        public RocketInteraction(Titan parentTitan, Titan targetTitan, Damage damage, float speed) : base(parentTitan) {
            TargetTitan = targetTitan;
            Damage = damage;
            flyTime = Vector3.Distance(parentTitan.Position, targetTitan.Position) / speed;
            timer = flyTime;
        }

        public override void Update(float deltaTime) {
            timer -= deltaTime;
            if (timer > 0)
                return;
            TargetTitan.Hit(Damage);
        }

    }
}
