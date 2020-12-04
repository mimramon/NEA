using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer; //renderer object
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture) //draws the noisemap
    { 
        textureRenderer.sharedMaterial.mainTexture = texture; //sets texture
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height); //scales texture to the map size
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
