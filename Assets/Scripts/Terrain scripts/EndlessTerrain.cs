using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float playerMoveThresholdForChunkUpdate = 25f;
    const float squarePlayerMoveThresholdForChunkUpdate = playerMoveThresholdForChunkUpdate * playerMoveThresholdForChunkUpdate;
    
    public static float maxViewDist;
    public LODInfo[] detailLevels;

    public Transform player;
    public static Vector2 playerPos;
    Vector2 oldPlayerPos;

    static ChunkGenerator chunkGenerator;
    public Material mapMaterial;
    int chunkSize;
    int visibleChunks;

    Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    static List<Chunk> oldChunks = new List<Chunk>();

    void Start()
    {
        chunkGenerator = FindObjectOfType<ChunkGenerator>();

        maxViewDist = detailLevels[detailLevels.Length - 1].distThreshold;
        chunkSize = ChunkGenerator.chunkSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDist / chunkSize);

        UpdateVisibleChunks();
    }

    void Update()
    {
        playerPos = new Vector2(player.position.x, player.position.z);

        if((oldPlayerPos - playerPos).sqrMagnitude > squarePlayerMoveThresholdForChunkUpdate)
        {
            oldPlayerPos = playerPos;
            UpdateVisibleChunks();
        }
        
    }

    void UpdateVisibleChunks()
    {
        for(int i = 0; i < oldChunks.Count; i++)
        {
            oldChunks[i].SetVisible(false);
        }
        oldChunks.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / chunkSize) + 1; //probably an issue here

        for(int yOffset = -visibleChunks; yOffset <= visibleChunks; yOffset++)
        {
            for(int xOffset = -visibleChunks; xOffset <= visibleChunks; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); //something about the y value here isnt being calculated correctly

                if(chunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    chunkDictionary[viewedChunkCoord].UpdateChunk();
                }
                else
                {
                    chunkDictionary.Add(viewedChunkCoord, new Chunk(viewedChunkCoord, chunkSize, transform, mapMaterial, detailLevels));
                }
            }
        }
    }

    public class Chunk
    {
        Vector2 position;
        GameObject meshObject;
        Bounds bounds;

        MapData mapData;
        bool mapDataRecieved;
        int prevLODIndex = -1;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        MeshCollider meshCollider;
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;
        

        public Chunk(Vector2 coord, int size, Transform parent, Material material, LODInfo[] detailLevels)
        {
            this.detailLevels = detailLevels;
            position = coord * size;

            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            meshObject.layer = 9;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one;
            
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].LOD, UpdateChunk);
                if(detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }

            chunkGenerator.RequestMapData(OnMapDataRecieved, position);
        }

        void OnMapDataRecieved(MapData mapData)
        {
            this.mapData = mapData;
            mapDataRecieved = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, ChunkGenerator.chunkSize, ChunkGenerator.chunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateChunk();
        }

        public void UpdateChunk()
        {
            if(mapDataRecieved)
            {
                float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
                bool visible = playerDistFromEdge <= maxViewDist;

                if(visible)
                {
                    int lodIndex = 0;
                    for(int i = 0; i <detailLevels.Length - 1; i++)
                    {
                        if(playerDistFromEdge > detailLevels[i].distThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    if(lodIndex != prevLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if(lodMesh.hasMesh)
                        {
                            prevLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if(!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }  
                    }

                    if(lodIndex == 0)
                    {
                        if(collisionLODMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                        }
                        else if(!collisionLODMesh.hasRequestedMesh)
                        {   
                            collisionLODMesh.RequestMesh(mapData);
                        }
                    }
                    oldChunks.Add(this);
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }   

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int LOD;
        System.Action updateCallback;

        public LODMesh(int LOD, System.Action updateCallback)
        {
            this.LOD = LOD;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            chunkGenerator.RequestMeshData(mapData, OnMeshDataRecieved, LOD);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int LOD;
        public float distThreshold;
        public bool useForCollider;
    }
}
