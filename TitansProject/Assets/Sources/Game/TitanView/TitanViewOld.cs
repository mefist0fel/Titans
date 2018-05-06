using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public sealed class TitanViewOld : MonoBehaviour {
    [SerializeField]
    private Transform fireTransfom;
    [SerializeField]
    private Transform hitTransfom;
    [SerializeField]
    private Material deathMaterial;
    [SerializeField]
    public float Speed = 2;
    [SerializeField]
    public float ResourceCollectionTime = 0.5f;
    [SerializeField]
    private ShieldView shieldView; // Set from editor
    [SerializeField]
    public LivesView livesView; // Set from editor

    [SerializeField]
    public Collider TitanCollider; // Set from editor

    public int FactionId;
    public OldFaction SelfFaction;

    private Shield shield;

    public Shield ShieldGenerator { get { return shield; } }
    public int MaxShield = 20;
    private int armor = 10;
    public int Armor { get { return armor; } }

    private int maxArmor = 10;

    public int MaxArmor { get { return maxArmor; } }
    private int resourceUnits = 10;
    public int ResourceUnits { get { return resourceUnits; } }
    public int Level = 0;
    public const int MaxLevel = 4;


    private int[] slotLevel = new int[] {
        0, 0, 0, 0,
        1, 1, 2, 2,
        3, 3, 4, 4,
        0, // titan build module
        0,  // Level Up
        0   // Rocket
    };
    public int[] SlotLevel { get { return slotLevel; } }

    private ITitanModule[] modules = new ITitanModule[15];
    public ITitanModule[] Modules { get { return modules; } }

    private List<IModificator> modificators = new List<IModificator>();

    private PlanetViewOld planet;
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

    public void FireRocket(Vector3 fireCoord, PlanetViewOld planet) {
        RocketLauncherModule maximal = null;
        foreach (var module in modules) {
            var rocketModule = module as RocketLauncherModule;
            if (rocketModule != null && (maximal == null || rocketModule.RocketCount < maximal.RocketCount)) {
                maximal = rocketModule;
            }
        }
        if (maximal != null) {
            maximal.Fire(fireCoord, planet);
        }
        UpdateState();
    }

    public int GetComponentMaxRocketsCount() {
        var count = 0;
        foreach (var module in modules) {
            var rocketModule = module as RocketLauncherModule;
            if (rocketModule != null) {
                count += rocketModule.MaxRocketCount;
            }
        }
        return count;
    }

    public int GetComponentRocketsCount() {
        var count = 0;
        foreach (var module in modules) {
            var rocketModule = module as RocketLauncherModule;
            if (rocketModule != null) {
                count += rocketModule.RocketCount;
            }
        }
        return count;
    }

    public void AddRocket(int count) {
        RocketLauncherModule minimal = null;
        foreach (var module in modules) {
            var rocketModule = module as RocketLauncherModule;
            if (rocketModule != null && (minimal == null || rocketModule.RocketCount < minimal.RocketCount)) {
                minimal = rocketModule;
            }
        }
        if (minimal != null) {
            minimal.RocketCount += count;
        }
        UpdateState();
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
        damage = shield.Hit(damage);
        armor -= damage;
        if (armor <= 0) {
            armor = 0;
            Die();
        }
        UpdateState();
    }

    private void Die() {
        Debug.LogError("Oh, I'm dying");
        livesView.gameObject.SetActive(false);
        UpdateState();
        ClearTasks();
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
        shieldView.UpdateState(shield.Value);
        if (livesView != null)
            livesView.UpdateState(Armor, MaxArmor, shield.Value, shield.MaxValue);
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

    public interface IAction { }

    public sealed class MoveAction : IAction {
        public readonly Vector3 Position;
        public MoveAction(Vector3 position) {
            Position = position;
        }
    }

    public sealed class ResourceAction : IAction {
        public readonly ResourcePointViewOld ResourcePoint;
        public ResourceAction(ResourcePointViewOld point) {
            ResourcePoint = point;
        }
    }

    public void ClearTasks() {
        actions.Clear();
    }

    public void AddResourceTask(ResourcePointViewOld resourcePoint) {
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

    public void Init(PlanetViewOld planetView) {
        planet = planetView;
    }

    public void Init(OldFaction faction, Vector3 position) {
        SelfFaction = faction;
        FactionId = faction.ID;
        transform.localPosition = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }

    private void Start () {
        shield = new Shield(this);
        UpdateState();
        // Attach(ModulesFactory.CreateModule(Config.Modules["weapon"], this), 0);
    }

    public void BuildTitan(ModuleDataOld module) {
        if (module == null) {
            Debug.LogError("Module data is empty");
            return;
        }
        if (module.Cost > ResourceUnits) {
            Debug.LogError("Not enought RU for build");
            return;
        }
        resourceUnits -= module.Cost;
        Attach(ModulesFactory.CreateTitanBuildModule(module), 12);
    }

    public void BuildUpgrade(ModuleDataOld module) {
        if (module == null) {
            Debug.LogError("Module data is empty");
            return;
        }
        if (module.Cost > ResourceUnits) {
            Debug.LogError("Not enought RU for build");
            return;
        }
        resourceUnits -= module.Cost;
        Attach(ModulesFactory.CreateTitanUpgradeModule(module), 13);
    }

    public void BuildModule(ModuleDataOld module, int slotId) {
        if (module == null) {
            Debug.LogError("Module data is empty");
            return;
        }
        if (module.Cost > ResourceUnits) {
            Debug.LogError("Not enought RU for build");
            return;
        }
        resourceUnits -= module.Cost;
        Attach(ModulesFactory.CreateBuildModule(module, slotId), slotId);
    }

    public void BuildRocket(ModuleDataOld module) {
        if (module == null) {
            Debug.LogError("Module data is empty");
            return;
        }
        if (module.Cost > ResourceUnits) {
            Debug.LogError("Not enought RU for build");
            return;
        }
        resourceUnits -= module.Cost;
        Attach(ModulesFactory.CreateRocketModule(module), 14);
    }

    public void Attach(ITitanModule module, int slotId) {
        if (slotId < 0 || slotId >= modules.Length) {
            Debug.LogError("Slot id is out of range " + slotId);
            return;
        }
        if (modules[slotId] != null) {
            modules[slotId].OnDetach();
        }
        modules[slotId] = module;
        if (modules[slotId] != null) {
            modules[slotId].OnAttach(this);
        }
        UpdateState();
    }

    public void AddModificator(IModificator modificator) {
        modificators.Add(modificator);
        UpdateModificators();
    }

    public void RemoveModificator(IModificator modificator) {
        modificators.Remove(modificator);
        UpdateModificators();
    }

    private void UpdateModificators() {
        shield.UpdateModificators(modificators);
    }

    private void Update () {
        shield.Update(Time.deltaTime);
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
        if (data.ResourcePoint == null) {
            actionTimer = 0;
        }
        if (actionTimer <= 0) {
            if (data.ResourcePoint != null && data.ResourcePoint.Count > 0) {
                data.ResourcePoint.Collect();
                resourceUnits += 1;
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
