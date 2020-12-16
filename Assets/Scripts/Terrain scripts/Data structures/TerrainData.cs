using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Data Objects/Terrain Data", order = 0)]
public class TerrainData : ScriptableObject 
{
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public float uniformScale = 1f;
}
