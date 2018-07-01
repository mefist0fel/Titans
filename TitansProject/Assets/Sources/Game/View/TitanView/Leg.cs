using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour {
    [SerializeField]
    private LegStepTarget target; // Set from editor
    [SerializeField]
    private Transform baseRotationTransform; // Set from editor
    [SerializeField]
    private Transform feetTransform; // Set from editor
    [SerializeField]
    private Transform firstSegmentTransform; // Set from editor
    [SerializeField]
    private Transform secondSegmentTransform; // Set from editor
    [SerializeField]
    private float firstSegmentLenght = 0.3f; // Set from editor
    [SerializeField]
    private float secondSegmentLenght = 0.3f; // Set from editor
    [SerializeField]
    private float baseAngle = -90; // Set from editor

    private void Update () {
        var positionDelta = transform.position - target.Target;
        Vector3 ObjectTargetPosition = Quaternion.Inverse(transform.parent.rotation) * positionDelta;
        var yawAngle = Utils.GetAngle(new Vector2(ObjectTargetPosition.x, ObjectTargetPosition.z));

        if (baseRotationTransform != null)
            baseRotationTransform.localRotation = Quaternion.Euler(0, yawAngle + baseAngle, 0);
        if (feetTransform != null) {
            //   var magnitude = positionDelta.magnitude;
            //   if (magnitude > firstSegmentLenght + secondSegmentLenght) {
            //       float height = positionDelta.y;
            //       positionDelta = positionDelta * ((firstSegmentLenght + secondSegmentLenght) / magnitude);
            //       positionDelta.y = height;
            //   }
            // feetTransform.localPosition = target.Target;
            feetTransform.position = target.Target;
            feetTransform.localRotation = Quaternion.Euler(0, yawAngle + baseAngle, 0);
        }

        if (firstSegmentTransform != null) {
            positionDelta = firstSegmentTransform.position - target.Target;
            ObjectTargetPosition = Quaternion.Inverse(transform.parent.rotation) * positionDelta;
            var pichAngleToTarget = GetPitchToTargetAngle(ObjectTargetPosition);
            float distanceToTarget = ObjectTargetPosition.magnitude;
            // distanceToTarget - a
            // secondSegmentLenght - b
            // firstSegmentLenght - c
            float pitchAngleToSecondSegment = Mathf.Acos(
                 (distanceToTarget * distanceToTarget + firstSegmentLenght * firstSegmentLenght - secondSegmentLenght * secondSegmentLenght) /
                 (2f * distanceToTarget * firstSegmentLenght)) / Mathf.PI * 180f;
            float pitchAngleToTargetSegment = Mathf.Acos(
                (secondSegmentLenght * secondSegmentLenght + firstSegmentLenght * firstSegmentLenght - distanceToTarget * distanceToTarget) /
                (2f * secondSegmentLenght * firstSegmentLenght)) / Mathf.PI * 180f;
            if (distanceToTarget >= firstSegmentLenght + secondSegmentLenght) {
                pitchAngleToSecondSegment = 0;
                pitchAngleToTargetSegment = 180;
            }
            if (firstSegmentTransform != null)
                firstSegmentTransform.localRotation = Quaternion.Euler(0, 0, pichAngleToTarget - pitchAngleToSecondSegment);
            if (secondSegmentTransform != null)
                secondSegmentTransform.localRotation = Quaternion.Euler(0, 0, 180f - pitchAngleToTargetSegment);
        }
    }

    private float GetPitchToTargetAngle(Vector3 objectSpaceDelta) {
        float DistanceToTarget = objectSpaceDelta.magnitude; // hypotenuse
        float DistanceToTargetProjection = new Vector2(objectSpaceDelta.x, objectSpaceDelta.z).magnitude; // adjanced side
        return Mathf.Acos(DistanceToTargetProjection / DistanceToTarget) / Mathf.PI * 180f;
    }

    // https://ru.wikipedia.org/wiki/Решение_треугольников
    //
    //      A
    //     / \
    //   c/   \b
    //   /     \
    //  /   a   \
    // B---------C
    //
    // A = acos (b^2 + c^2 - a^2 / 2bc )
    // B = acos (a^2 + c^2 - b^2 / 2ac )
    // C = acos (a^2 + b^2 - c^2 / 2ab ) or C = 180 - A - B

#if UNITY_EDITOR
    [ContextMenu("Get Lenght")]
    private void GetLenght() {
        Debug.Log((firstSegmentTransform.position - secondSegmentTransform.position).magnitude);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.01f);
        if (target != null)
            Gizmos.DrawWireSphere(target.Target, 0.01f);
        if (firstSegmentTransform != null)
            Gizmos.DrawWireSphere(firstSegmentTransform.position, 0.02f);
        if (secondSegmentTransform != null)
            Gizmos.DrawWireSphere(secondSegmentTransform.position, 0.01f);
        //  var objectRotation = transform.parent.rotation;
        //  var start = transform.position;
        //  var knee = start - objectRotation * kneePosition;
        //  var feet = start - objectRotation * feetPosition;
        //  Gizmos.DrawWireSphere(knee, 0.01f);
        //  Gizmos.DrawLine(start, knee);
        //  Gizmos.DrawLine(knee, feet);
        //  Gizmos.DrawWireSphere(feet, 0.01f);
    }
#endif
}
