using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    [SerializeField]
    private PlanetView planet; // Set from editor

    [SerializeField]
    private Camera mainCamera; // Set from editor

    [SerializeField]
    public GameUI gameUI; // Set from editor

    [SerializeField]
    public TitanMoveMarkers MoveController; // Set from editor

    public TitanView SelectedTitan = null;

    private Faction[] factions;

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        gameUI.UpdateTitanStatus();
        factions = new Faction[] {
            new Faction(0),
            new Faction(1)
        };
        factions[0].EnemyFaction = factions[1];
        factions[1].EnemyFaction = factions[0];
        var position = planet.GetRandomPosition();
        factions[0].AddUnit(CreateTitan("Prefabs/titan"), position);
        factions[0].AddUnit(CreateTitan("Prefabs/titan"), Quaternion.Euler(20f, 0, 0) * position);
        factions[1].AddUnit(CreateTitan("Prefabs/titan_enemy"), Quaternion.Euler(-20f, 0, 0) * position); //planet.GetRandomPosition());
        foreach (var faction in factions) {
            foreach (var unit in faction.Units) {
                unit.Init(planet);
            }
        }
        CameraController.SetViewToTitan(factions[0].Units[0].transform.position);

        MoveController.HideSelection();
        //factions[0].units[0].OnSelect();
        // MoveController.SelectTitan(factions[0].units[0]);
    }

    public static void SelectNextTitan() {
        Instance.SelectNext();
    }

    private void SelectNext() {
        if (SelectedTitan == null) {
            SelectedTitan = factions[0].Units[0];
        }
        CameraController.MoveToTitan(SelectedTitan.Position);
    }

    private TitanView CreateTitan(string prefabName) {
        var titan = Instantiate(Resources.Load<TitanView>(prefabName), transform);
        return titan;
    }

    private Vector3 prevMousePosition;
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            prevMousePosition = Input.mousePosition;
        }
        const int minAccuracyPixels = 5;
        if (Input.GetMouseButtonUp(0) && (prevMousePosition - Input.mousePosition).sqrMagnitude < minAccuracyPixels * minAccuracyPixels) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            TitanView selectedTitan = null;
            if (Physics.Raycast(ray, out hit, 20f)) {
                selectedTitan = hit.collider.GetComponent<TitanView>();
                if (selectedTitan != null && (SelectedTitan == selectedTitan || selectedTitan.FactionId != 0)) {
                    selectedTitan = null;
                }
                SelectTitan(selectedTitan);
            }
        }
        if (Input.GetMouseButtonUp(1) && SelectedTitan != null && SelectedTitan.IsAlive) {
            var isCommandsQueue = false;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                isCommandsQueue = true;
            }
            if (!isCommandsQueue) {
                SelectedTitan.ClearTasks();
            }
            Vector3 clickPosition;
            if (planet.GetSurfacePoint(mainCamera.ScreenPointToRay(Input.mousePosition), out clickPosition)) {
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
            return;
        }
        SelectedTitan = titan;
        MoveController.SelectTitan(titan);
        gameUI.SelectTitan(titan);
        titan.OnSelect();
    }
}
