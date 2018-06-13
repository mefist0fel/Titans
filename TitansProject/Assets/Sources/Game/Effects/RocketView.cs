using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View {
    public sealed class RocketView : MonoBehaviour {
        [SerializeField]
        private TrailRenderer trail; // Set from editor
        [SerializeField]
        private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private AnimationCurve heightCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0,0), new Keyframe(0.5f, 1), new Keyframe(1, 0)});
        [SerializeField]
        private float explosionDelay = 0.06f;
        [SerializeField]
        private float destroyDelay = 0.3f;
        private RocketInteraction interaction;
        private TitanView target;
        private Vector3 startPosition;
        private Vector3 prevPosition;
        private Vector3 heightVector;
        const float destroySpeed = 0.4f;

        public void Init(RocketInteraction rocketInteraction) {
            interaction = rocketInteraction;
            var titanView = rocketInteraction.ParentTitan.View as TitanView;
            startPosition = titanView.GetHitPoint();

            target = rocketInteraction.TargetTitan.View as TitanView;
            // magic
            float maxHeight = Vector3.Distance(target.Titan.Position, titanView.Titan.Position) / 4f;
            heightVector = ((titanView.Titan.Position + target.Titan.Position).normalized + Random.insideUnitSphere * 0.3f) * maxHeight;
            gameObject.SetActive(true);
            prevPosition = Vector3.Lerp(startPosition, target.GetHitPoint(), 1.1f);
            Update();
            if (trail != null)
                trail.Clear();
        }

        private void Update() {
            if (interaction == null) {
                return;
            }
            Vector3 position = GetPosition(1f - interaction.NormalizedTime);
            Quaternion rotation = Quaternion.LookRotation(prevPosition - position) * Quaternion.Euler(90, 0, 0);
            prevPosition = position;
            transform.rotation = rotation;
            transform.position = position;
            if (interaction.IsEnded) {
                var destroyVector = Vector3.zero;
                if (interaction.NormalizedTime > 0) {
                    var extrapolatedPosition = GetPosition((1f - interaction.NormalizedTime) + 0.01f / interaction.FlyTime);
                    destroyVector = (extrapolatedPosition - position) * 100f * destroySpeed;
                }
                const float explodeRadius = 0.15f;
                ExplosionPool.Explode(position, destroyVector, explodeRadius);
                interaction = null;
                Timer.Add(explosionDelay, (anim) => {
                    if (this == null)
                        return;
                    transform.position = transform.position + destroyVector * Time.deltaTime;
                }, () => {
                    if (this == null)
                        return;
                    Timer.Add(destroyDelay, () => {
                        if (this == null)
                            return;
                        gameObject.SetActive(false);
                    });
                });
            }
        }

        private Vector3 GetPosition(float normalizedTime) {
            var curvedTime = speedCurve.Evaluate(normalizedTime);
            return Vector3.Lerp(startPosition, target.GetHitPoint(), curvedTime) + heightVector * heightCurve.Evaluate(curvedTime);
        }
    }
}