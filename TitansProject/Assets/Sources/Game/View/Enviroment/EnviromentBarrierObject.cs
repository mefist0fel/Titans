using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnviromentBarrierObject : MonoBehaviour {
    [System.Serializable]
    public sealed class Volume {
        public float Size = 1f;
        public Vector3 Offset = Vector3.zero;
    }

    [SerializeField]
    private Volume[] occupiedVolumes = new Volume[1]; // Set from editor

    public Volume[] OccupatedVolumes {
        get {
            return occupiedVolumes;
        }
    }

    public void RebuildMeshSpherical(float raduis) {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters) {
            ApplyRotations(meshFilter);
            meshFilter.mesh = Spherize(meshFilter.mesh, raduis);
        }
    }

    private void ApplyRotations(MeshFilter meshFilter) {
        var mesh = meshFilter.mesh;
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var localRotation = meshFilter.transform.localRotation;
        var localScale = meshFilter.transform.localScale;
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = localRotation * Vector3.Scale(vertices[i], localScale);
            normals[i] = localRotation * normals[i];
        }
        meshFilter.transform.localRotation = Quaternion.identity;
        meshFilter.transform.localScale = Vector3.one;
        meshFilter.mesh = new Mesh() { vertices = vertices, normals = normals, triangles = mesh.triangles, uv = mesh.uv };
    }

    private Mesh Spherize(Mesh mesh, float raduis) {
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        float perimeter = 2f * Mathf.PI * raduis;
        var distanceToDegreeAspect = 360f / perimeter;
        for (int i = 0; i < vertices.Length; i++) {
            if (vertices[i].y < 0.005f) {
                vertices[i].y = -0.06f;
                normals[i] = Vector3.up;
            }
            vertices[i] = ConvertToPolar(vertices[i], raduis, distanceToDegreeAspect);
            normals[i] = ConvertToPolar(normals[i], raduis, distanceToDegreeAspect);
        }
        return new Mesh() { vertices = vertices, normals = normals, triangles = mesh.triangles, uv = mesh.uv };
    }

    private Vector3 ConvertToPolar(Vector3 position, float raduis, float distanceToDegreeAspect) {
        var polar = position + new Vector3(0, raduis, 0);
        var newPosition = Quaternion.Euler(polar.x * distanceToDegreeAspect, 0, polar.z * distanceToDegreeAspect) * new Vector3(0, polar.y, 0);
        return newPosition - new Vector3(0, raduis, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (occupiedVolumes == null)
            return;
        Gizmos.color = Color.gray;
        foreach (var volume in occupiedVolumes) {
            Gizmos.DrawWireSphere(transform.position + transform.rotation * volume.Offset, volume.Size);
        }
    }
#endif
}
