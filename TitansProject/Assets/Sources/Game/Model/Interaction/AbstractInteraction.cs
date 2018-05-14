using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public abstract class AbstractInteraction {
        public readonly Titan ParentTitan;

        public AbstractInteraction(Titan parentTitan) {
            ParentTitan = parentTitan;
        }

        public abstract bool IsEnded { get; }
        public abstract void Update(float deltaTime);
    }
}
