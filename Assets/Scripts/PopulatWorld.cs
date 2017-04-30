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
					Vector3 position = worldData [i].mesh.vertices [worldData [i].mesh.triangles [index * 6]];
					//Debug.Log (position.magnitude);
					if (position.magnitude >= 101.0f &&
					    position.magnitude <= 101.4f) {
						if (countdown <= 0) {
							Instantiate (tree, position, Quaternion.identity);
							countdown = (int)Random.Range (2.0f, 6.0f);
						}
						countdown--;
					}
					index++;
				}
			}
		}
	}
}
