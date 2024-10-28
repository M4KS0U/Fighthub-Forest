using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LowPolyTerrain))]
public class LowPolyTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LowPolyTerrain script = (LowPolyTerrain) target;
        
        if(DrawDefaultInspector())
        {
            script.Initiate();
        }

        if (GUILayout.Button("New Seed"))
        {
            script.seed = Random.Range(0, 100);
            script.Initiate();
        }
    }
}
