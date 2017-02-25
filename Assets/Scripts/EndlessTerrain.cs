using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

    public lodInfo[] detailLevels;
    public static float maxViewDist;

    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static IslandGenerator islandGen;

    int chunkSize;
    int chunksVisible;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleChunks = new List<TerrainChunk>();

    void Start()
    {
        islandGen = FindObjectOfType<IslandGenerator>();

        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistanceThreshhold;

        chunkSize = IslandGenerator.mapChunkSize - 1;
        chunksVisible = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < visibleChunks.Count; i++)
        {
            visibleChunks[i].SetVisible(false);
        }
        visibleChunks.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisible; yOffset < chunksVisible; yOffset++)
        {
            for (int xOffset = -chunksVisible; xOffset < chunksVisible; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);

                if (terrainChunkDictionary.ContainsKey (viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        visibleChunks.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        lodInfo[] detailLevels;
        LodMesh[] meshLods;

        MapData mapData;
        bool mapDataRecieved;
        int prevLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, lodInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();           
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            meshLods = new LodMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                meshLods[i] = new LodMesh(detailLevels[i].lod);
            }

            islandGen.RequestMapData(OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData)
        {
            this.mapData = mapData;
            mapDataRecieved = true;
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataRecieved)
            {
                float viewerDistanceFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistanceFromEdge <= maxViewDist;

                if (visible)
                {
                    int index = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistanceFromEdge > detailLevels[i].visibleDistanceThreshhold)
                        {
                            index = i + 1;
                        }
                        else { break; }
                    }

                    if (index != prevLODIndex)
                    {
                        LodMesh lodMesh = meshLods[index];
                        if (lodMesh.hasMesh)
                        {
                            prevLODIndex = index;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

    }

    class LodMesh
    {
        public Mesh mesh;
        public bool hasRequestMesh;
        public bool hasMesh;
        int lod;

        public LodMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestMesh = true;
            islandGen.RequestMeshData(mapData, lod, OnMeshDataRecieved);
        }
    }

    [System.Serializable]
    public struct lodInfo
    {
        public int lod;
        public float visibleDistanceThreshhold;
    }
}
