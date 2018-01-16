using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    public Button random;
    public Button randomColor;

    public IslandGenerator[] generators;
    public GameObject player;
    public GameObject enemy;

	// Use this for initialization
	void Start () {
        GenerateRandom(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space))
        {
            int index = Random.Range(0, generators.Length - 1);
            GenerateRandom(index);
        }
	}

    void GenerateRandom(int i)
    {
        generators[i].RandomizeColors();
        generators[i].RandomizeHeights();
        generators[i].randomize = true;
        generators[i].DrawMapInEditor();
        generators[i].randomize = false;
        SetPlayerLocation();
        PopulateEnemys();
    }

    void GenerateRandomColors(int i)
    {
        generators[i].randomize = false;
        generators[i].RandomizeColors();
        generators[i].DrawMapInEditor();
    }

    void SetPlayerLocation()
    {
        //player.transform.position = new Vector3(6, 50, 0);
    }

    void PopulateEnemys()
    {

    }
}
