using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class TitanView : MonoBehaviour {
    [SerializeField]
    public float speed = 2;
    [SerializeField]
    public float resourceCollectionTime = 0.5f;

    public int Lives = 10;
    public int ResourceUnits = 0;

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

    public void OnSelect() {
        UpdateUI();
    }

    public interface IAction {}

    public sealed class MoveAction : IAction {
        public readonly Vector3 Position;
        public MoveAction(Vector3 position) {
            Position = position;
        }
    }

    public sealed class ResourceAction : IAction {
        public readonly ResourcePointView ResourcePoint;
        public ResourceAction(ResourcePointView point) {
            ResourcePoint = point;
        }
    }

    public void AddResourceTask(ResourcePointView resourcePoint) {
        actions.Add(new MoveAction(resourcePoint.transform.position));
        actions.Add(new ResourceAction(resourcePoint));
        if (actions.Count >= 1)
            StartNewAction(CurrentAction);
    }

    public void AddMoveTask(Vector3 targetPosition) {
        actions.Add(new MoveAction(targetPosition));
        if (actions.Count >= 1)
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

    private void UpdateUI() {
        Game.Instance.gameUI.SetStatusText("M: " + ResourceUnits);
    }

    private void ProcessCurrentAction(IAction currentAction) {
        if (currentAction is MoveAction) {
            ProcessMove();
        }
        if (currentAction is ResourceAction) {
            ProcessResourceCollection(currentAction as ResourceAction);
        }
    }

    private void ProcessResourceCollection(ResourceAction data) {
        if (actionTimer <= 0) {
            if (data.ResourcePoint != null && data.ResourcePoint.Count > 0) {
                data.ResourcePoint.Collect();
                ResourceUnits += 1;
                UpdateUI();
                actionTimer = resourceCollectionTime;
                if (data.ResourcePoint.Count > 0) {
                    return;
                }
            }
            actions.RemoveAt(0);
            StartNewAction(CurrentAction);
            return;
        }
        actionTimer -= Time.deltaTime;
    }

    private void StartNewAction(IAction currentAction) {
        if (currentAction is MoveAction) {
            CalculateMoveTask(currentAction as MoveAction);
        }
        if (currentAction is ResourceAction) {
            actionTimer = resourceCollectionTime;
        }
    }

    private void CalculateMoveTask(MoveAction action) {
        var startPosition = transform.localPosition;
        endPosition = action.Position;
        moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
        angle = Vector3.Angle(startPosition, endPosition);
        float distance = angle / 180f * Mathf.PI * 2f * 10f;
        moveTime = distance / speed;
        actionTimer = moveTime;
    }

    Vector3 endPosition;
    Vector3 moveAxe;
    float actionTimer = 0;
    float moveTime = 0;
    float angle;
    Quaternion currentPosition;
    Quaternion needPosition;

    private void ProcessMove() {
        if (actionTimer <= 0) {
            actions.RemoveAt(0);
            StartNewAction(CurrentAction);
            return;
        }
        actionTimer -= Time.deltaTime;
        if (actionTimer < 0)
            actionTimer = 0;
        AnimateMove(actionTimer / moveTime);
    }

    private void AnimateMove(float backTime) {
        var rotation = Quaternion.AngleAxis(angle * backTime, moveAxe);
        transform.localPosition = rotation * endPosition;
        transform.rotation = Quaternion.LookRotation(rotation * endPosition) * Quaternion.Euler(90, 0, 0);
    }

    public void Init(Faction controlFaction, Vector3 position) {
        faction = controlFaction;
        transform.localPosition = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }
}
