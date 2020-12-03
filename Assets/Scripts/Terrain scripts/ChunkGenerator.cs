using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, colourMap
    }
    public DrawMode drawMode;

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
    public TerrainType[] regions;

    public void GenerateChunk() //generates the chunk
    {
        float[,] noiseMap = NoiseGenerator.NoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset); //gets the noisemap
        Color[] colourMap = new Color[mapWidth * mapHeight];

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>(); //gets MapDisplay object
        if(drawMode == DrawMode.NoiseMap)
        {
           display.DrawNoiseMap(noiseMap); //draws the noisemap 
        }
        else if(drawMode == DrawMode.colourMap)
        {

        }
        
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
        if(noiseScale < 10)
        {
            noiseScale = 10;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public float height;
    public Color colour;
    public string name;
}