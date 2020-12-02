using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer; //renderer object

    public void DrawNoiseMap(float[,] noiseMap) //draws the noisemap
    {
        int width = noiseMap.GetLength(0); //gets width from noisemap array
        int height = noiseMap.GetLength(1); //ges height from noisemap array

        Texture2D texture = new Texture2D(width, height); //creates new texture object

        Color[] colourMap = new Color[width*height]; //creates an array of colours for the texture

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]); //populates the colour array
            }
        }

        texture.SetPixels(colourMap); //sets all pixels to their respextive colour
        texture.Apply(); //applies changes
        textureRenderer.sharedMaterial.mainTexture = texture; //sets texture
        textureRenderer.transform.localScale = new Vector3(width, 1, height); //scales texture to the map size
    }
}
