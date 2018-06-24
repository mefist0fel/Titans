using System;
using UnityEngine;

public class LegStepTarget : MonoBehaviour {
    [SerializeField]
    private float fullTime = 0.08f;
    [SerializeField]
    private float correctionTime = 0.06f;
    [SerializeField]
    private float maxUpDistance = 0.3f;
    [SerializeField]
    private float minUpDistance = 0.02f;
    [SerializeField]
    private Vector3 upVector = new Vector3(0, 0.1f, 0);
    [SerializeField]
    private AnimationCurve moveCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1f, 0f) });

    public Vector3 Target {
        get {
            return currentAbsolutePosition;
        }
    }

    private Vector3 currentAbsolutePosition;
    private Vector3 prevAbsolutePosition;
    private float timer;
    private Vector3 prevPosition;
    private Vector3 correctionVector;

    private void Start () {
        currentAbsolutePosition = transform.position;
        prevPosition = transform.position;
    }
	
	private void Update () {
        UpdateCorrection();
        if (timer > 0) {
            timer -= Time.deltaTime;
            var normalizedTime = Mathf.Clamp01(timer / fullTime);
            var needPosition = transform.position + correctionVector;
            float moveDistance = Vector3.Distance(needPosition, prevAbsolutePosition);
            float moveUpDistance = Mathf.Clamp01((moveDistance - minUpDistance) / maxUpDistance);
            currentAbsolutePosition = Vector3.Lerp(needPosition, prevAbsolutePosition, normalizedTime) + transform.rotation * upVector * moveCurve.Evaluate(normalizedTime) * moveUpDistance;
        }
    }

    private void UpdateCorrection() {
        correctionVector = (transform.position - prevPosition) * (1f / Time.deltaTime * correctionTime);
        prevPosition = transform.position;
    }

    public void StartStep() {
        timer = fullTime;
        prevAbsolutePosition = currentAbsolutePosition;
    }

    public void StopStep() {}
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentAbsolutePosition, 0.02f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + correctionVector, 0.01f);
    }
#endif
}
