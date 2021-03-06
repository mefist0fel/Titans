﻿using Model;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using View;

/// <summary>
/// Base class for player controls his faction
/// </summary>
public sealed class PlayerFactionController : MonoBehaviour, Battle.IFactionController {
    [SerializeField]
    private Camera mainCamera;

    private Faction faction;
    private TitanView selectedTitan = null;
    private Vector3 prevMousePosition;
    private GameUI gameUI;
    private bool isInitialized = false;

    private TitanMoveMarkers moveMarkers;

    private void Awake() {
        gameUI = UILayer.Show<GameUI>();
        moveMarkers = FindObjectOfType<TitanMoveMarkers>();
    }

    public void OnAddTitan(TitanView titanView, PlanetView planetView) {
        gameUI.AddMarker(titanView, planetView);
    }

    public void OnRemoveTitan(TitanView titanView) {
        gameUI.RemoveMarker(titanView);
    }

    public void OnAddTitan(Titan titan) {
        if (!isInitialized)
            Initialize(titan);
    }

    private void Initialize(Titan titan) {
        CameraController.SetViewToTitan(titan.Position);
        isInitialized = true;
    }

    public void OnRemoveTitan(Titan titan) {}

    // TODO remove battle init
    public void Init(Battle currentBattle) {}

    public void Init(Faction playerFaction) {
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
        // Enemy titan
        if (titanView.Titan.Faction != faction) {
            return;
        }
        if (selectedTitan != null) {
            selectedTitan.UpdateTaskList -= UpdateTaskList;
            selectedTitan.UpdateResources -= UpdateResources;
        }
        selectedTitan = null;
        if (titanView == null || !titanView.Titan.IsAlive) {
            gameUI.SelectTitan();
            moveMarkers.HideSelection();
            // RocketAimView.Hide();
            return;
        }
        selectedTitan = titanView;
        selectedTitan.UpdateTaskList += UpdateTaskList;
        selectedTitan.UpdateResources += UpdateResources;
        selectedTitan.UpdateModules += UpdateModules;
        moveMarkers.SelectTitan(titanView);
        //RocketAimView.Hide();
        gameUI.SelectTitan(selectedTitan);
    }

    private void UpdateModules() {
        gameUI.UpdateTitanStatus();
    }

    private void UpdateResources(int count) {
        gameUI.UpdateTitanStatus();
    }

    private void UpdateTaskList() {
        if (selectedTitan == null) {
            return;
        }
        var taskPoints = GetPathPoints(selectedTitan.Titan.TaskList);
        moveMarkers.ShowPathMarkers(taskPoints);
        gameUI.ShowTaskCancelButtons(selectedTitan.Titan.TaskList);
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
