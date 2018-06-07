using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public enum DamageType {
        Kinetic,
        Heat,
        Electric
    }

    public struct Damage {
        public readonly DamageType Type;
        public readonly int Value;
        public readonly bool Critical;
        public Damage(DamageType type, int value, bool critical = false) {
            Type = type;
            Value = value;
            Critical = critical;
        }
    }
}
