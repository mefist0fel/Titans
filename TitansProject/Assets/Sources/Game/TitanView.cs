using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public sealed class TitanView : MonoBehaviour {
    [SerializeField]
    private Transform fireTransfom;
    [SerializeField]
    private Transform hitTransfom;
    [SerializeField]
    private Material deathMaterial;
    [SerializeField]
    private MeshRenderer ShieldMesh;
    [SerializeField]
    public float Speed = 2;
    [SerializeField]
    public float ResourceCollectionTime = 0.5f;

    [SerializeField]
    public Collider TitanCollider; // Set from editor

    public int FactionId;
    public Faction SelfFaction;

    [SerializeField]
    public WeaponModule Weapon; // Set from editor
    [SerializeField]
    public RocketLauncherModule RocketLauncher; // Set from editor
    [SerializeField]
    public AntiAirLaserModule AntiAir; // Set from editor

    public int Shield = 20;
    public int Armor = 20;
    public int EnergyUnits = 0;
    public int Level = 0;
    public const int MaxLevel = 4;

    private int[] slotLevel = new int[] {
        0, 0, 0, 0,
        1, 1, 2, 2,
        3, 3, 4, 4
    };

    private ITitanModule[] modules = new ITitanModule[12];

    private PlanetView planet;
    private List<IAction> actions = new List<IAction>();
    private Action onUpdateAction = null;

    public IAction CurrentAction {
        get {
            if (actions.Count > 0)
                return actions[0];
            return null;
        }
    }

    public Vector3 Position {
        get {
            return transform.position;
        }
    }

    public bool IsAlive {
        get {
            return Armor > 0;
        }
    }

    public Vector3 GetHitPosition() {
        return hitTransfom.position + Random.insideUnitSphere * 0.1f;
    }

    public Vector3 GetFirePosition() {
        return fireTransfom.position;
    }

    public void Subscribe(Action onChangeStateAction) {
        onUpdateAction += onChangeStateAction;
    }

    public void UnSubscribe(Action onChangeState) {
        onUpdateAction -= onChangeState;
    }

    public void Hit(int damage) {
        StatusTextView.Create(damage.ToString(), Color.red, hitTransfom.position);
        Armor -= damage;
        if (Armor <= 0) {
            Die();
        }
        UpdateState();
    }

    private void Die() {
        Debug.LogError("Oh, I'm dying");
        if (ShieldMesh != null)
            ShieldMesh.gameObject.SetActive(false);
        UpdateState();
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers) {
            renderer.material = deathMaterial;
        }
    }

    public void OnSelect() { // TODO remove
    }

    public void UpdateState() {
        if (onUpdateAction != null)
            onUpdateAction();
    }

    public interface IAction {}

    public sealed class MoveAction : IAction {
        public readonly Vector3 Position;
        public MoveAction(Vector3 position) {
            Position = position;
        }
    }

    public List<Vector3> GetPathPoints() {
        var points = new List<Vector3>();
        foreach (var action in actions) {
            var moveAction = action as MoveAction;
            if (moveAction != null)
                points.Add(moveAction.Position);
        }
        return points;
    }

    public sealed class ResourceAction : IAction {
        public readonly ResourcePointView ResourcePoint;
        public ResourceAction(ResourcePointView point) {
            ResourcePoint = point;
        }
    }

    public void ClearTasks() {
        actions.Clear();
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
        UpdateState();
    }

    public void Init(PlanetView planetView) {
        planet = planetView;
    }

    public void Init(Faction faction, Vector3 position) {
        SelfFaction = faction;
        FactionId = faction.ID;
        transform.localPosition = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }

    private void Start () {
        //var weapon = TitanComponentFactory.AttachWeapon(this);
        //Components.Add(weapon);
        Weapon.Attach(this);
        RocketLauncher.Attach(this);
        AntiAir.Attach(this);
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
        if (currentAction is ResourceAction) {
            ProcessResourceCollection(currentAction as ResourceAction);
        }
    }

    private void ProcessResourceCollection(ResourceAction data) {
        if (actionTimer <= 0) {
            if (data.ResourcePoint != null && data.ResourcePoint.Count > 0) {
                data.ResourcePoint.Collect();
                EnergyUnits += 1;
                UpdateState();
                actionTimer = ResourceCollectionTime;
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
            actionTimer = ResourceCollectionTime;
        }
        UpdateState();
    }

    private void CalculateMoveTask(MoveAction action) {
        var startPosition = transform.localPosition;
        endPosition = action.Position;
        moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
        angle = Vector3.Angle(startPosition, endPosition);
        float distance = angle / 180f * Mathf.PI * 2f * planet.Radius;
        moveTime = distance / Speed;
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
}
