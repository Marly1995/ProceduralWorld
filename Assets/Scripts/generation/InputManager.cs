using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    public Button random;
    public Button randomColor;

    public IslandGenerator gen;
    public GameObject player;

	// Use this for initialization
	void Start () {
        random.onClick.AddListener(GenerateRandom);
        randomColor.onClick.AddListener(GenerateRandomColors);
        GenerateRandom();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("a")) {
            GenerateRandom();
        }
	}

    void GenerateRandom()
    {
        gen.RandomizeColors();
        gen.randomize = true;
        gen.DrawMapInEditor();
        gen.randomize = false;
        SetPlayerLocation();
    }

    void GenerateRandomColors()
    {
        gen.randomize = false;
        gen.RandomizeColors();
        gen.DrawMapInEditor();
    }

    void SetPlayerLocation()
    {
        player.transform.position = new Vector3(6, 8, 0);
    }
}
