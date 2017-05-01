using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

	public SegmentData[] worldData;
	public int gridSize;

    public GameObject pineTreeHolder;
    GameObject pineTreeHolderOriginal;
    public GameObject pineTree1;
    public GameObject pineTree2;

    public GameObject forestTreeHolder;
    GameObject forestTreeHolderOriginal;
    public GameObject forestTree1;
    public GameObject forestTree2;
    public GameObject forestTree3;

    public GameObject palmTreeHolder;
    GameObject palmTreeHolderOriginal;
    public GameObject palmTree1;
    public GameObject palmTree2;

	// Use this for initialization
	public void Populate (SegmentData[] segments) {
		worldData = segments;
        pineTreeHolderOriginal = pineTreeHolder;
        palmTreeHolderOriginal = palmTreeHolder;
        forestTreeHolderOriginal = forestTreeHolder;
        pineTreeHolder = Instantiate(pineTreeHolder);
        forestTreeHolder = Instantiate(forestTreeHolder);
        palmTreeHolder = Instantiate(palmTreeHolder);
		PopulateTrees ();
        pineTreeHolder = pineTreeHolderOriginal;
        palmTreeHolder = palmTreeHolderOriginal;
        forestTreeHolder = forestTreeHolderOriginal;
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
            int forestTreeCountdown = 0;
            int palmTreeCountdown = 0;

            for (int x = 0; x < gridSize; x++) 
			{
				for (int y = 0; y < gridSize; y++) 
				{
					Vector3 position = worldData [i].mesh.vertices [worldData [i].mesh.triangles [index * 6]];
                    float height = position.magnitude;

                    // PINE TREES
                    if (height >= 101.0f &&
                        height <= 101.4f)
                    {
                        if (pineTreeCountdown <= 0)
                        {
                            SpawnPineTree(position);
                            pineTreeCountdown = (int)Random.Range(2.0f, 6.0f);
                        }
                        pineTreeCountdown--;
                    }

                    // FOREST TREES
                    if (height >= 100.82f &&
                        height <= 100.99f)
                    {
                        if (forestTreeCountdown <= 0)
                        {
                            SpawnForestTree(position);
                            forestTreeCountdown = (int)Random.Range(20.0f, 60.0f);
                        }
                        forestTreeCountdown--;
                    }

                    if (height >= 100.4f &&
                       height <= 100.6f)
                    {
                        if (palmTreeCountdown <= 0)
                        {
                            SpawnPalmTree(position);
                            palmTreeCountdown = (int)Random.Range(10.0f, 30.0f);
                        }
                        palmTreeCountdown--;
                    }

                    index++;
				}
			}
		}
	}

    void SpawnPineTree(Vector3 position)
    {
        if(Random.Range(0.0f, 2.0f) >= 1.0f)
        {
            GameObject obj = Instantiate(pineTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = pineTreeHolder.transform;          
        }
        else
        {
            GameObject obj = Instantiate(pineTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = pineTreeHolder.transform;
        }
    }

    void SpawnForestTree(Vector3 position)
    {
        float picker = Random.Range(0.0f, 3.0f);
        if (picker > 1.0f)
        {
            GameObject obj = Instantiate(forestTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else if (picker > 2.0f)
        {
            GameObject obj = Instantiate(forestTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else
        {
            GameObject obj = Instantiate(forestTree3, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
    }

    void SpawnPalmTree(Vector3 position)
    {
        if (Random.Range(0.0f, 2.0f) >= 1.0f)
        {
            GameObject obj = Instantiate(palmTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = palmTreeHolder.transform;
        }
        else
        {
            GameObject obj = Instantiate(palmTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, position.normalized);
            obj.transform.parent = palmTreeHolder.transform;
        }
    }

}
