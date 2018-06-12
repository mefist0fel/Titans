using System;
using UnityEngine;

namespace Model {
    public sealed class RocketInteraction : AbstractInteraction {
        public readonly Titan TargetTitan;
        public readonly Damage Damage;
        
        private readonly float flyTime;
        private float timer;
        private const float interceptTime = 0.3f;
        private const float defaultSpeed = 1.5f; // units per second
        private bool interceptTry = false;
        public bool isIntercepted { get; private set; }

        public override bool IsEnded {
            get {
                return timer <= 0 || isIntercepted;
            }
        }

        public float NormalizedTime {
            get {
                return timer / flyTime;
            }
        }

        public RocketInteraction(Titan parentTitan, Titan targetTitan, Damage damage, float speed = defaultSpeed) : base(parentTitan) {
            TargetTitan = targetTitan;
            Damage = damage;
            flyTime = Vector3.Distance(parentTitan.Position, targetTitan.Position) / speed;
            timer = flyTime;
            isIntercepted = false;
        }

        public override void Update(float deltaTime) {
            timer -= deltaTime;
            if (timer > 0) {
                if (timer < interceptTime && !interceptTry) {
                    TryIntercept();
                }
                return;
            }
            if (!isIntercepted)
                TargetTitan.Hit(Damage);
        }

        private void TryIntercept() {
            if (TargetTitan.AntiAirDefence.Intercept()) {
                isIntercepted = true;
            }
            interceptTry = true;
        }
    }
}
