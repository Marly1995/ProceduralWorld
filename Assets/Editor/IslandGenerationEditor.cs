using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof  (IslandGenerator))]
public class IslandGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IslandGenerator islandGen = (IslandGenerator)target;

        if(DrawDefaultInspector())
        {
            if(islandGen.autoUpdate)
            {
                islandGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button ("Generate"))
        {
            islandGen.DrawMapInEditor();
        }
    }
}
