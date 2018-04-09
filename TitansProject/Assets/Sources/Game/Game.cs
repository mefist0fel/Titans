using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    [SerializeField]
    private PlanetView planet; // Set from editor

    public PlanetView Planet { get { return planet; } }

    [SerializeField]
    private Camera mainCamera; // Set from editor

    [SerializeField]
    public GameUI gameUI; // Set from editor

    [SerializeField]
    public TitanMoveMarkers MoveController; // Set from editor

    public TitanView SelectedTitan = null;

    public Faction[] Factions;

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        gameUI.UpdateTitanStatus();
        Factions = new Faction[] {
            new Faction(0),
            new Faction(1)
        };
        Factions[0].EnemyFaction = Factions[1];
        Factions[1].EnemyFaction = Factions[0];
        Factions[0].AddUnit(CreateTitan("Prefabs/titan"), planet.GetRandomPosition());
        Factions[1].AddUnit(CreateTitan("Prefabs/titan_enemy"), planet.GetRandomPosition());
        CameraController.SetViewToTitan(Factions[1].Units[0].transform.position);

        MoveController.HideSelection();
    }

    public static void SelectNextTitan() {
        Instance.SelectNext();
    }

    private void SelectNext() {
        if (SelectedTitan == null) {
            SelectedTitan = Factions[0].Units[0];
        }
        CameraController.MoveToTitan(SelectedTitan.Position);
    }

    public TitanView CreateTitan(string prefabName) {
        var titan = Instantiate(Resources.Load<TitanView>(prefabName), transform);
        titan.Init(planet);
        return titan;
    }

    public static void OnSelectRocketStrike() {
        RocketAimView.Show(OnSelectRocketStrike, 1f);
    }

    private static void OnSelectRocketStrike(Vector3 fireCoord) {
        if (Instance.SelectedTitan == null)
            return;
        Instance.SelectedTitan.FireRocket(fireCoord, Instance.planet);
    }

    private Vector3 prevMousePosition;
    private void Update() {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0)) {
            prevMousePosition = Input.mousePosition;
        }
        const int minAccuracyPixels = 10;
        if (Input.GetMouseButtonUp(0) && (prevMousePosition - Input.mousePosition).sqrMagnitude < minAccuracyPixels * minAccuracyPixels) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            TitanView selectedTitan = null;
            if (Physics.Raycast(ray, out hit, 20f)) {
                selectedTitan = hit.collider.GetComponent<TitanView>();
                if (selectedTitan != null && (SelectedTitan == selectedTitan)) {
                    selectedTitan = null;
                }
                SelectTitan(selectedTitan);
            }
        }
        if (Input.GetMouseButtonUp(1) && SelectedTitan != null && SelectedTitan.IsAlive && !RocketAimView.IsActive()) {
            var isCommandsQueue = false;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                isCommandsQueue = true;
            }
            if (!isCommandsQueue) {
                SelectedTitan.ClearTasks();
            }
            Vector3 clickPosition;
            if (planet.RaycastSurfacePoint(mainCamera.ScreenPointToRay(Input.mousePosition), out clickPosition)) {
                ResourcePointView resourcePoint;
                if (planet.FindResourcePointClick(clickPosition, out resourcePoint)) {
                    SelectedTitan.AddResourceTask(resourcePoint);
                } else {
                    SelectedTitan.AddMoveTask(clickPosition);
                }
            }
        }
    }

    private void SelectTitan(TitanView titan) {
        if (titan == null || !titan.IsAlive) {
            SelectedTitan = null;
            MoveController.HideSelection();
            gameUI.SelectTitan();
            RocketAimView.Hide();
            return;
        }
        SelectedTitan = titan;
        RocketAimView.Hide();
        MoveController.SelectTitan(titan);
        gameUI.SelectTitan(titan);
        titan.OnSelect();
    }
}
