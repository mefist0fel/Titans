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

    private Faction[] factions;

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        gameUI.SetStatusText(string.Empty);
        factions = new Faction[] {
            new Faction(0),
            new Faction(1)
        };
        var position = planet.GetRandomPosition();
        factions[0].AddUnit(CreateTitan("Prefabs/titan"), position);
        factions[0].AddUnit(CreateTitan("Prefabs/titan"), Quaternion.Euler(5f, 0, 0) * position);
        factions[1].AddUnit(CreateTitan("Prefabs/titan_enemy"), planet.GetRandomPosition());
        CameraController.SetViewToTitan(factions[0].units[0].transform.position);
        factions[0].units[0].OnSelect();
        MoveController.SelectTitan(factions[0].units[0]);
    }

    private TitanView CreateTitan(string prefabName) {
        var titan = Instantiate(Resources.Load<TitanView>(prefabName), transform);
        return titan;
    }
    
    private void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 clickPosition;
            if (planet.GetSurfacePoint(mainCamera.ScreenPointToRay(Input.mousePosition), out clickPosition)) {
                ResourcePointView resourcePoint;
                if (planet.FindResourcePointClick(clickPosition, out resourcePoint)) {
                    factions[0].units[0].AddResourceTask(resourcePoint);
                } else {
                    factions[0].units[0].AddMoveTask(clickPosition);
                }
            }
        }
	}
}
