using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatWorld : MonoBehaviour {

    public SegmentData[] worldData;
    ObjectLocations[] objectLocations;
    public GameObject tikiLocation;
    public GameObject igluLocation;
    public List<SpawnLocation> tikiLocations;
    public List<SpawnLocation> igluLocations;
    public int gridSize;

    // mountain assets
    public GameObject pineTreeHolder;
    GameObject pineTreeHolderOriginal;
    public GameObject pineTree;
    public GameObject pineTreeSnow;
    public GameObject deadTree;
    public GameObject deadTreeSnow;

    // plains assets
    public GameObject forestTreeHolder;
    GameObject forestTreeHolderOriginal;
    public GameObject forestTree1;
    public GameObject forestTree2;
    public GameObject berryTree;
    public GameObject bush;
    public List<SpawnLocation> forestLocations;

    // beach assets
    public GameObject palmTreeHolder;
    GameObject palmTreeHolderOriginal;
    public GameObject palmTree1;
    public GameObject palmTree2;

    // village assets
    public GameObject beachHutSmall;
    public GameObject beachhutLarge;
    public GameObject tikiStatue;
    public GameObject fishRack1;
    public GameObject fishRack2;
    public GameObject fishRack3;
    public GameObject pier;
    public GameObject walkway;

    // ice village assets
    public GameObject smallIglu;
    public GameObject fireIglu;
    public GameObject fishRackSnow;
    public GameObject smallFire;
    public GameObject largeFire;

    // Use this for initialization
    public void Populate (SegmentData[] segments) {
		worldData = segments;
        objectLocations = new ObjectLocations[segments.Length];
        tikiLocations = new List<SpawnLocation>();
        igluLocations = new List<SpawnLocation>();
        forestLocations = new List<SpawnLocation>();

        pineTreeHolderOriginal = pineTreeHolder;
        palmTreeHolderOriginal = palmTreeHolder;
        forestTreeHolderOriginal = forestTreeHolder;

        pineTreeHolder = Instantiate(pineTreeHolder);
        forestTreeHolder = Instantiate(forestTreeHolder);
        palmTreeHolder = Instantiate(palmTreeHolder);

        PopulateWorld();

        LocateVillageLocations();

        PopulateTikiVillages();
        PopulateIgluVillages();

        PopulateTrees();

        pineTreeHolder = pineTreeHolderOriginal;
        palmTreeHolder = palmTreeHolderOriginal;
        forestTreeHolder = forestTreeHolderOriginal;
    }

    void LocateVillageLocations()
    {
        for (int i = 0; i < worldData.Length; i++)
        {
            for (int x = 0; x < gridSize; x += gridSize / 4)
            {
                for (int y = 0; y < gridSize; y += gridSize / 4)
                {
                    if (objectLocations[i].height[x, y] >= 100.82f &&
                        objectLocations[i].height[x, y] <= 100.99f)
                    {
                        int terraincheck = 0;
                        for (int o = x - gridSize / 4; o < x + gridSize / 4; o++)
                        {
                            for (int p = y - gridSize / 4; p < y + gridSize / 4; p++)
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
                            bool tikispawn = true;
                            bool igluspawn = true;
                            for (int k = 0; k < tikiLocations.Count; k++)
                            {
                                if (Vector3.Distance(objectLocations[i].position[x, y], tikiLocations[k].obj.transform.position) <= Random.Range(80.0f, 100.0f))
                                {
                                    tikispawn = false;
                                    break;
                                }
                            }
                            for (int k = 0; k < igluLocations.Count; k++)
                            {
                                if (Vector3.Distance(objectLocations[i].position[x, y], igluLocations[k].obj.transform.position) <= Random.Range(40.0f, 60.0f))
                                {
                                    igluspawn = false;
                                    break;
                                }
                            }
                            if (objectLocations[i].position[x, y].y >= 85.0f || objectLocations[i].position[x, y].y <= -85.0f)
                            {
                                if (igluspawn == true && x > 10 && x < 40 && y > 10 && y < 40)
                                {
                                    GameObject igluVillage = Instantiate(igluLocation, objectLocations[i].position[x, y], Quaternion.identity);
                                    igluLocations.Add(new SpawnLocation(x, y, i, igluVillage));
                                }
                            }
                            else if (objectLocations[i].position[x, y].y <= 70.0f && objectLocations[i].position[x, y].y >= -70.0f)
                            {
                                if (tikispawn == true)
                                {
                                    GameObject tikiVillage = Instantiate(tikiLocation, objectLocations[i].position[x, y], Quaternion.identity);
                                    tikiLocations.Add(new SpawnLocation(x, y, i, tikiVillage));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void PopulateTikiVillages()
    {
        for (int i = 0; i < tikiLocations.Count; i++)
        {
            List<GameObject> huts = new List<GameObject>();
            huts.Add(Instantiate(beachhutLarge, tikiLocations[i].obj.transform.position * 1.005f, Quaternion.identity));
            huts[huts.Count - 1].transform.SetParent(tikiLocations[i].obj.transform);
            int nextHut = (int)Random.Range(20.0f, 40.0f);
            int nextDock = (int)Random.Range(2.0f, 5.0f);
            tikiLocations[i].items.Add(huts[huts.Count - 1]);
            int searchRange = gridSize / 2;
            for (int x = tikiLocations[i].x - gridSize; x < gridSize; x++)
            {
                for (int y = tikiLocations[i].y - gridSize; y < gridSize; y++)
                {
                    if (x < gridSize && x >= 0 && y < gridSize && y >= 0)
                    {
                        nextHut--;
                        if (objectLocations[tikiLocations[i].chunk].height[x, y] >= 100.82f &&
                            objectLocations[tikiLocations[i].chunk].height[x, y] <= 100.99f)
                        {
                            if (nextHut <= 0)
                            {
                                if (!Physics.Raycast(Vector3.zero, objectLocations[tikiLocations[i].chunk].position[x, y], Mathf.Infinity))
                                {
                                    huts.Add(Instantiate(beachHutSmall, objectLocations[tikiLocations[i].chunk].position[x, y] * 1.005f, Quaternion.identity));
                                    huts[huts.Count - 1].transform.SetParent(tikiLocations[i].obj.transform);
                                    nextHut = (int)Random.Range(20.0f, 40.0f);
                                }
                            }
                        }
                        if (objectLocations[tikiLocations[i].chunk].height[x, y] >= 100.021f &&
                            objectLocations[tikiLocations[i].chunk].height[x, y] <= 100.023f) {
                            if (nextDock <= 0) {
                                GameObject obj = Instantiate(pier, objectLocations[tikiLocations[i].chunk].position[x, y] * 1.005f, Quaternion.identity);
                                obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -obj.transform.position);
                                obj.transform.Rotate(0.0f, Random.Range(-40, 40), 0.0f, Space.Self);
                                obj.transform.SetParent(tikiLocations[i].obj.transform);
                                nextDock = (int)Random.Range(2.0f, 5.0f);
                            }
                            nextDock--;
                        }
                    }
                }
            }
            bool[] ignores = new bool[huts.Count];
            for (int j = 0; j < huts.Count; j++)
            {
                for (int k = 0; k < huts.Count; k++)
                {
                    float dist = Vector3.Distance(huts[j].transform.position, huts[k].transform.position);                   
                    if (dist <= 10.0f && 
                        k != j && !ignores[j] && !ignores[k])
                    {
                        if (j > 0) {
                            ignores[j] = true;
                        }
                        ignores[k] = true;
                        if (j > 0) 
                        {
                            huts[j].transform.LookAt(huts[k].transform.position, huts[j].transform.position);
                            for (float p = 0; p < dist - 2.0f; p += 1.5f) 
                            {
                                if (p >= 6) { break; }
                                Vector3 pos = Vector3.Lerp(huts[j].transform.position, huts[k].transform.position, p / ((dist + 1.0f) / 1.5f));
                                GameObject way = Instantiate(walkway, pos * 0.998f, huts[j].transform.rotation);
                                way.transform.SetParent(huts[j].transform);
                            }
                            huts[k].transform.LookAt(huts[j].transform.position, huts[k].transform.position);
                        } 
                        else 
                        {
                            //huts[j].transform.LookAt(huts[k].transform.position, huts[j].transform.position);
                            huts[k].transform.LookAt(huts[j].transform.position, huts[k].transform.position);
                            for (float p = 0; p < dist - 2.0f; p += 1.5f) 
                            {
                                if (p >= 6) { break; }
                                Vector3 pos = Vector3.Lerp(huts[k].transform.position, huts[j].transform.position, p / ((dist + 1.0f) / 1.5f));
                                GameObject way = Instantiate(walkway, pos * 0.998f, huts[k].transform.rotation);
                                way.transform.SetParent(huts[k].transform);
                            }                        
                        }    
                    }                    
                }
            }
            for (int j = 0; j < ignores.Length; j++) {
                if (!ignores[j]) {
                    int loc = Random.Range(0, 3);
                    huts[j].transform.rotation = Quaternion.FromToRotation(Vector3.down, -huts[j].transform.position);
                    Vector3 extraPos = Vector3.zero;
                    switch (loc) {
                        case 0:
                            extraPos = huts[j].transform.GetChild(2).transform.position;
                            break;
                        case 1:
                            extraPos = huts[j].transform.GetChild(3).transform.position;
                            break;
                        case 2:
                            extraPos = huts[j].transform.GetChild(4).transform.position;
                            break;
                        case 3:
                            extraPos = huts[j].transform.GetChild(5).transform.position;
                            break;
                    }
                    GameObject extra = Instantiate(tikiStatue, extraPos * 0.995f, Quaternion.identity);
                    extra.transform.rotation = Quaternion.FromToRotation(Vector3.down, -extra.transform.position);
                    extra.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
                    extra.transform.SetParent(huts[j].transform);
                }
                if (ignores[j]) {
                    int loc = Random.Range(0, 3);                    
                    Vector3 extraPos = Vector3.zero;
                    switch (loc) {
                        case 0:
                            extraPos = huts[j].transform.GetChild(2).transform.position;
                            break;
                        case 1:
                            extraPos = huts[j].transform.GetChild(3).transform.position;
                            break;
                        case 2:
                            extraPos = huts[j].transform.GetChild(4).transform.position;
                            break;
                        case 3:
                            extraPos = huts[j].transform.GetChild(5).transform.position;
                            break;
                    }
                    GameObject extra = Instantiate(fishRack1, extraPos * 0.995f, Quaternion.identity);
                    extra.transform.rotation = Quaternion.FromToRotation(Vector3.down, -extra.transform.position);
                    extra.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
                    extra.transform.SetParent(huts[j].transform);
                }
            }
        }
    }

    void PopulateIgluVillages()
    {
        for (int i = 0; i < igluLocations.Count; i++)
        {
            List<GameObject> iglus = new List<GameObject>();
            iglus.Add(Instantiate(fireIglu, igluLocations[i].obj.transform.position * 0.999f, Quaternion.identity));
            iglus[iglus.Count - 1].transform.rotation = Quaternion.FromToRotation(Vector3.down, -igluLocations[i].obj.transform.position);
            iglus[iglus.Count - 1].transform.SetParent(igluLocations[i].obj.transform);
            igluLocations[i].items.Add(iglus[iglus.Count - 1]);
            for (int x = igluLocations[i].x - gridSize; x < gridSize; x++)
            {
                for (int y = igluLocations[i].y - gridSize; y < gridSize; y++)
                {                    
                    if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
                    {
                        if (objectLocations[igluLocations[i].chunk].height[x, y] >= 100.82f &&
                            objectLocations[igluLocations[i].chunk].height[x, y] <= 100.99f)
                        {
                            if (!Physics.Raycast(Vector3.zero, objectLocations[igluLocations[i].chunk].position[x, y], Mathf.Infinity))
                            {
                                if (Vector3.Distance(objectLocations[igluLocations[i].chunk].position[x, y] * 0.999f, iglus[0].transform.position) <= 10.0f)
                                {
                                    iglus.Add(Instantiate(smallIglu, objectLocations[igluLocations[i].chunk].position[x, y] * 0.999f, Quaternion.identity));
                                    iglus[iglus.Count - 1].transform.SetParent(igluLocations[i].obj.transform);                                   
                                    iglus[iglus.Count - 1].transform.LookAt(iglus[0].transform.position, iglus[iglus.Count - 1].transform.position);                                   
                                }
                            }                            
                        }
                    }
                }                
            }           
        }
    }

    void PopulateWorld()
    {
        for (int i = 0; i < worldData.Length; i++)
        {
            int index = 0;
            int forestTreeCountdown = 0;
            objectLocations[i] = new ObjectLocations(gridSize);
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 position = worldData[i].mesh.vertices[worldData[i].mesh.triangles[index * 6]];
                    float height = position.magnitude;

                    objectLocations[i].position[x, y] = position;
                    objectLocations[i].height[x, y] = height;

                    if (height >= 100.82f &&
                        height <= 100.99f &&
                        position.y <= 70.0f &&
                        position.y >= -70.0f)
                    {
                        if (forestTreeCountdown <= 0)
                        {
                            bool spawn = true;
                            for (int k = 0; k < forestLocations.Count; k++)
                            {
                                if(Vector3.Distance(objectLocations[i].position[x, y], forestLocations[k].obj.transform.position) <= 30.0f)
                                {
                                    spawn = false;
                                }
                            }
                            if (spawn)
                            {
                                GameObject obj = Instantiate(forestTreeHolderOriginal, objectLocations[i].position[x, y], Quaternion.identity);
                                obj.transform.SetParent(forestTreeHolder.transform);
                                forestLocations.Add(new SpawnLocation(x, y, i, obj));
                                forestTreeCountdown = (int)Random.Range(20.0f, 50.0f);
                                objectLocations[i].space[x, y] = true;
                            }
                        }
                        forestTreeCountdown--;
                    }

                    index++;
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

            for (int x = 0; x < gridSize; x++) 
			{
				for (int y = 0; y < gridSize; y++) 
				{
                    // PINE TREES
                    if (objectLocations[i].height[x, y] >= 101.2f &&
                        objectLocations[i].height[x, y] <= 101.8f)
                    {
                        if (pineTreeCountdown <= 0 &&
                            !Physics.Raycast(Vector3.zero, objectLocations[i].position[x, y], Mathf.Infinity))
                        {
                            SpawnPineTree(objectLocations[i].position[x, y]);
                            pineTreeCountdown = (int)Random.Range(2.0f, 6.0f);
                            objectLocations[i].space[x, y] = true;
                        }
                        pineTreeCountdown--;
                    }
                    if(objectLocations[i].height[x, y] >= 100.75f &&
                        objectLocations[i].height[x, y] <= 100.99f &&
                       objectLocations[i].position[x, y].y <= 70.0f &&
                       objectLocations[i].position[x, y].y >= -70.0f)
                    {
                        for (int k = 0; k < forestLocations.Count; k++)
                        {
                            if (Vector3.Distance(objectLocations[i].position[x, y], forestLocations[k].obj.transform.position) <= 10.0f)
                            {
                                if (forestTreeCountdown <= 0 &&
                                    !Physics.Raycast(Vector3.zero, objectLocations[i].position[x, y], Mathf.Infinity))
                                {
                                    SpawnForestTree(objectLocations[i].position[x, y]);
                                    forestTreeCountdown = (int)Random.Range(1.0f, 3.0f);
                                    objectLocations[i].space[x, y] = true;
                                }
                                forestTreeCountdown--;
                            }
                        }                       
                    }

                    // PALM TREES
                    if (objectLocations[i].height[x, y] >= 100.2f &&
                       objectLocations[i].height[x, y] <= 100.6f &&
                       objectLocations[i].position[x, y].y <= 70.0f &&
                       objectLocations[i].position[x, y].y >= -70.0f)
                    {
                        if (palmTreeCountdown <= 0 &&
                            !Physics.Raycast(Vector3.zero, objectLocations[i].position[x, y], Mathf.Infinity))
                        {
                            SpawnPalmTree(objectLocations[i].position[x, y]);
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
        if (position.y <= 70.0f && position.y >= -70.0f)
        {
            if (Random.Range(0.0f, 2.0f) >= 1.0f) {
                GameObject obj = Instantiate(pineTree, position, Quaternion.identity);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
                obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
                obj.transform.parent = pineTreeHolder.transform;               
            } else {
                GameObject obj = Instantiate(deadTree, position, Quaternion.identity);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
                obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
                obj.transform.parent = pineTreeHolder.transform;
            }
        }
        else if (Random.Range(0.0f, 2.0f) >= 1.0f) 
        {
            GameObject obj = Instantiate(pineTreeSnow, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = pineTreeHolder.transform;
        } 
        else 
        {
            GameObject obj = Instantiate(deadTreeSnow, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = pineTreeHolder.transform;
        }
    }

    void SpawnForestTree(Vector3 position)
    {  
        float picker = Random.Range(0.0f, 10.0f);
        if (picker <= 2.0f)
        {
            GameObject obj = Instantiate(forestTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else if (picker <= 4.0f)
        {
            GameObject obj = Instantiate(forestTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = forestTreeHolder.transform;
        }
        else if (picker <= 6.0f)
        {
            GameObject obj = Instantiate(berryTree, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = forestTreeHolder.transform;
        } 
        else 
        { 
            GameObject obj = Instantiate(bush, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = forestTreeHolder.transform;            
        }
    }

    void SpawnPalmTree(Vector3 position)
    {
        if (Random.Range(0.0f, 2.0f) >= 1.0f)
        {
            GameObject obj = Instantiate(palmTree1, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
            obj.transform.parent = palmTreeHolder.transform;
        }
        else
        {
            GameObject obj = Instantiate(palmTree2, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.down, -position.normalized);
            obj.transform.Rotate(0.0f, Random.Range(0, 360), 0.0f, Space.Self);
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
