using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Planet))]
public class PlanetEditor : Editor {

    public override void OnInspectorGUI() {

        Planet planet = (Planet)target;

        if (DrawDefaultInspector()) {
            if (planet.autoUpdate) {
                planet.Generate();
            }
        }

        if(GUILayout.Button("Generate Planet")) {
            planet.Generate();
        }

    }

}
