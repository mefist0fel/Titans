using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class TitanView : MonoBehaviour {
    [SerializeField]
    public float speed = 2;

    private Faction faction;
    private PlanetView planet;
    private List<IAction> actions = new List<IAction>();

    public IAction CurrentAction {
        get {
            if (actions.Count > 0)
                return actions[0];
            return null;
        }
    }

    public interface IAction {
    }


    public sealed class MoveAction : IAction {
        public readonly Vector3 Position;
        public MoveAction(Vector3 position) {
            Position = position;
        }
    }

    public void AddMovePoint(Vector3 position) {

    }

    public void OnMoveClick(Vector3 targetPosition) {
        actions.Add(new MoveAction(targetPosition));
        if (actions.Count == 1)
            StartNewAction(CurrentAction);
    }

    public void Init(PlanetView planetView) {
        planet = planetView;
    }

    private void Start () {
    }

	private void Update () {
        if (CurrentAction != null) {
            ProcessCurrentAction(CurrentAction);
        }
	}

    private void ProcessCurrentAction(IAction currentAction) {
        if (currentAction is MoveAction) {
            ProcessMove();
        }
    }

    private void StartNewAction(IAction currentAction) {
        if (currentAction is MoveAction) {
            CalculateMoveTask(currentAction as MoveAction);
        }
    }

    private void CalculateMoveTask(MoveAction action) {
        var startPosition = transform.localPosition;
        endPosition = action.Position;
        moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
        angle = Vector3.Angle(startPosition, endPosition);
        float distance = angle / 180f * Mathf.PI * 2f * 10f;
        moveTime = distance / speed;
        moveTimer = moveTime;
    }

    Vector3 endPosition;
    Vector3 moveAxe;
    float moveTimer = 0;
    float moveTime = 0;
    float angle;
    Quaternion currentPosition;
    Quaternion needPosition;

    private void ProcessMove() {
        if (moveTimer <= 0) {
            actions.RemoveAt(0);
            StartNewAction(CurrentAction);
        }
        moveTimer -= Time.deltaTime;
        var rotation = Quaternion.AngleAxis(angle * moveTimer / moveTime, moveAxe);
        transform.localPosition = rotation * endPosition;
        transform.rotation = Quaternion.LookRotation(rotation * endPosition) * Quaternion.Euler(90, 0, 0);
    }

    public void Init(Faction controlFaction, Vector3 position) {
        faction = controlFaction;
        transform.localPosition = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }
}
