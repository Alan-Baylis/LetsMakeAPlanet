using UnityEngine;
using System.Collections.Generic;

// Require both a MeshFilter and MeshRenderer component
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class PlanetSurface : MonoBehaviour {

    // Parent planet this surface belongs too
    public Planet planet;

    public PlanetSurface parent;
    // The face of this surface
    public PlanetFace planetFace;
    // The material of the surface
    public Material surfaceMaterial;

    // The mesh of the surface
    public Mesh mesh;

    public int lodLevel;

    public bool isLeaf;

    public int nodeSize;

    // The center of this face
    public Vector3 center;
    // The local X axis for this face
    public Vector3 localX;
    // The local Y axis for this face
    public Vector3 localY;

    public List<PlanetSurface> planetSubSurfaces = new List<PlanetSurface>();

    // The number of vertices in each column/row of the grid.
    const int meshGridSize = 2;
    const int meshGridSizePlus = meshGridSize + 1;

    enum NodeType { BOTTOMLEFT, TOPLEFT, TOPRIGHT, BOTTOMRIGHT }

    // Initialize the surface and set its center and local X and Y axis based on the desired radius of the planet.
    public void InitializeRoot(Planet planet, PlanetFace planetFace, int lodLevel, int nodeSize) {
        this.planet = planet;
        this.planetFace = planetFace;
        this.lodLevel = lodLevel;
        if (planet.surfaceMaterial != null) {
            this.surfaceMaterial = planet.surfaceMaterial;
        } else {
            this.surfaceMaterial = new Material(Shader.Find("Standard"));
        }

        int nodeHalf = nodeSize / 2;

        switch (planetFace) {
            case PlanetFace.FRONT:
                base.name = base.name + " (Front)";
                center = new Vector3(0, 0, -nodeHalf);
                localX = new Vector3(nodeSize, 0, 0);
                localY = new Vector3(0, nodeSize, 0);
                break;
            case PlanetFace.BACK:
                base.name = base.name + " (Back)";
                center = new Vector3(0, 0, nodeHalf);
                localX = new Vector3(-nodeSize, 0, 0);
                localY = new Vector3(0, nodeSize, 0);
                break;
            case PlanetFace.LEFT:
                base.name = base.name + " (Left)";
                center = new Vector3(-nodeHalf, 0, 0);
                localX = new Vector3(0, 0, -nodeSize);
                localY = new Vector3(0, nodeSize, 0);
                break;
            case PlanetFace.RIGHT:
                base.name = base.name + " (Right)";
                center = new Vector3(nodeHalf, 0, 0);
                localX = new Vector3(0, 0, nodeSize);
                localY = new Vector3(0, nodeSize, 0);
                break;
            case PlanetFace.TOP:
                base.name = base.name + " (Top)";
                center = new Vector3(0, nodeHalf, 0);
                localX = new Vector3(-nodeSize, 0, 0);
                localY = new Vector3(0, 0, -nodeSize);
                break;
            case PlanetFace.BOTTOM:
                base.name = base.name + " (Bottom)";
                center = new Vector3(0, -nodeHalf, 0);
                localX = new Vector3(nodeSize, 0, 0);
                localY = new Vector3(0, 0, -nodeSize);
                break;
        }

        // If the mesh isn't already set then get the needed components and set a defaul mesh and material.
        if(mesh == null) {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh = new Mesh();

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = surfaceMaterial;
        }

        if(lodLevel == 0) {
            isLeaf = true;
        } else {
            for(int i = 0; i < 4; i++) {
                GameObject go = new GameObject("Sub Surface");
                go.transform.parent = this.transform;
                go.transform.position = transform.position;
                PlanetSurface planetSurface = go.AddComponent<PlanetSurface>();
                planetSurface.InitializeChild(this, (NodeType)i, lodLevel - 1, nodeSize);
                planetSubSurfaces.Add(planetSurface);
            }
        }
    }

    private void InitializeChild(PlanetSurface parent, NodeType nodeType, int lodLevel, int nodeSize) {
        this.planet = parent.planet;
        this.parent = parent;
        this.planetFace = parent.planetFace;
        this.lodLevel = lodLevel;
        if (planet.surfaceMaterial != null) {
            this.surfaceMaterial = parent.surfaceMaterial;
        } else {
            this.surfaceMaterial = new Material(Shader.Find("Standard"));
        }

        this.nodeSize = nodeSize;

        switch (nodeType) {
            case NodeType.BOTTOMLEFT:
                base.name = base.name + " (Bottom Left)";
                localX = new Vector3(parent.localX.x / 2f, parent.localX.y / 2f, parent.localX.z / 2f);
                localY = new Vector3(parent.localY.x / 2f, parent.localY.y / 2f, parent.localY.z / 2f);
                center = parent.center - (localX / 2f) - (localY / 2f);
                break;
            case NodeType.TOPLEFT:
                base.name = base.name + " (Top Left)";
                localX = new Vector3(parent.localX.x / 2f, parent.localX.y / 2f, parent.localX.z / 2f);
                localY = new Vector3(parent.localY.x / 2f, parent.localY.y / 2f, parent.localY.z / 2f);
                center = parent.center - (localX / 2f) + (localY / 2f);
                break;
            case NodeType.TOPRIGHT:
                base.name = base.name + " (Top Right)";
                localX = new Vector3(parent.localX.x / 2f, parent.localX.y / 2f, parent.localX.z / 2f);
                localY = new Vector3(parent.localY.x / 2f, parent.localY.y / 2f, parent.localY.z / 2f);
                center = parent.center + (localX / 2f) + (localY / 2f);
                break;
            case NodeType.BOTTOMRIGHT:
                base.name = base.name + " (Bottom Right)";
                localX = new Vector3(parent.localX.x / 2f, parent.localX.y / 2f, parent.localX.z / 2f);
                localY = new Vector3(parent.localY.x / 2f, parent.localY.y / 2f, parent.localY.z / 2f);
                center = parent.center + (localX / 2f) - (localY / 2f);
                break;
        }

        // If the mesh isn't already set then get the needed components and set a defaul mesh and material.
        if (mesh == null) {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh = new Mesh();

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = surfaceMaterial;
        }

        if(lodLevel == 0) {
            isLeaf = true;
            GenerateMesh(planet.Spherized);
        } else {
            for (int i = 0; i < 4; i++) {
                GameObject go = new GameObject("Sub Surface");
                go.transform.parent = this.transform;
                go.transform.position = transform.position;
                PlanetSurface planetSurface = go.AddComponent<PlanetSurface>();
                planetSurface.InitializeChild(this, (NodeType)i, lodLevel - 1, nodeSize);
                planetSubSurfaces.Add(planetSurface);
            }
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
                meshData.uvs[vertexIndex] = new Vector2((v / (float)meshGridSize), (u / (float)meshGridSize));

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
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
        }

        if(this != null) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
        }
    }

    // Clear our array of surfaces
    public void ClearPlanetSubSurfaces() {
        Component[] childComponents = GetComponentsInChildren<PlanetSurface>();
        
        if (childComponents.Length > 0) {
            foreach (Component c in childComponents) {
                PlanetSurface s = (PlanetSurface)c;
                if (s != null) {
                    DestroyImmediate(s.gameObject);
                }
            }
        }

        planetSubSurfaces.Clear();
    }

    void Start () {
	
	}
	
	void Update () {
	
	}

}
