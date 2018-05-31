using UnityEngine;

namespace UI {
    public sealed class UIScaleAnimator : MonoBehaviour {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private float animationTime = 0.3f;
        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float normalizedTimer;

        private bool isShowed = true;

        public bool IsShowed {
            get {
                return isShowed;
            }
        }

        public void Show(bool show) {
            isShowed = show;
        }

        private void Awake () {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
        }

        private void Start() {
            SetScale(animationCurve.Evaluate(normalizedTimer));
        }

        private void Update() {
            AnimateScale();
        }

        private void AnimateScale() {
            if (isShowed) {
                if (normalizedTimer >= 1f)
                    return;
                normalizedTimer += Time.unscaledDeltaTime / animationTime;
            } else {
                if (normalizedTimer <= 0f)
                    return;
                normalizedTimer -= Time.unscaledDeltaTime / animationTime;
            }
            normalizedTimer = Mathf.Clamp01(normalizedTimer);
            SetScale(animationCurve.Evaluate(normalizedTimer));
        }

        private void SetScale(float scale) {
            rectTransform.localScale = Vector3.one * scale;
        }
    }
}
