using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int size)
    {
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
    //Texture2D front, Texture2D right, Texture2D back, Texture2D left, Texture2D top, Texture2D bottom
    public static Texture2D TextureFromImages(Texture2D [] textures)
    {
        int texturesAmount = textures.Length;
        int size = 16 * 3;
        Color[] blockColor = new Color[size * size];
        Texture2D top = textures[0];
        Texture2D bottom = texturesAmount >= 2 ? textures[1] : top;
        Texture2D front = texturesAmount >= 3 ? textures[2] : top;
        Texture2D side = texturesAmount >= 4 ? textures[3] : front;
        for (int i = 0; i < 32; ++i)
        {
            for (int j = 0; j < 48; ++j)
            {
                if (i < 16 && j < 16)
                    blockColor[i * 48 + j] = front.GetPixel(j % 16, i % 16);
                else if (i < 16 && j <= 32)
                    blockColor[i * 48 + j] = side.GetPixel(j % 16, i % 16);
                else if (i < 16 && j <= 48)
                    blockColor[i * 48 + j] = side.GetPixel(j % 16, i % 16);
                else if (i <= 32 && j < 16)
                    blockColor[i * 48 + j] = side.GetPixel(j % 16, i % 16);
                else if (i <= 32 && j <= 32)
                    blockColor[i * 48 + j] = top.GetPixel(j % 16, i % 16);
                else if (i <= 32 && j <= 48)
                    blockColor[i * 48 + j] = bottom.GetPixel(j % 16, i % 16);
            }
        }
        return TextureFromColourMap(blockColor, size);
    }
}
