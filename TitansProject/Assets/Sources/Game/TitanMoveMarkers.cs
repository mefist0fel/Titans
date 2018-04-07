using System;
using System.Collections.Generic;
using UnityEngine;

public class TitanMoveMarkers : MonoBehaviour {
    [SerializeField]
    private SelectionMarkerView selectMarker; // Set from editor
    [SerializeField]
    private GameObject pathMarkerPrefab; // Set from editor

    private List<GameObject> pathMarkersCache = new List<GameObject>();

    public void SelectTitan(TitanView titan) {
        if (selectMarker != null) {
            selectMarker.SelectTitan(titan);
        }
    }

    public void HideSelection() {
        if (selectMarker != null) {
            selectMarker.HideSelection();
        }
        HidePathMarkers();
    }

    public void ShowPathMarkers(TitanView titan, List<Vector3> pathPoints) {
        HidePathMarkers();
        CreatePoints(pathPoints.Count);
        for (int i = 0; i < pathPoints.Count; i++) {
            pathMarkersCache[i].SetActive(true);
            pathMarkersCache[i].transform.localPosition = pathPoints[i];
            pathMarkersCache[i].transform.localRotation = Quaternion.LookRotation(pathPoints[i]) * Quaternion.Euler(90, 0, 0);
        }
    }

    private void CreatePoints(int count) {
        for (int i = 0; i < pathMarkersCache.Count; i++) {
            if (pathMarkersCache[i] == null) {
                pathMarkersCache[i] = CreateMarker();
                pathMarkersCache[i].SetActive(false);
            }
        }
        for (int i = pathMarkersCache.Count; i < count; i++) {
            var pathMarker = CreateMarker();
            pathMarker.SetActive(false);
            pathMarkersCache.Add(pathMarker);
        }
    }

    private GameObject CreateMarker() {
        return Instantiate<GameObject>(pathMarkerPrefab, transform);
    }

    private void HidePathMarkers() {
        foreach (var pathMarker in pathMarkersCache) {
            if (pathMarker != null) {
                pathMarker.SetActive(false);
            }
        }
    }
}
