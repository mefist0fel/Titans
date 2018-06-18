using System;
using UnityEngine;

public class Leg : MonoBehaviour {
    [SerializeField]
    private float fullTime = 0.08f;
    [SerializeField]
    private float correctionTime = 0.06f;

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
            currentAbsolutePosition = Vector3.Lerp(transform.position + correctionVector, prevAbsolutePosition, normalizedTime);
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentAbsolutePosition, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + correctionVector, 0.06f);
    }
}
