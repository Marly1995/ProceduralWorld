using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

    public SegmentData[] worldData;
    ObjectLocations[] objectLocations;
    public int gridSize;

    // rocks
    public GameObject rockHolder;
    public GameObject rock1;
    public GameObject rock2;
    public GameObject rock3;
    public GameObject rock4;

    // cliff walls
    public GameObject wall1;
    public GameObject wall2;

    // Use this for initialization
    public void Populate (SegmentData[] segments) {
		worldData = segments;
        objectLocations = new ObjectLocations[segments.Length];

        PopulateWorld();

        PopulateRocks();
    }

    void PopulateWorld()
    {
        for (int i = 0; i < worldData.Length; i++)
        {
            int index = 0;
            objectLocations[i] = new ObjectLocations(gridSize);
            for (int x = 0; x < gridSize; x+=6)
            {
                for (int y = 0; y < gridSize; y+=6)
                {
                    Vector3 position = worldData[i].mesh.vertices[worldData[i].mesh.triangles[index * 36]];
                    float height = position.magnitude;

                    objectLocations[i].position[x, y] = position;
                    objectLocations[i].height[x, y] = height;
                    index+=6;
                }
            }
        }
    }

	void PopulateRocks()
	{
        for (int i = 0; i < worldData.Length; i++)
        {
			int rockCount = 0;

            for (int x = 0; x < gridSize; x+=6) 
			{
				for (int y = 0; y < gridSize; y+=6) 
				{
                    // PINE TREES
                    if (objectLocations[i].height[x, y] >= 10.001f &&
                        objectLocations[i].height[x, y] <= 11.8f)
                    {
                        if (rockCount <= 0)
                        {
                            SpawnRock(objectLocations[i].position[x, y]);
                            rockCount = Random.Range(36, 72);
                            objectLocations[i].space[x, y] = true;
                        }
                        rockCount--;
                    }
				}
			}
		}
	}

    void SpawnRock(Vector3 position)
    {
        int index = Random.Range(0, 3);
        switch(index)
        {
            case 0:
                PlaceObject(rock1, position, rockHolder);
                break;
            case 1:
                PlaceObject(rock2, position, rockHolder);
                break;
            case 2:
                PlaceObject(rock3, position, rockHolder);
                break;
            case 3:
                PlaceObject(rock4, position, rockHolder);
                break;
        }
    }

    void PlaceObject(GameObject temp, Vector3 position, GameObject parent)
    {
        GameObject obj = Instantiate(temp, position, Quaternion.identity);
        obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
        obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
        obj.transform.parent = parent.transform;
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
