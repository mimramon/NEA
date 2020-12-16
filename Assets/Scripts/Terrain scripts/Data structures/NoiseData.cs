using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseData", menuName = "Data Objects/Noise Data", order = 0)]
public class NoiseData : ScriptableObject 
{
    public float noiseScale; //scale of noise
    public int octaves; //determines No of octaves
    [Range(0,1)]
    public float persistance; //determines persistance in range 0-1
    public float lacunarity; //determines lacunarity
    public int seed; //determines seed
    public Vector2 offset; //determines offset
    public NoiseGenerator.NormalizeMode normalizeMode;
}


