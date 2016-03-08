using UnityEngine;
using System.Collections;

// Require both a MeshFilter and MeshRenderer component
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class PlanetSurface : MonoBehaviour {

    // Parent planet this surface belongs too
    public Planet planet;
    // The face of this surface
    public PlanetFace planetFace;
    // The material of the surface
    public Material surfaceMaterial;

    // The mesh of the surface
    public Mesh mesh;

    // The center of this face
    public Vector3 center;
    // The local X axis for this face
    public Vector3 localX;
    // The local Y axis for this face
    public Vector3 localY;

    // The number of vertices in each column/row of the grid.
    const int meshGridSize = 240;
    const int meshGridSizePlus = meshGridSize + 1;

    // Initialize the surface and set its center and local X and Y axis based on the desired radius of the planet.
    public void Initialize(Planet planet, PlanetFace planetFace, int cubeSize, int cubeHalf) {
        this.planet = planet;
        this.planetFace = planetFace;
        if (planet.surfaceMaterial != null) {
            this.surfaceMaterial = planet.surfaceMaterial;
        } else {
            this.surfaceMaterial = new Material(Shader.Find("Standard"));
        }

        switch (planetFace) {
            case PlanetFace.FRONT:
                base.name = base.name + " (Front)";
                center = new Vector3(0, 0, -cubeHalf);
                localX = new Vector3(cubeSize, 0, 0);
                localY = new Vector3(0, cubeSize, 0);
                break;
            case PlanetFace.BACK:
                base.name = base.name + " (Back)";
                center = new Vector3(0, 0, cubeHalf);
                localX = new Vector3(-cubeSize, 0, 0);
                localY = new Vector3(0, cubeSize, 0);
                break;
            case PlanetFace.LEFT:
                base.name = base.name + " (Left)";
                center = new Vector3(-cubeHalf, 0, 0);
                localX = new Vector3(0, 0, -cubeSize);
                localY = new Vector3(0, cubeSize, 0);
                break;
            case PlanetFace.RIGHT:
                base.name = base.name + " (Right)";
                center = new Vector3(cubeHalf, 0, 0);
                localX = new Vector3(0, 0, cubeSize);
                localY = new Vector3(0, cubeSize, 0);
                break;
            case PlanetFace.TOP:
                base.name = base.name + " (Top)";
                center = new Vector3(0, cubeHalf, 0);
                localX = new Vector3(-cubeSize, 0, 0);
                localY = new Vector3(0, 0, -cubeSize);
                break;
            case PlanetFace.BOTTOM:
                base.name = base.name + " (Bottom)";
                center = new Vector3(0, -cubeHalf, 0);
                localX = new Vector3(cubeSize, 0, 0);
                localY = new Vector3(0, 0, -cubeSize);
                break;
        }

        // If the mesh isn't already set then get the needed components and set a defaul mesh and material.
        if(mesh == null) {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh = new Mesh();

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = surfaceMaterial;
        }
    }

    // Generate the mesh(either as a cube or sphere)
    public void GenerateMesh(bool spherized) {

        MeshData meshData = new MeshData(meshGridSizePlus, meshGridSizePlus);

        int vertexIndex = 0;

        // Generate vertex positions and uv cooridinates and store them in our MeshData class.

        for(int u = 0; u <= meshGridSize; u++) {
            for(int v = 0; v <= meshGridSize; v++) {
                Vector3 position = center + (localX / meshGridSize) * (v - meshGridSize / 2) + (localY / meshGridSize) * (u - meshGridSize / 2);

                meshData.vertices[vertexIndex] = (spherized) ? position.normalized * planet.radius : position;
                meshData.uvs[vertexIndex] = new Vector2(v / (float)meshGridSize, u / (float)meshGridSize);

                // Add triangles to our MeshData class
                if(u < meshGridSize && v < meshGridSize) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + meshGridSizePlus, vertexIndex + 1);
                    meshData.AddTriangle(vertexIndex + meshGridSizePlus, vertexIndex + meshGridSizePlus + 1, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        // Calculate Normals

        for(int i = 0; i < meshData.triangles.Length; i += 3) {
            int index1 = meshData.triangles[i];
            int index2 = meshData.triangles[i + 1];
            int index3 = meshData.triangles[i + 2];

            Vector3 v1 = meshData.vertices[index1];
            Vector3 v2 = meshData.vertices[index2];
            Vector3 v3 = meshData.vertices[index3];

            Vector3 n1 = v2 - v1;
            Vector3 n2 = v3 - v1;

            Vector3 normal = Vector3.Cross(n1, n2).normalized;

            meshData.normals[index1] += normal;
            meshData.normals[index2] += normal;
            meshData.normals[index3] += normal;
        }

        for(int i = 0; i < meshData.normals.Length; i++) {
            meshData.normals[i].Normalize();
        }

        // Set the mesh using data stored in our MeshData class from above.

        if (mesh != null) {
            mesh.name = base.name;
            mesh.vertices = meshData.vertices;
            mesh.normals = meshData.normals;
            mesh.uv = meshData.uvs;
            mesh.triangles = meshData.triangles;
            mesh.RecalculateBounds();
            mesh.Optimize();
        }

        if(this != null) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
        }
    }

	void Start () {
	
	}
	
	void Update () {
	
	}

}
