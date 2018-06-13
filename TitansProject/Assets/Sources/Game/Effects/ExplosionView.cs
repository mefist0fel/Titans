using UnityEngine;

namespace View {
    public sealed class ExplosionView : MonoBehaviour {
        [SerializeField]
        private float explosionSpeed = 0.4f;
        [SerializeField]
        private AnimationCurve explosionCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
        
        public void Init(Vector3 position, float maxRadius) {
            Init(position, Vector3.zero, maxRadius);
        }

        public void Init(Vector3 position, Vector3 velocityPerSecond, float maxRadius) {
            var radius = maxRadius;
            transform.position = position;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            var time = explosionSpeed * radius;
            Vector3 startPosition = transform.position;
            Timer.Add(time,
                (anim) => {
                    transform.localScale = Vector3.one * explosionCurve.Evaluate(anim) * radius;
                    transform.position = startPosition + velocityPerSecond * (anim * time);
                },
                () => {
                    gameObject.SetActive(false);
                });
        }
    }
}