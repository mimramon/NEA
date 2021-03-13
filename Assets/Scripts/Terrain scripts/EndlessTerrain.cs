using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float playerMoveThresholdForChunkUpdate = 25f;
    const float squarePlayerMoveThresholdForChunkUpdate = playerMoveThresholdForChunkUpdate * playerMoveThresholdForChunkUpdate;
    const float colliderDistThreshold = 4;

    public int colliderLODIndex;
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
        playerPos = new Vector2(player.position.x, player.position.z) / chunkGenerator.terrainData.uniformScale;

        if(playerPos != oldPlayerPos)
        {
            foreach(Chunk chunk in oldChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if((oldPlayerPos - playerPos).sqrMagnitude > squarePlayerMoveThresholdForChunkUpdate)
        {
            oldPlayerPos = playerPos;
            UpdateVisibleChunks();
        }
        
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> beenUpdatedCoords = new HashSet<Vector2>();
        for(int i = oldChunks.Count - 1; i >= 0; i--)
        {
            beenUpdatedCoords.Add(oldChunks[i].coord);
            oldChunks[i].UpdateChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / chunkSize) + 1; //probably an issue here

        for(int yOffset = -visibleChunks; yOffset <= visibleChunks; yOffset++)
        {
            for(int xOffset = -visibleChunks; xOffset <= visibleChunks; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); //something about the y value here isnt being calculated correctly
                if(!beenUpdatedCoords.Contains(viewedChunkCoord))
                {
                    if(chunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        chunkDictionary[viewedChunkCoord].UpdateChunk();
                    }
                    else
                    {
                        chunkDictionary.Add(viewedChunkCoord, new Chunk(viewedChunkCoord, chunkSize, transform, mapMaterial, detailLevels, colliderLODIndex));
                    }
                }
                
            }
        }
    }

    public class Chunk
    {
        public Vector2 coord;
        
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
        int colliderLODIndex;
        bool hasSetCollider;
        

        public Chunk(Vector2 coord, int size, Transform parent, Material material, LODInfo[] detailLevels, int colliderLODIndex)
        {
            this.coord = coord;
            this.detailLevels = detailLevels;
            this.colliderLODIndex = colliderLODIndex;
            position = coord * size;

            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            meshObject.layer = 9;

            meshObject.transform.position = positionV3 * chunkGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * chunkGenerator.terrainData.uniformScale;
            
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].LOD);
                lodMeshes[i].updateCallback += UpdateChunk;
                if(i == colliderLODIndex)
                {
                    lodMeshes[i].updateCallback += UpdateCollisionMesh;
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
                bool wasVisible = isVisible();

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
                    
                }
                if(wasVisible != visible)
                {
                    if(visible)
                    {
                        oldChunks.Add(this);
                    }
                    else
                    {
                        oldChunks.Remove(this);
                    }
                    SetVisible(visible);
                }
            }
        }

        public void UpdateCollisionMesh()
        {
            if(hasSetCollider){return;}
            float sqrDistPlayer2Edge = bounds.SqrDistance(playerPos);

            if(sqrDistPlayer2Edge < detailLevels[colliderLODIndex].sqrVsbleDistThreshold)
            {
                if(!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(mapData);
                }
            }

            if(sqrDistPlayer2Edge < colliderDistThreshold * colliderDistThreshold)
            {
                if(lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
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
        public event System.Action updateCallback;

        public LODMesh(int LOD)
        {
            this.LOD = LOD;
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

        public float sqrVsbleDistThreshold
        {
            get
            {
                return distThreshold * distThreshold;
            }
        }
    }
}
