using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float[,,] Get2DNoise (NoiseData noiseData, Vector3 offset,int _size)
    {
        int size = _size;
        int height = _size;
        int halfSize = size / 2;
        int halfHeight = height / 2;
        int noiseAmount = noiseData.NoiseParams.Length;
        int weightSum = 0;
        float[,,] noiseMap = new float[noiseAmount+1,size, height];
        System.Random prng = new System.Random(noiseData.NoiseParams[0].seed);
        Vector2[] octavesOffsets = new Vector2[noiseData.NoiseParams[0].octaves];
        float[,] minMaxValue = new float[noiseAmount, 2];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        for (int i = 0; i < noiseAmount; ++i)
        {
            minMaxValue[i,0]= float.MinValue;
            minMaxValue[i,1]= float.MaxValue;
            if (noiseData.NoiseParams[i].enable)
                weightSum += noiseData.NoiseParams[i].weight;
        }
        for (int i = 0; i < noiseData.NoiseParams[0].octaves; ++i)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.z;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= noiseData.NoiseParams[0].persistance;
        }
        if (noiseData.NoiseParams[0].noiseScale <= 0) noiseData.NoiseParams[0].noiseScale = 0.001f;
        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < height; ++y)
            {

                for (int noise = 0; noise < noiseAmount; ++noise)
                {
                    if (noiseData.NoiseParams[noise].enable)
                    {
                        amplitude = 1;
                        float frequency = 1;
                        float noiseValue = 0;
                        for (int k = 0; k < noiseData.NoiseParams[noise].octaves; ++k)
                        {
                            float sampleX = (x - halfSize + octavesOffsets[k].x) / noiseData.NoiseParams[noise].noiseScale * frequency;
                            float sampleY = (y - halfHeight + octavesOffsets[k].y) / noiseData.NoiseParams[noise].noiseScale * frequency;

                            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;

                            noiseValue += perlinValue * amplitude;

                            amplitude *= noiseData.NoiseParams[noise].persistance;
                            frequency *= noiseData.NoiseParams[noise].lacunarity;
                        }
                        if (noiseValue > minMaxValue[noise,0])
                        {
                            minMaxValue[noise, 0] = noiseValue;
                        }
                        else if (noiseValue < minMaxValue[noise, 1])
                        {
                            minMaxValue[noise, 1] = noiseValue;
                        }
                        noiseMap[noise, x, y] = noiseValue;
                    }
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseSum = 0;
                for (int noise = 0; noise < noiseAmount; ++noise)
                {
                    float normalizeHeight = (noiseMap[noise,x,y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[noise,x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                    //noiseMap[noise, x, y] = Mathf.InverseLerp(minMaxValue[noise, 1], minMaxValue[noise, 0], noiseMap[noise, x, y]);
                    noiseSum += noiseData.NoiseParams[noise].heightCurve.Evaluate(noiseMap[noise, x, y]) * noiseData.NoiseParams[noise].weight;
                }
                noiseMap[noiseAmount, x, y] = noiseSum / weightSum;
            }
        }
        return noiseMap;
    }
    public static float[,,] Get3DNoise(int seed, float scale, int octaves, float persistance, float lacunarity, Vector3 offset,AnimationCurve heightParams)
    {
        float under0Value = 0.4f / 128f;
        float over0Value = -0.3f / 256f;
        int size = Chunk.chunkSize;
        int height = Chunk.chunkHeight;
        float[,,] noiseMap = new float[size, height, size];
        System.Random prng = new System.Random(seed);
        Vector3[] octavesOffsets = new Vector3[octaves];
        for(int i = 0; i < octaves; ++i)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            float offsetZ = prng.Next(-100000, 100000) + offset.z;
            octavesOffsets[i] = new Vector3(offsetX, offsetY, offsetZ);
        }
        if (scale <= 0) scale = 0.001f;
        for(int x=0; x < size; ++x)
        {
            for(int z = 0; z < size; ++z)
            {
                for(int y = 0; y < height; ++y)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseValue = 0;
                    for (int k = 0; k < octaves; ++k)
                    {
                        float sampleX = (x + octavesOffsets[k].x) / scale * frequency;
                        float sampleY = (y + octavesOffsets[k].y) / scale * frequency;
                        float sampleZ = (z + octavesOffsets[k].z) / scale * frequency;

                        float noiseXY = Mathf.PerlinNoise(sampleX, sampleY);
                        float noiseXZ = Mathf.PerlinNoise(sampleX, sampleZ);
                        float noiseYZ = Mathf.PerlinNoise(sampleY, sampleZ);                        
                        
                        float noiseYX = Mathf.PerlinNoise(sampleY, sampleX);
                        float noiseZX = Mathf.PerlinNoise(sampleZ, sampleX);
                        float noiseZY = Mathf.PerlinNoise(sampleZ, sampleY);

                        float perlinValue = (noiseXY + noiseXZ + noiseYZ + noiseYX + noiseZX + noiseZY) / 6.0f;
                        perlinValue = perlinValue * 2 - 1;

                        noiseValue += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    if (y < 128) noiseValue += (under0Value * (128-y));
                    else noiseValue += (over0Value * (y - 128));
                    noiseMap[x, y, z] = noiseValue;
                }
            }
        }
        return noiseMap;
    }
}
