using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDist = 250;
    public Transform player;
    public static Vector2 playerPos;
    int chunkSize;
    int visibleChunks;

    Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    List<Chunk> oldChunks = new List<Chunk>();

    void Start()
    {
        chunkSize = ChunkGenerator.chunkSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        playerPos = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for(int i = 0; i < oldChunks.Count; i++)
        {
            oldChunks[i].SetVisible(false);
        }
        oldChunks.Clear();
        int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / chunkSize);

        for(int yOffset = -visibleChunks; yOffset <= visibleChunks; yOffset++)
        {
            for(int xOffset = -visibleChunks; xOffset <= visibleChunks; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(chunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    chunkDictionary[viewedChunkCoord].UpdateChunk();
                    if(chunkDictionary[viewedChunkCoord].isVisible())
                    {
                        oldChunks.Add(chunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    chunkDictionary.Add(viewedChunkCoord, new Chunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class Chunk
    {
        Vector2 position;
        GameObject meshObject;
        Bounds bounds;
        public Chunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateChunk()
        {
            float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
            bool visible = playerDistFromEdge <=maxViewDist;
            SetVisible(visible);
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
}
