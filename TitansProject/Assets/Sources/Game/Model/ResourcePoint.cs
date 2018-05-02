using System;
using UnityEngine;

namespace Model {
    public sealed class ResourcePoint {

        public readonly int MaxCount;
        public readonly Vector3 Position;

        public int Count { get; private set; }

        public Action<int> OnUpdateResourcePoints;

        public ResourcePoint(Vector3 position, int count) {
            Position = position;
            MaxCount = count;
            Count = count;
        }

        public int Collect(int count = 1) {
            if (Count >= count) {
                Count -= count;
                OnUpdate(Count);
                return count;
            }
            var collected = Count;
            Count = 0;
            OnUpdate(Count);
            return collected;
        }

        private void OnUpdate(int count) {
            if (OnUpdateResourcePoints != null) {
                OnUpdateResourcePoints(count);
            }
        }
    }
}