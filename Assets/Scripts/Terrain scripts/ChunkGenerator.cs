using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public int mapWidth; //chunk width
    public int mapHeight; //chunk height
    public float noiseScale; //scale of noise
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate; //editor variable

    public void GenerateChunk() //generates the chunk
    {
        float[,] noiseMap = NoiseGenerator.NoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset); //gets the noisemap

        MapDisplay display = FindObjectOfType<MapDisplay>(); //gets MapDisplay object
        display.DrawNoiseMap(noiseMap); //draws the noisemap
    }

    void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 1)
        {
            octaves = 1;
        }
    }
}
