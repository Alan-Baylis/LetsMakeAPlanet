using UnityEngine;
using System.Collections.Generic;
using System;

public class Planet : MonoBehaviour {

    // Radius of the planet
    public float radius = 10f;

    // Set to true for a sphere, false for cube.
    public bool Spherized;

    public Material surfaceMaterial;
    public bool autoUpdate;

    // Array of planet surface objects.
    public List<PlanetSurface> planetSurfaces = new List<PlanetSurface>();

    // Generate our planet
    public void Generate() {
        ClearPlanetSurfaces();
        for (int i = 0; i < 6; i++) {
            CreatePlanetSurface((PlanetFace)i);
        }
    }

    // Create, initialize, and render the planet surfaces
    private void CreatePlanetSurface(PlanetFace planetFace) {
        // Calculate the max LOD based on the radius of the planet.
        int iLODMax = (int)Math.Log((float)((2.0f * Math.PI * radius) / 4.0f), 2);
        int cubeSize = iLODMax * 2;
        int halfCube = cubeSize / 2;

        GameObject go = new GameObject("Surface");
        go.transform.parent = this.transform;
        go.transform.position = transform.position;
        PlanetSurface planetSurface = go.AddComponent<PlanetSurface>();
        planetSurface.Initialize(this, planetFace, cubeSize, halfCube);
        planetSurface.GenerateMesh(Spherized);
        planetSurfaces.Add(planetSurface);
    }

    // Clear our array of surfaces
    public void ClearPlanetSurfaces() {
        Component[] childComponents = GetComponentsInChildren<PlanetSurface>();
        if (childComponents.Length > 0) {
            foreach(Component c in childComponents) {
                PlanetSurface s = (PlanetSurface)c;
                if(s != null) {
                    DestroyImmediate(s.gameObject);
                }
            }
        }

        planetSurfaces.Clear();
    }

}
