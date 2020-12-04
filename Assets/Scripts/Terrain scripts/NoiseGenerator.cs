using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] NoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset) //Generates the noise map/height map of the terrain, takes width, height, scale of noise, octaves (number of "waves" or maps), persistance (the scale of the effect of an octave on the whole map), lacunarity (the "frequency" or noisiness of an octave ) and the seed used to produce a unique map as arguments
    {
        float[,] noiseMap = new float[mapWidth, mapHeight]; //stores the noise map

        System.Random psuedoRNG = new System.Random(seed); //used generate a map that can be reloaded
        Vector2[] octaveOffset = new Vector2[octaves]; //samples octaves from different coordinates

        for(int i = 0; i < octaves; i++)
        {
            float xOffset = psuedoRNG.Next(-100000,100000) + offset.x; //offset allows you to scroll through noise
            float yOffset = psuedoRNG.Next(-100000,100000) + offset.y; //offset allows you to scroll through noise
            octaveOffset[i] = new Vector2(xOffset, yOffset); // total x and y offset
        }
        //max and min noise heights
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //clamps scale to greater than 0
        if(scale <= 0)
        {
            scale = 0.0000001f;
        }

        for(int y = 0; y < mapHeight; y++) //runs for the height of the noise map
        {
            for(int x = 0; x < mapWidth; x++) //runs for the width of the noise map
            {
                float amplitude = 1; //amplitude of octaves
                float frequency = 1; //frequency of octaves
                float noiseHeight = 0; //height at a point

                for(int i = 0; i < octaves; i++) //runs for no of octaves
                {
                    float sampleX = x / scale * frequency + octaveOffset[i].x; //the sample x value from which perlin noise is generated, divided by scale to get non integer numbers
                    float sampleY = y / scale * frequency + octaveOffset[i].y; //the sample y value from which perlin noise is generated, divided by scale to get non integer numbers
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //generates the "height" value
                    noiseHeight += perlinValue * amplitude; //gives you the height
                    amplitude *= persistance; //sets new amplitude
                    frequency *= lacunarity;  //sets new frequency
                }

                //updates max and min values
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight; //sets the height at that point
            }
        }

        for(int y = 0; y < mapHeight; y++) //runs for the height of the noise map
        {
            for(int x = 0; x < mapWidth; x++) //runs for the width of the noise map
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //normalises noisemap
            }
        }

        return noiseMap; //returns the completed noisemap
    }
}
