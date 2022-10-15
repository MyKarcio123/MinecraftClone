using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NoiseMapVisualization : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Vector3 offset;
    public NoiseData noiseData;

    public void DrawInEditor()
    {
        DrawTexture(TextureFromHeightMap(Noise.Get2DNoise(noiseData,offset, 16*16)));
    }
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
    }
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public Texture2D TextureFromHeightMap(float [,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);


        Color[] colors = new Color[width * height];
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                colors[width * i + j] = Color.Lerp(Color.black, Color.white, (heightMap[j, i]+1)/2);
            }
        }

        return TextureFromColorMap(colors, width, height);
    }

    private void OnValidate()
    {
        DrawInEditor();
    }
    [InspectorButton("OnButtonClicked")]
    public bool clickMe;

    private void OnButtonClicked()
    {
        Debug.Log("Clicked!");
        DrawInEditor();
    }
}
