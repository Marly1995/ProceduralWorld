﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (SeamlessMapGenerator))]
public class SeamlessMapGenEditor : Editor {

    public NoiseFunctions.NoiseType noiseType;
    public bool showNoiseFunctions;
    public bool showTextures;
    private string fileName;
    public NoiseFunctions[] oldNoises;

    public override void OnInspectorGUI()
    {
        
        #region Standard Calls
         
        SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
        
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            { mapGen.GenerateMap(); }
        }
        #endregion

        #region Error Control
        //error control to keep height positive and non zero
        if (mapGen.mapWidth < 1) mapGen.mapWidth = 1;
        if (mapGen.mapHeight < 1) mapGen.mapHeight = 1;
        #endregion

        #region Image Save Functionality

        if (GUILayout.Button("Save Image"))
        {
            fileName = EditorUtility.SaveFilePanel("Save a Copy of Texture", Application.dataPath, "mapimage", "png");
            mapGen.SaveImage(fileName);
        }
#endregion

        #region Noise Functions Foldout
        showNoiseFunctions = EditorGUILayout.Foldout(showNoiseFunctions, "Noise Functions");
        if (showNoiseFunctions)
        {
            
            if (GUILayout.Button("Add New Noise Function"))
            {
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length + 1];
                for (int j = 0; j < mapGen.noiseFunctions.Length; j++)
                {
                    placeholder[j] = mapGen.noiseFunctions[j];
                }
                placeholder[mapGen.noiseFunctions.Length] = new NoiseFunctions();
                mapGen.noiseFunctions = new NoiseFunctions[placeholder.Length];
                for (int j = 0; j < placeholder.Length; j++)
                {
                    mapGen.noiseFunctions[j] = placeholder[j];
                }
                return;
            }

            #region Save / Load Functions
            if (GUILayout.Button("Save This Noise Preset"))
            {
                fileName = EditorUtility.SaveFilePanel("Save a New Preset", Application.dataPath, "Noise Preset", "npr");
                mapGen.SavePresets(mapGen.noiseFunctions, fileName); 
            }
            if (GUILayout.Button("Load Preset From File"))
            {
                fileName = EditorUtility.OpenFilePanel("Load a noise File ", null, "npr");
                mapGen.LoadPresets(fileName);
            }
            
        }

#endregion


        for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
            {
                if (showNoiseFunctions)
                {
                    GetInspectorElements(mapGen.noiseFunctions[i], i, mapGen);
                }

            }
#endregion

        

        if (GUILayout.Button("Generate"))
            {
                mapGen.GenerateMap();
            }
    }
   
    public void GetInspectorElements(NoiseFunctions noiseFunc, int index, SeamlessMapGenerator generator)
    {
        //to autoupdate if this panel has been changed
        EditorGUI.BeginChangeCheck();

        #region Perlin Function UI
        if (noiseFunc.type == NoiseFunctions.NoiseType.Perlin)
        {
            EditorGUILayout.Space();
            string name = "Perlin Noise";
            EditorGUILayout.LabelField(name);
            noiseFunc.type = (NoiseFunctions.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", noiseFunc.type);
            noiseFunc.enabled = EditorGUILayout.ToggleLeft("Enabled", noiseFunc.enabled);
            noiseFunc.frequency = (double)EditorGUILayout.Slider("Frequency",(float)noiseFunc.frequency, 0f, 20f);
            noiseFunc.lacunarity = (double)EditorGUILayout.Slider("Lacunarity", (float)noiseFunc.lacunarity, 2.0000000f, 2.5000000f);
            noiseFunc.persistence = (double)EditorGUILayout.Slider("Persistence", (float)noiseFunc.persistence, 0f, 1f);
            noiseFunc.octaves = EditorGUILayout.IntSlider("Octaves", noiseFunc.octaves, 0, 18);
            noiseFunc.qualityMode = (LibNoise.QualityMode)EditorGUILayout.EnumPopup("Quality Mode", noiseFunc.qualityMode);
            if (GUILayout.Button("Remove"))
            {
                SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    int tempIndex = 0;
                    if (i != index)
                    {
                        placeholder[tempIndex] = mapGen.noiseFunctions[i];
                        tempIndex++;
                    }
                }
                mapGen.noiseFunctions = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    mapGen.noiseFunctions[i] = placeholder[i];
                }
                
            }
        }
        #endregion

        #region Billow Function UI
        else if (noiseFunc.type == NoiseFunctions.NoiseType.Billow)
        {
            EditorGUILayout.Space();
            string name = "Billow Noise";
            EditorGUILayout.LabelField(name);
            noiseFunc.type = (NoiseFunctions.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", noiseFunc.type);
            noiseFunc.enabled = EditorGUILayout.ToggleLeft("Enabled", noiseFunc.enabled);
            noiseFunc.frequency = (double)EditorGUILayout.Slider("Frequency", (float)noiseFunc.frequency, 0f, 20f);
            noiseFunc.lacunarity = (double)EditorGUILayout.Slider("Lacunarity", (float)noiseFunc.lacunarity, 1.5000000f, 3.5000000f);
            noiseFunc.persistence = (double)EditorGUILayout.Slider("Persistence", (float)noiseFunc.persistence, 0f, 1f);
            noiseFunc.octaves = EditorGUILayout.IntSlider("Octaves", noiseFunc.octaves, 0, 18);
            noiseFunc.qualityMode = (LibNoise.QualityMode)EditorGUILayout.EnumPopup("Quality Mode", noiseFunc.qualityMode);
            if (GUILayout.Button("Remove"))
            {
                SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    int tempIndex = 0;
                    if (i != index)
                    {
                        placeholder[tempIndex] = mapGen.noiseFunctions[i];
                        tempIndex++;
                    }
                }
                mapGen.noiseFunctions = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    mapGen.noiseFunctions[i] = placeholder[i];
                }

            }
        }
