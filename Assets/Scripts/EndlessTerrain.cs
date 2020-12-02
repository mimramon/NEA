using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float viewDistance = 200;
    public Transform player;
    public static Vector2 playerPos;
    int chunkSize;
    int visibleChunks;
    Dictionary<Vector2, TerrainChunk> chunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> previouslyVisibleChunks = new List<TerrainChunk>();

    void Start()
    {
        chunkSize = MeshGeneration.chunkSize;
        visibleChunks = Mathf.RoundToInt(viewDistance / chunkSize);
    }

    void Update()
    {
        playerPos = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for(int i = 0; i < previouslyVisibleChunks.Count; i++)
        {
            previouslyVisibleChunks[i].SetVisible(false);
        }
        previouslyVisibleChunks.Clear();
        {
            
        }
        int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / chunkSize);

        for(int yOffest = -visibleChunks; yOffest <= visibleChunks; yOffest+=5)
        {
            for(int xOffest = -visibleChunks; xOffest <= visibleChunks; xOffest+=5)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffest, currentChunkCoordY + yOffest);
                if(chunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    chunkDictionary[viewedChunkCoord].UpdateChunk();
                    if(chunkDictionary[viewedChunkCoord].isVisible())
                    {
                        previouslyVisibleChunks.Add(chunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    chunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }
    public class TerrainChunk
    {
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshGeneration meshGenerator;
        MeshCollider meshCollider;
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);
            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshGenerator = meshObject.AddComponent<MeshGeneration>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size/10f;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateChunk()
        {
            float playerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
            bool visible = playerDistanceFromNearestEdge <= viewDistance;
            meshFilter.mesh = meshGenerator.mesh;
            meshRenderer.material = new Material(Shader.Find("Diffuse")); 
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public  bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }
}


