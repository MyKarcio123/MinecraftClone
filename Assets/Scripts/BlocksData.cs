using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Block data",menuName ="Data/Block Data")]
public class BlocksData : ScriptableObject
{
    public Material material;
    public BlockType[] blocks;
}
[System.Serializable]
public struct BlockType
{
    public string name;
    public string id;
    public Vector2 [] atlsatTexture;
}