#endregion

        #region Voronoi UI
        else if (noiseFunc.type == NoiseFunctions.NoiseType.Voronoi)
        {
            EditorGUILayout.Space();
            string name = "Voronoi Noise";
            EditorGUILayout.LabelField(name);
            noiseFunc.type = (NoiseFunctions.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", noiseFunc.type);
            noiseFunc.enabled = EditorGUILayout.ToggleLeft("Enabled", noiseFunc.enabled);
            noiseFunc.frequency = (double)EditorGUILayout.Slider("Frequency", (float)noiseFunc.frequency, 0f, 20f);
            noiseFunc.displacement = (double)EditorGUILayout.Slider("Displacement", (float)noiseFunc.displacement, 0f, 20f);
            noiseFunc.distance = EditorGUILayout.ToggleLeft("Use Distance", noiseFunc.distance);
            if (GUILayout.Button("Remove"))
            {
                SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    int tempIndex = 0;
                    if (i != index)
                    {
                        placeholder[tempIndex] = mapGen.noiseFunctions[i];
                        tempIndex++;
                    }
                }
                mapGen.noiseFunctions = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    mapGen.noiseFunctions[i] = placeholder[i];
                }

            }

        }
#endregion

        #region Rigged Multifractal UI
        else if (noiseFunc.type == NoiseFunctions.NoiseType.RiggedMultifractal)
        {

            EditorGUILayout.Space();
            string name = "Rigged Multifractal";
            EditorGUILayout.LabelField(name);
            noiseFunc.type = (NoiseFunctions.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", noiseFunc.type);
            noiseFunc.enabled = EditorGUILayout.ToggleLeft("Enabled", noiseFunc.enabled);
            noiseFunc.frequency = (double)EditorGUILayout.Slider("Frequency", (float)noiseFunc.frequency, 0f, 20f);
            noiseFunc.lacunarity = (double)EditorGUILayout.Slider("Lacunarity", (float)noiseFunc.lacunarity, 1.5000000f, 3.5000000f);
            noiseFunc.octaves = EditorGUILayout.IntSlider("Octaves", noiseFunc.octaves, 0, 18);
            noiseFunc.qualityMode = (LibNoise.QualityMode)EditorGUILayout.EnumPopup("Quality Mode", noiseFunc.qualityMode);
            if (GUILayout.Button("Remove"))
            {
                SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    int tempIndex = 0;
                    if (i != index)
                    {
                        placeholder[tempIndex] = mapGen.noiseFunctions[i];
                        tempIndex++;
                    }
                }
                mapGen.noiseFunctions = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    mapGen.noiseFunctions[i] = placeholder[i];
                }
                return;

            }
        }
        #endregion

        #region None UI
        else if (noiseFunc.type == NoiseFunctions.NoiseType.None)
        {

            EditorGUILayout.Space();
            string name = "None";
            EditorGUILayout.LabelField(name);
            noiseFunc.type = (NoiseFunctions.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", noiseFunc.type);
            if (GUILayout.Button("Remove"))
            {
                SeamlessMapGenerator mapGen = (SeamlessMapGenerator)target;
                NoiseFunctions[] placeholder = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    int tempIndex = 0;
                    if (i != index)
                    {
                        placeholder[tempIndex] = mapGen.noiseFunctions[i];
                        tempIndex++;
                    }
                }
                mapGen.noiseFunctions = new NoiseFunctions[mapGen.noiseFunctions.Length - 1];
                for (int i = 0; i < mapGen.noiseFunctions.Length; i++)
                {
                    mapGen.noiseFunctions[i] = placeholder[i];
                }

            }
            noiseFunc.enabled = false;
        }

        #endregion

        //to autoupdate if this inspector element has changed
        if (generator.autoUpdate&&EditorGUI.EndChangeCheck())
        {
            generator.GenerateMap();
        }

        

    }
  



}
