using Model;
using UnityEngine;

namespace View {
    public sealed class ResourcePointView : MonoBehaviour {
        private ResourcePoint point;

        public void Init(ResourcePoint resourcePoint) {
            point = resourcePoint;
            point.OnUpdateResourcePoints += OnUpdate;
        }

        private void OnUpdate(int count) {
            if (count <= 0)
                DestroyResourcePoint();
        }

        private void DestroyResourcePoint() {
            if (point != null) {
                point.OnUpdateResourcePoints -= OnUpdate;
            }
            point = null;
            // Visualisation of destroy
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position - transform.position.normalized * 0.3f;
            Timer.Add(0.5f, (anim) => {
                if (this == null)
                    return;
                transform.position = Vector3.LerpUnclamped(startPosition, endPosition, anim);
            });
            Destroy(gameObject, 0.5f);
        }

        private void OnDestroy() {
            if (point != null) {
                point.OnUpdateResourcePoints -= OnUpdate;
            }
        }
    }
}
