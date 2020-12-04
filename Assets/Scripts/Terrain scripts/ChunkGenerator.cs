using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public enum DrawMode //enum for different drawing types
    {
        NoiseMap, ColourMap, Mesh
    }
    public DrawMode drawMode;

    public int mapWidth; //chunk width
    public int mapHeight; //chunk height
    public float noiseScale; //scale of noise
    public int octaves; //determines No of octaves
    [Range(0,1)]
    public float persistance; //determines persistance in range 0-1
    public float lacunarity; //determines lacunarity
    public int seed; //determines seed
    public Vector2 offset; //determines offset
    public bool autoUpdate; //editor variable
    public TerrainType[] regions; //variable for the colourmap
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public void GenerateChunk() //generates the chunk
    {
        float[,] noiseMap = NoiseGenerator.NoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset); //gets the noisemap
        Color[] colourMap = new Color[mapWidth * mapHeight]; //gets colourmap

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y]; //hold height of that point
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour; //determines what region and therefore what colour it is
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>(); //gets MapDisplay object
        
        if(drawMode == DrawMode.NoiseMap)
        {
           display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap)); //draws the noisemap 
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight)); //draws the colourmap 
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGeneration.GenerateMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        
    }

    void OnValidate()
    {
        //clamps variables to certain values

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
        if(noiseScale < 5)
        {
            noiseScale = 5;
        }
    }
}

[System.Serializable]//allows this to be shown in editor
public struct TerrainType
{
    //all these variables are set in editor
    public float height;
    public Color colour;
    public string name;
}