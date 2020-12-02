using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(ChunkGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ChunkGenerator chunkGen = (ChunkGenerator)target;

        if(DrawDefaultInspector())
        {
            if(chunkGen.autoUpdate)
            {
                chunkGen.GenerateChunk();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            chunkGen.GenerateChunk();
        }
    }
}
