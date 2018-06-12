using UnityEngine;

namespace View {
    public sealed class ExplosionView : MonoBehaviour {
        [SerializeField]
        private float explosionSpeed = 0.4f;
        [SerializeField]
        private AnimationCurve explosionCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
        
        public void Init(Vector3 position, float maxRadius) {
            var radius = maxRadius;
            transform.position = position;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            Timer.Add(explosionSpeed * radius,
                (anim) => {
                    transform.localScale = Vector3.one * explosionCurve.Evaluate(anim) * radius;
                },
                () => {
                    gameObject.SetActive(false);
                });
        }
    }
}