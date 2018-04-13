using System.Collections.Generic;
using UnityEngine;

public sealed class PlanetAssetSpawner : MonoBehaviour {
    [SerializeField]
    private float radius = 10f;
    [SerializeField]
    private List<AssetPrefab> plasedAssets = new List<AssetPrefab>();

    private List<GameObject> spawnedAssets = new List<GameObject>();

    [System.Serializable]
    public sealed class AssetPrefab {
        public int Count = 10;
        public GameObject Asset = null;
    }

    private void Start () {
        SpawnAssets();
	}

    [ContextMenu("Spawn assets")]
    private void SpawnAssets() {
        ClearAssets();
        foreach (var prefab in plasedAssets) {
            if (prefab == null || prefab.Asset == null) {
                continue;
            }
            for (int i = 0; i < prefab.Count; i++) {
                var asset = Instantiate(prefab.Asset, transform);
                var randomNormal = GetRandomNormal();
                asset.transform.localPosition = randomNormal * radius;
                asset.transform.localRotation = Quaternion.LookRotation(randomNormal) * Quaternion.Euler(90, 0, 0);
                spawnedAssets.Add(asset);
            }
        }
    }

    private Vector3 GetRandomNormal() {
        Vector3 point;
        do {
            point = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f));
        } while (point.sqrMagnitude > 1f);
        return point.normalized;
    }

    private void ClearAssets() {
        foreach (var asset in spawnedAssets) {
            Destroy(asset);
        }
        spawnedAssets.Clear();
    }
}
