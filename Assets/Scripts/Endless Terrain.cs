using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public static MeshGeneration meshGenerator;
    public static float viewDistance = 500;
    public static Transform player;
    public Vector2 playerPos = new Vector2(player.position.x, player.position.z);
    static int chunkSize = meshGenerator.chunkSize - 1;
    int visibleChunks = Mathf.RoundToInt(viewDistance/chunkSize);
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    void Update() 
    {
        playerPos = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(player.position.x / chunkSize);
        int currentChunkCoordZ = Mathf.RoundToInt(player.position.z / chunkSize);
        for(int xOffset = -visibleChunks; xOffset <= visibleChunks; xOffset++)
        {
            for(int zOffset = -visibleChunks; zOffset <= visibleChunks; zOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordZ + zOffset);
                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary(viewedChunkCoord).UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk());
                }
            }
        }
    }
}

public class TerrainChunk
{
    public EndlessTerrain EndlessTerrain = new EndlessTerrain();
    Vector2 position;
    GameObject meshObject;
    Bounds bounds;
    
    public TerrainChunk(Vector2 coord, int size)
    {
        position = coord * size;
        bounds = new Bounds(position, Vector2.one * size);
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);
        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.transform.position = positionV3;
        meshObject.transform.localScale = Vector3.one * size / 10f;
        SetVisible(false);
    }

    void UpdateTerrainChunk() 
    {
        float distanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(position));
        bool visible = distanceFromNearestEdge <= EndlessTerrain.viewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }
}