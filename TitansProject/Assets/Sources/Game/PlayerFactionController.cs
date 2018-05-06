using Model;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using View;

/// <summary>
/// Base class for player controls his faction
/// </summary>
public sealed class PlayerFactionController : MonoBehaviour {
    [SerializeField]
    private Camera mainCamera;

    private Faction faction;
    private Battle battle;
    private TitanView selectedTitan = null;
    private Vector3 prevMousePosition;
    private GameUI gameUI;

    private TitanMoveMarkers moveMarkers;

    private void Awake() {
        gameUI = UILayer.Show<GameUI>();
        // TODO make normal
        moveMarkers = FindObjectOfType<TitanMoveMarkers>();
    }

    private void Start() {
        mainCamera = Camera.main;
    }

    public void Init(Battle currentBattle, Faction playerFaction) {
        battle = currentBattle;
        faction = playerFaction;
    }

    private void Update () {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0)) {
            prevMousePosition = Input.mousePosition;
        }
        const int minAccuracyPixels = 10;
        if (Input.GetMouseButtonUp(0) && (prevMousePosition - Input.mousePosition).sqrMagnitude < minAccuracyPixels * minAccuracyPixels) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            TitanView titan = null;
            if (Physics.Raycast(ray, out hit, 20f)) {
                titan = hit.collider.GetComponent<TitanView>();
                if (titan != null && (selectedTitan != titan)) {
                    SelectTitan(titan);
                }
                var planet = hit.collider.GetComponent<PlanetView>();
                if (planet != null && selectedTitan != null) {
                    var point = hit.point;
                    ResourcePoint resourcePoint;
                    selectedTitan.Titan.TryRemoveLastMoveTask();
                    if (planet.Planet.FindNearestResourcePoint(point, out resourcePoint)) {
                        selectedTitan.Titan.AddResourceTask(resourcePoint);
                    } else {
                        selectedTitan.Titan.AddMoveTask(point);
                    }
                }
            }
        }
       // if (Input.GetMouseButtonUp(1) && SelectedTitan != null && SelectedTitan.IsAlive && !RocketAimView.IsActive()) {
       //     var isCommandsQueue = false;
       //     if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
       //         isCommandsQueue = true;
       //     }
       //     if (!isCommandsQueue) {
       //         SelectedTitan.ClearTasks();
       //     }
       //     Vector3 clickPosition;
       //     if (planet.RaycastSurfacePoint(mainCamera.ScreenPointToRay(Input.mousePosition), out clickPosition)) {
       //         ResourcePointViewOld resourcePoint;
       //         if (planet.FindResourcePointClick(clickPosition, out resourcePoint)) {
       //             SelectedTitan.AddResourceTask(resourcePoint);
       //         } else {
       //             SelectedTitan.AddMoveTask(clickPosition);
       //         }
       //     }
       // }
    }

    private void SelectTitan(TitanView titanView) {
        if (selectedTitan != null) {
            selectedTitan.UpdateTaskList -= UpdateTaskList;
            selectedTitan.UpdateResources -= UpdateResources;
        }
        if (titanView == null || !titanView.Titan.IsAlive) {
            selectedTitan = null;
            gameUI.SelectTitan();
            moveMarkers.HideSelection();
            // RocketAimView.Hide();
            return;
        }
        selectedTitan = titanView;
        selectedTitan.UpdateTaskList += UpdateTaskList;
        selectedTitan.UpdateResources += UpdateResources;
        moveMarkers.SelectTitan(titanView);
        //RocketAimView.Hide();
        gameUI.SelectTitan(selectedTitan);
    }

    private void UpdateResources(int count) {
        gameUI.UpdateTitanStatus();
    }

    private void UpdateTaskList() {
        if (selectedTitan == null) {
            return;
        }
        moveMarkers.ShowPathMarkers(GetPathPoints(selectedTitan.Titan.TaskList));
    }

    private List<Vector3> GetPathPoints(List<Titan.Task> taskList) {
        var points = new List<Vector3>();
        if (taskList == null)
            return points;
        foreach (var task in taskList) {
            var moveTask = task as MoveTask;
            if (moveTask != null)
                points.Add(moveTask.Position);
        }
        return points;
    }
}
