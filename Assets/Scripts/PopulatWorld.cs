using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

	public SegmentData[] worldData;
	public int gridSize;

	public GameObject tree;
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
			int countdown = 0;
			for (int x = 0; x < gridSize; x++) 
			{
				for (int y = 0; y < gridSize; y++) 
				{
					if (worldData [i].MapData.heightMap [x, y] >= 0.55f &&
					   worldData [i].MapData.heightMap [x, y] <= 0.6f) 
					{
						countdown--;
						if (countdown <= 0)
						{
							Vector3 position = worldData [i].mesh.vertices [worldData [i].mesh.triangles [index * 6]];
							Debug.Log (position.magnitude);
							Instantiate (tree, position, Quaternion.identity);
							countdown = (int)Random.Range (2.0f, 6.0f);
						}
					}
					index++;
				}
			}
		}
	}
}
