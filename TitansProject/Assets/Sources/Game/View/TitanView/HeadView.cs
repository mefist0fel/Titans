using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HeadView : MonoBehaviour {
    [SerializeField]
    private Transform headTransform = null;
    [SerializeField]
    private float startAngle = 0f; // per second
    [SerializeField]
    private float rotationSpeed = 360f; // per second

    private float currentRotation = 0;
    private float needRotation = 0;

    public void SetAngle(Vector3 position) {
        Vector3 positionDelta = transform.position - position;
        Vector3 ObjectTargetPosition = Quaternion.Inverse(transform.parent.rotation) * positionDelta;
        needRotation = Utils.GetAngle(new Vector2(ObjectTargetPosition.x, -ObjectTargetPosition.y));
    }

    private void Start () {}
	
	private void Update () {
        currentRotation = Utils.RotateToAngle(currentRotation, needRotation, rotationSpeed * Time.deltaTime);
        if (headTransform != null) {
            headTransform.localEulerAngles = new Vector3(0, 0, currentRotation + startAngle);
        }
	}
}
