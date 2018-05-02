using Model;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    [SerializeField]
    private PlanetViewOld planet; // Set from editor

    public PlanetViewOld Planet { get { return planet; } }

    [SerializeField]
    private Camera mainCamera; // Set from editor

    [SerializeField]
    public TitanMoveMarkers MoveController; // Set from editor

    public TitanViewOld SelectedTitan = null;

    public OldFaction[] Factions;
    private GameUI gameUI; // Set from editor

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        gameUI = UILayer.Show<GameUI>();
        gameUI.UpdateTitanStatus();
        Factions = new OldFaction[] {
            new OldFaction(0),
            new OldFaction(1)
        };
        Factions[0].EnemyFaction = Factions[1];
        Factions[1].EnemyFaction = Factions[0];
        Factions[0].AddUnit(CreateTitan("Prefabs/titan"), new Vector3(0, planet.Radius, 0)); // planet.GetRandomPosition());
        Factions[1].AddUnit(CreateTitan("Prefabs/titan_enemy"), planet.GetRandomPosition());
        CameraController.SetViewToTitan(Factions[0].Units[0].transform.position);

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

    public TitanViewOld CreateTitan(string prefabName) {
        var titan = Instantiate(Resources.Load<TitanViewOld>(prefabName), transform);
        titan.Init(planet);
        return titan;
    }

    internal static bool CheckWinConditions() {
      //  if (Instance.Factions[1].ActiveUnitsCount <= 0) {
      //      return true;
      //  }
        return false;
    }

    internal static bool CheckLoseConditions() {
        if (Instance.Factions[0].ActiveUnitsCount <= 0) {
            return true;
        }
        return false;
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
            TitanViewOld selectedTitan = null;
            if (Physics.Raycast(ray, out hit, 20f)) {
                selectedTitan = hit.collider.GetComponent<TitanViewOld>();
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
                ResourcePointViewOld resourcePoint;
                if (planet.FindResourcePointClick(clickPosition, out resourcePoint)) {
                    SelectedTitan.AddResourceTask(resourcePoint);
                } else {
                    SelectedTitan.AddMoveTask(clickPosition);
                }
            }
        }
    }

    private void SelectTitan(TitanViewOld titan) {
        if (titan == null || !titan.IsAlive) {
            SelectedTitan = null;
            MoveController.HideSelection();
            gameUI.SelectTitan();
            RocketAimView.Hide();
            return;
        }
        SelectedTitan = titan;
        RocketAimView.Hide();
       // MoveController.SelectTitan(titan);
       // gameUI.SelectTitan(titan);
        titan.OnSelect();
    }
}
