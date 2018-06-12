using Model;
using UnityEngine;

namespace View {
    public sealed class RocketView : MonoBehaviour {
        [SerializeField]
        private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private AnimationCurve heightCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0,0), new Keyframe(0.5f, 1), new Keyframe(1, 0)});
        private RocketInteraction interaction;
        private TitanView target;
        private Vector3 startPosition;
        private Vector3 prevPosition;
        private Vector3 heightVector;

        public void Init(RocketInteraction rocketInteraction) {
            interaction = rocketInteraction;
            var titanView = rocketInteraction.ParentTitan.View as TitanView;
            startPosition = titanView.GetHitPoint();

            target = rocketInteraction.TargetTitan.View as TitanView;
            // magic
            float maxHeight = Vector3.Distance(target.Titan.Position, titanView.Titan.Position) / 4f;
            heightVector = ((titanView.Titan.Position + target.Titan.Position).normalized + Random.insideUnitSphere * 0.3f) * maxHeight;
            gameObject.SetActive(true);
            prevPosition = startPosition;
            Update();
        }

        private void Update() {
            if (interaction == null) {
                gameObject.SetActive(false);
                return;
            }
            var curvedTime = speedCurve.Evaluate(1f - interaction.NormalizedTime);
            var position = Vector3.Lerp(startPosition, target.GetHitPoint(), curvedTime) + heightVector * heightCurve.Evaluate(curvedTime);
            Quaternion rotation = Quaternion.LookRotation(prevPosition - position) * Quaternion.Euler(90, 0, 0);
            prevPosition = position;
            transform.rotation = rotation;
            transform.position = position;
            if (interaction.IsEnded) {
                interaction = null;
            }
        }
    }
}