using System;

namespace Model {
    public sealed class LaserInteraction : AbstractInteraction {
        private float timer;
        public readonly Titan TargetTitan;

        public LaserInteraction(Titan parentTitan, Titan targetTitan, Damage damage, float time = 0): base (parentTitan) {
            TargetTitan = targetTitan;
        }

        public override bool IsEnded {
            get {
                throw new NotImplementedException();
            }
        }

        public override void Update(float deltaTime) {
            throw new NotImplementedException();
        }
    }
}
