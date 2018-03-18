using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TitanView : MonoBehaviour {
    [SerializeField]
    public float speed = 2;
    [SerializeField]
    public GameObject moveMarker;

    private Faction faction;

    float moveTimer = 0;
    float moveTime = 0;
    Vector3 endPosition;
    Vector3 moveAxe;
    float angle;
    Quaternion currentPosition;
    Quaternion needPosition;

    public void OnMoveClick(Vector3 targetPosition) {
        if (moveMarker != null) {
            moveMarker.transform.position = targetPosition;
        }
        endPosition = targetPosition.normalized;
        var startPosition = transform.localPosition.normalized;
        moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
        angle = Vector3.Angle(startPosition, endPosition);
        float distance = angle / 180f * Mathf.PI * 2f * 10f;
        moveTime = distance / speed;
        moveTimer = moveTime;
    }

    private void Start () {
        moveMarker.transform.parent = transform.parent;
    }

	private void Update () {
        if (moveTimer > 0) {
            moveTimer -= Time.deltaTime;
            var rotation = Quaternion.AngleAxis(angle * moveTimer / moveTime, moveAxe);
            transform.localPosition = rotation * endPosition * 10f;
            transform.rotation = Quaternion.LookRotation(rotation * endPosition) * Quaternion.Euler(90, 0, 0);
        }
	}

    public void Init(Faction controlFaction, Vector3 position) {
        faction = controlFaction;
        transform.localPosition = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }
}
