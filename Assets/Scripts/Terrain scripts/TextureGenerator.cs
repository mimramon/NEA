using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height); //calls constructor
        texture.filterMode = FilterMode.Point; //sets the filter mode
        texture.wrapMode = TextureWrapMode.Clamp; //sets the wrap mode
        texture.SetPixels(colourMap); //applies colourmap to texture
        texture.Apply(); //applies changes
        return texture; //returns the texture
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0); //gets width from noisemap array
        int height = heightMap.GetLength(1); //ges height from noisemap array

        Color[] colourMap = new Color[width * height]; //creates an array of colours for the texture

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]); //populates the colour array
            }
        }
        return TextureFromColourMap(colourMap, width, height);
    }
}
