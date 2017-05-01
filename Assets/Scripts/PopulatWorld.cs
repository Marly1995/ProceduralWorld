using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

	public SegmentData[] worldData;
	public int gridSize;

	public GameObject pineTree1;
    public GameObject pineTree2;

    public GameObject forestTree1;
    public GameObject forestTree2;
    public GameObject forestTree3;

    public GameObject palmTree1;
    public GameObject palmTree2;

	// Use this for initialization
	public void Populate (SegmentData[] segments) {
		worldData = segments;
		PopulateTrees ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void PopulateTrees()
	{
		for (int i = 0; i < worldData.Length; i++) 
		{
			int index = 0;
			int pineTreeCountdown = 0;
			for (int x = 0; x < gridSize; x++) 
			{
				for (int y = 0; y < gridSize; y++) 
				{
					Vector3 position = worldData [i].mesh.vertices [worldData [i].mesh.triangles [index * 6]];
                    float height = position.magnitude;


                    if (height >= 101.0f &&
					    height <= 101.4f) {
                        SpawnPineTree(position, pineTreeCountdown);
					}
					index++;
				}
			}
		}
	}

    void SpawnPineTree(Vector3 position, int pineTreeCountdown)
    {
        if (pineTreeCountdown <= 0)
        {
            if(Random.Range(0.0f, 2.0f) >= 1.0f)
            {
                GameObject obj = Instantiate(pineTree1, position, Quaternion.identity);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
                pineTreeCountdown = (int)Random.Range(2.0f, 6.0f);
            }
            else
            {
                GameObject obj = Instantiate(pineTree2, position, Quaternion.identity);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
                pineTreeCountdown = (int)Random.Range(2.0f, 6.0f);
            }
        }
        pineTreeCountdown--;
    }
}
