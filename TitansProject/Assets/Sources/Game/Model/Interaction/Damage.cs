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
        public Damage(DamageType type, int value) {
            Type = type;
            Value = value;
        }
    }
}
