using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public sealed class SphereMesh : MonoBehaviour {
	[SerializeField]
    private float radius = 1;
	[SerializeField]
    private int details = 18;

    [SerializeField]
    [HideInInspector]
    private MeshFilter meshFilter;

    static Quaternion[] sides = new Quaternion[] {
		Quaternion.Euler(0,0,0),
		Quaternion.Euler(180,0,0),
		Quaternion.Euler(0, 90,0),
		Quaternion.Euler(0,-90,0),
		Quaternion.Euler( 90,0,0),
		Quaternion.Euler(-90,0,0)
	};

    private void Awake () {
        if (meshFilter == null) {
            meshFilter = Utils.GetOrCreateComponent<MeshFilter>(gameObject);
        }
	}
		
	private void Start () {
        GenerateMesh();
    }

	[ContextMenu("regenerate planet")]
    private void GenerateMesh() {
        Awake();
        meshFilter.mesh = CreateMesh(radius, details);
    }

    private Mesh CreateMesh(float radius, int details = 18) {
        int h = (details + 1) * (details + 1);
        CombineInstance[] meshes = new CombineInstance[6];
        for (int z = 0; z < 6; z++) {
            Vector3[] normals = new Vector3[h];
            Vector2[] textures = new Vector2[h];
            Vector3[] vertexMeshComponent = new Vector3[h];
            Color[] colorMeshComponent = new Color[h];
            int[] triangles = new int[details * details * 2 * 6];
            for (int i = 0; i <= details; i++) {
                for (int j = 0; j <= details; j++) {
                    normals[i + j * (details + 1)] = (sides[z] * new Vector3(1f - 2f * i / (details), 1f - 2f * j / (details), 1f)).normalized;
                }
            }
            for (int i = 0; i <= details; i++) {
                for (int j = 0; j <= details; j++) {
                    vertexMeshComponent[i + j * (details + 1)] = normals[i + j * (details + 1)] * radius;
                    colorMeshComponent[i + j * (details + 1)] = new Color(1, 1, 1);
                    textures[i + j * (details + 1)] = new Vector2((float)i / (details), (float)j / (details));
                }
            }
            for (int i = 0; i < details; i++) {
                for (int j = 0; j < details; j++) {
                    triangles[(i + j * details) * 6 + 0] = i + j * (details + 1);
                    triangles[(i + j * details) * 6 + 1] = i + 1 + (j) * (details + 1);
                    triangles[(i + j * details) * 6 + 2] = i + (j + 1) * (details + 1);
                    triangles[(i + j * details) * 6 + 3] = i + (j + 1) * (details + 1);
                    triangles[(i + j * details) * 6 + 4] = i + 1 + (j) * (details + 1);
                    triangles[(i + j * details) * 6 + 5] = i + 1 + (j + 1) * (details + 1);
                }
            }
            meshes[z] = new CombineInstance();
            meshes[z].transform = Matrix4x4.identity;
            meshes[z].mesh = new Mesh { vertices = vertexMeshComponent, uv = textures, colors = colorMeshComponent, triangles = triangles, normals = normals };
        }

        Mesh combined = new Mesh();
        combined.CombineMeshes(meshes, true);
        return combined;
    }

#if UNITY_EDITOR
    private void Update() {
        TryUpdate();
    }

    private void OnDrawGizmos() {
        TryUpdate();
    }

    private float prevRadius = 1f;
    private int prevDetails = 18;
    private void TryUpdate() {
        if (prevRadius != radius) {
            prevRadius = radius;
            GenerateMesh();
        }
        if (prevDetails != details) {
            prevDetails = details;
            GenerateMesh();
        }
    }
#endif
}
