using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetSurface))]
public class PlanetSurfaceEditor : Editor {

    public override void OnInspectorGUI() {

        PlanetSurface planetSurface = (PlanetSurface)target;

        if (DrawDefaultInspector()) {
            
        }

        if (GUILayout.Button("Generate Planet")) {
            planetSurface.GenerateMesh(false);
        }

    }

}
