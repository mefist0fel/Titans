using System;

namespace Model {
    public sealed class LaserInteraction : AbstractInteraction {
        public readonly Titan TargetTitan;
        public readonly Damage Damage;

        public LaserInteraction(Titan parentTitan, Titan targetTitan, Damage damage, float time = 0): base (parentTitan) {
            TargetTitan = targetTitan;
            Damage = damage;
        }

        public override bool IsEnded {
            get {
                return true;
            }
        }

        public override void Update(float deltaTime) {
            TargetTitan.Hit(Damage);
        }
    }
}
