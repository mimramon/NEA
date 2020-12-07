using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public enum DrawMode //enum for different drawing types
    {
        NoiseMap, ColourMap, Mesh
    }
    public NoiseGenerator.NormalizeMode normalizeMode;
    public DrawMode drawMode;

    public const int chunkSize = 241; //chunk dimensions
    [Range(0, 6)]
    public int editorLevelOfDetail;
    public float noiseScale; //scale of noise
    public int octaves; //determines No of octaves
    [Range(0,1)]
    public float persistance; //determines persistance in range 0-1
    public float lacunarity; //determines lacunarity
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public int seed; //determines seed
    public Vector2 offset; //determines offset
    public bool autoUpdate; //editor variable
    public TerrainType[] regions; //variable for the colourmap
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    
    public void DrawMap()
    {
        MapData mapData = GenerateChunkData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>(); //gets MapDisplay object
        
        if(drawMode == DrawMode.NoiseMap)
        {
           display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap)); //draws the noisemap 
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, chunkSize, chunkSize)); //draws the colourmap 
        }
        else if(drawMode == DrawMode.Mesh)
        {
            lock(meshHeightCurve)
            {
                display.DrawMesh(MeshGeneration.GenerateMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorLevelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, chunkSize, chunkSize));
            }
        }
    }

    public void RequestMapData(Action<MapData> callback, Vector2 centre)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback, centre);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback, Vector2 centre)
    {
        MapData mapData = GenerateChunkData(centre);
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback, int LOD)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback, LOD);
        };
        new Thread(threadStart).Start();
    }
    void MeshDataThread(MapData mapData, Action<MeshData> callback, int LOD)
    {  
        MeshData meshData = MeshGeneration.GenerateMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, LOD);
        
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        } 
    }






    void Update()
    {
        lock(mapDataThreadInfoQueue)
        {
            if(mapDataThreadInfoQueue.Count > 0)
            {
                for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        lock(meshDataThreadInfoQueue)
        {
            if(meshDataThreadInfoQueue.Count > 0)
            {
                for(int i = 0; i < meshDataThreadInfoQueue.Count; i++)
                {
                        MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                        threadInfo.callback(threadInfo.parameter);
                }
            }
        }
    }

    MapData GenerateChunkData(Vector2 centre) //generates the chunk
    {
        float[,] noiseMap = NoiseGenerator.NoiseMap(chunkSize, chunkSize, noiseScale, octaves, persistance, lacunarity, seed, centre + offset, normalizeMode); //gets the noisemap
        Color[] colourMap = new Color[chunkSize * chunkSize]; //gets colourmap

        for(int y = 0; y < chunkSize; y++)
        {
            for(int x = 0; x < chunkSize; x++)
            {
                float currentHeight = noiseMap[x, y]; //hold height of that point
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight >= regions[i].height)
                    {
                        colourMap[y * chunkSize + x] = regions[i].colour; //determines what region and therefore what colour it is
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }

    void OnValidate()
    {
        //clamps variables to certain values
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

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;
    
    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}