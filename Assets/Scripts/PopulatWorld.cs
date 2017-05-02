using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

    public SegmentData[] worldData;
    ObjectLocations[] objectLocations;
    public GameObject villageLocation;
    public List<SpawnLocation> villageLocations;
    public int gridSize;

    // mountain assets
    public GameObject pineTreeHolder;
    GameObject pineTreeHolderOriginal;
    public GameObject pineTree1;
    public GameObject pineTree2;

    // plains assets
    public GameObject forestTreeHolder;
    GameObject forestTreeHolderOriginal;
    public GameObject forestTree1;
    public GameObject forestTree2;
    public GameObject forestTree3;

    // beach assets
    public GameObject palmTreeHolder;
    GameObject palmTreeHolderOriginal;
    public GameObject palmTree1;
    public GameObject palmTree2;

    // village assets
    public GameObject beachHut1;
    public GameObject pier;
    public GameObject walkway;

    // ice village assets
    public GameObject smallIglu;
    public GameObject fireIglu;

	// Use this for initialization
	public void Populate (SegmentData[] segments) {
		worldData = segments;
        objectLocations = new ObjectLocations[segments.Length];
        villageLocations = new List<SpawnLocation>();

        pineTreeHolderOriginal = pineTreeHolder;
        palmTreeHolderOriginal = palmTreeHolder;
        forestTreeHolderOriginal = forestTreeHolder;

        pineTreeHolder = Instantiate(pineTreeHolder);
        forestTreeHolder = Instantiate(forestTreeHolder);
        palmTreeHolder = Instantiate(palmTreeHolder);

		PopulateTrees();

        pineTreeHolder = pineTreeHolderOriginal;
        palmTreeHolder = palmTreeHolderOriginal;
        forestTreeHolder = forestTreeHolderOriginal;

        LocateVillageLocations();

        PopulateVillages();
    }

    void LocateVillageLocations()
    {
        for (int i = 0; i < worldData.Length; i++)
        {
            for (int x = 0; x < gridSize; x+=gridSize/4)
            {
                for (int y = 0; y < gridSize; y+=gridSize/4)
                {
                    if (objectLocations[i].height[x,y] >= 100.82f &&
                        objectLocations[i].height[x,y] <= 100.99f)
                    {
                        int terraincheck = 0;
                        for (int o = x-gridSize/4; o < x+gridSize/4; o++)
                        {
                            for (int p = y-gridSize/4; p < y+gridSize/4; p++)
                            {
                                if (o < gridSize && o >= 0 && p < gridSize && p >= 0)
                                {
                                    if (objectLocations[i].height[o, p] >= 100.82f &&
                                        objectLocations[i].height[o, p] <= 100.99f)
                                    {
                                        terraincheck++;
                                    }
                                }
                            }
                        }

                        if (terraincheck >= (gridSize / 8) * (gridSize / 8))
                        {
                            bool spawn = true;
                            for (int k = 0; k < villageLocations.Count; k++)
                            {
                                if (Vector3.Distance(objectLocations[i].position[x, y], villageLocations[k].obj.transform.position) <= 100.0f)
                                {
                                    spawn = false;
                                    break;
                                }
                            }
                            if (spawn == true)
                            {
                                GameObject village = Instantiate(villageLocation, objectLocations[i].position[x, y], Quaternion.identity);
                                villageLocations.Add(new SpawnLocation(x, y, i, village));
                            }
                        }
                    }
                }
            }
       }
    }

    void PopulateVillages()
    {
        for (int i = 0; i < villageLocations.Count; i++)
        {
            GameObject obj = Instantiate(beachHut1, villageLocations[i].obj.transform.position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -villageLocations[i].obj.transform.position);
            objectLocations[villageLocations[i].chunk].space[villageLocations[i].x, villageLocations[i].y] = true;
            int nextHut = (int)Random.Range(10.0f, 20.0f);
            villageLocations[i].items.Add(obj);
            int searchRange = gridSize / 2;
            for (int x = villageLocations[i].x-gridSize; x < gridSize; x++)
            {
                for (int y = villageLocations[i].y- gridSize; y < gridSize; y++)
                {                  
                    if (x < gridSize && x >= 0 && y < gridSize && y >= 0)
                    {
                        nextHut--;
                        if (objectLocations[villageLocations[i].chunk].height[x, y] >= 100.82f &&
                            objectLocations[villageLocations[i].chunk].height[x, y] <= 100.99f)
                        {
                            if (nextHut <= 0)
                            {
                                if (!Physics.Raycast(Vector3.zero, objectLocations[villageLocations[i].chunk].position[x, y], Mathf.Infinity))
                                {
                                    GameObject hut = Instantiate(beachHut1, objectLocations[villageLocations[i].chunk].position[x, y], Quaternion.identity);
                                    hut.transform.rotation = Quaternion.FromToRotation(Vector3.down, -objectLocations[villageLocations[i].chunk].position[x, y]);
                                    nextHut = (int)Random.Range(10.0f, 20.0f);
                                }
                            }                   
                        }
                    }
                }
            }
        }
    }

	void PopulateTrees()
	{
		for (int i = 0; i < worldData.Length; i++) 
		{
			int index = 0;
			int pineTreeCountdown = 0;
            int forestTreeCountdown = 0;
            int palmTreeCountdown = 0;
            objectLocations[i] = new ObjectLocations(gridSize);

            for (int x = 0; x < gridSize; x++) 
			{
				for (int y = 0; y < gridSize; y++) 
				{
					Vector3 position = worldData [i].mesh.vertices [worldData [i].mesh.triangles [index * 6]];
                    float height = position.magnitude;

                    objectLocations[i].position[x, y] = position;
                    objectLocations[i].height[x, y] = height;

                    // PINE TREES
                    if (height >= 101.0f &&
                        height <= 101.4f)
                    {
                        if (pineTreeCountdown <= 0)
                        {
                            SpawnPineTree(position);
                            pineTreeCountdown = (int)Random.Range(2.0f, 6.0f);
                            objectLocations[i].space[x, y] = true;
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
                            objectLocations[i].space[x, y] = true;
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
                            objectLocations[i].space[x, y] = true;
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
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = pineTreeHolder.transform;          
        }
        else
        {
            GameObject obj = Instantiate(pineTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = pineTreeHolder.transform;
        }
    }

    void SpawnForestTree(Vector3 position)
    {
        float picker = Random.Range(0.0f, 3.0f);
        if (picker > 1.0f)
        {
            GameObject obj = Instantiate(forestTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else if (picker > 2.0f)
        {
            GameObject obj = Instantiate(forestTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else
        {
            GameObject obj = Instantiate(forestTree3, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = forestTreeHolder.transform;
        }
    }

    void SpawnPalmTree(Vector3 position)
    {
        if (Random.Range(0.0f, 2.0f) >= 1.0f)
        {
            GameObject obj = Instantiate(palmTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = palmTreeHolder.transform;
        }
        else
        {
            GameObject obj = Instantiate(palmTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.parent = palmTreeHolder.transform;
        }
    }

    public struct ObjectLocations
    {
        public bool[,] space;
        public Vector3[,] position;
        public float[,] height;

        public ObjectLocations(int size)
        {
            space = new bool[size,size];
            position = new Vector3[size, size];
            height = new float[size, size];
        }
    }

    public struct SpawnLocation
    {
        public GameObject obj;
        public List<GameObject> items;
        public int chunk;
        public int x;
        public int y;

        public SpawnLocation(int x, int y, int chunk, GameObject obj)
        {
            this.obj = obj;
            this.x = x;
            this.y = y;
            this.chunk = chunk;
            items = new List<GameObject>();
        }
    }
}
