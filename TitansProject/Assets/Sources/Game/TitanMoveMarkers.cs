using System.Collections.Generic;
using UnityEngine;

public class TitanMoveMarkers : MonoBehaviour {
    [SerializeField]
    private GameObject selectMarker; // Set from editor
    [SerializeField]
    private GameObject pathMarkerPrefab; // Set from editor

    private List<GameObject> pathMarkersCache = new List<GameObject>();

    public void SelectTitan(TitanView titan) {
        if (selectMarker != null) {
            selectMarker.transform.parent = titan.transform;
            selectMarker.transform.localRotation = Quaternion.identity;
            selectMarker.transform.localPosition = Vector3.zero;
        }
    }
}
