using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AtlasMaker : EditorWindow
{
    int blockSize = 16;
    int blocksAmount = 16;
    int atlasSize;

    Object[] rawTextures;
    List<Texture2D> sortexTextures;
    Texture2D atlas;
    [MenuItem ("Minecraft/AtlasMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AtlasMaker));
    }
    private void OnGUI()
    {
        atlasSize = blockSize * blocksAmount;

        GUILayout.Label("Minecraft texture creator",EditorStyles.boldLabel);

        blockSize = EditorGUILayout.IntField("Block Size",blockSize);
        blocksAmount = EditorGUILayout.IntField("Blocks Amount", blocksAmount);

        GUILayout.Label(atlas);

        if (GUILayout.Button("Load Textures"))
        {
            sortexTextures = new List<Texture2D>();
            rawTextures = new Object[blocksAmount*blocksAmount];
            LoadTextures();
            CreateAtlas();
        }

        if(GUILayout.Button("Clear Textures"))
        {
            atlas = new Texture2D(atlasSize, atlasSize);
        }

        if(GUILayout.Button("Save Atlas"))
        {
            atlas.filterMode = FilterMode.Point;
            byte[] bytes = atlas.EncodeToPNG();
            try
            {
                File.WriteAllBytes(Application.dataPath + "/Textures/Atlas.png",bytes);
            }
            catch
            {
                Debug.LogError("AtlasMaker: Couldn't save atlas to file");
            }
        }
    }
    private void LoadTextures()
    {
        rawTextures = Resources.LoadAll("BlockTextures", typeof(Texture2D));
        int index = 0;
        foreach (Texture2D texture in rawTextures)
        {
            if(texture.width == blockSize && texture.height == texture.width)
            {
                sortexTextures.Add(texture);
            }
            else
            {
                Debug.LogWarning("AtlasMaker: " + texture.name + " incorrect size. Texture not loaded");
            }
            ++index;
        }
        Debug.Log("AtlasMaker: " + sortexTextures.Count + " textures successfully loaded");
    }
    private void CreateAtlas()
    {
        atlas = new Texture2D(atlasSize, atlasSize);
        Color[] colors = new Color[atlasSize * atlasSize];
        int currentBlockX;
        int currentBlockY;
        int index;
        for(int x = 0; x < atlasSize; ++x)
        {
            for (int y = 0; y < atlasSize; ++y)
            {
                currentBlockX = x / blockSize;
                currentBlockY = y / blockSize;

                index = currentBlockY * blocksAmount + currentBlockX;

                if (index < sortexTextures.Count)
                    colors[(atlasSize - y - 1) * atlasSize + x] = sortexTextures[index].GetPixel(x, blockSize - y - 1);
                else
                    colors[(atlasSize - y - 1) * atlasSize + x] = Color.clear;
            }
        }

        atlas.SetPixels(colors);
        atlas.Apply();
    }
}
