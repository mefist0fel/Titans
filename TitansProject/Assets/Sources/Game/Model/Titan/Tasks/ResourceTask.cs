using System;
using UnityEngine;

namespace Model {
    public sealed class ResourceTask : Titan.Task {
        private readonly ResourcePoint resourcePoint;
        private readonly Action<int> OnCollectResource;
        private float resourceCollectionTime = 0.5f; // TODO to settings
        private float collectionTimer = 0;

        public override Vector3 Position {
            get {
                return resourcePoint.Position;
            }
        }

        public ResourceTask(ResourcePoint point, Action<int> onCollectResource) {
            resourcePoint = point;
            collectionTimer = resourceCollectionTime;
            OnCollectResource = onCollectResource;
        }

        public override bool IsEnded {
            get {
                return resourcePoint.Count <= 0;
            }
        }

        public override void MakeTask(float deltaTime) {
            collectionTimer -= deltaTime;
            if (collectionTimer > 0)
                return;
            collectionTimer = resourceCollectionTime;
            var collected = resourcePoint.Collect(1);
            OnCollectResource(collected);
        }
    }
}
