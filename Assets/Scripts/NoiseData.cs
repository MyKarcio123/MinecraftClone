using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise data", menuName = "Data/Noise Data")]
public class NoiseData : ScriptableObject
{
    public NoiseParams[] NoiseParams;
}
[System.Serializable]
public struct NoiseParams
{
    public string name;

    public float noiseScale;

    public int octaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector3 offset;

    public AnimationCurve heightCurve;

    public int weight;

    public bool enable;
}
