using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    public Button random;
    public Button randomColor;

    public IslandGenerator gen;

	// Use this for initialization
	void Start () {
        random.onClick.AddListener(GenerateRandom);
        randomColor.onClick.AddListener(GenerateRandomColors);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateRandom()
    {
        gen.RandomizeColors();
        gen.randomize = true;
        gen.DrawMapInEditor();
        gen.randomize = false;
    }

    void GenerateRandomColors()
    {
        gen.randomize = false;
        gen.RandomizeColors();
        gen.DrawMapInEditor();
    }
}
