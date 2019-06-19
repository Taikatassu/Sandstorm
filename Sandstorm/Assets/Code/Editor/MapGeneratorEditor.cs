using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator targetScript = (MapGenerator)target;
        //if (GUILayout.Button("ResetSeeds"))
        //{
        //    targetScript.ResetSeeds();
        //    targetScript.Generate();
        //}
        
        if (GUILayout.Button("Generate"))
        {
            targetScript.ResetSeeds();
            targetScript.Generate();
        }

        DrawDefaultInspector();
    }
}
