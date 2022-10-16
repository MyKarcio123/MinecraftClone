using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static int chunkSize = 16;
    public static int chunkHeight = 384;
    public Vector2 position;
    GameObject chunkObject;
    public float[,,] noiseValues;
    public float[,,] chunkData;
    public float[,] heightMap;
    MeshRenderer chunkMeshRenderer;
    MeshFilter chunkMeshFilter;
    List<Vector3> verticies;
    List<Vector2> uvs;
    List<int> triangles;
    BlocksData blocksData;
    public bool active = true;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector3 offset;

    public Chunk(GameObject parent, BlocksData blocksData,Vector2 position,NoiseData noiseData)
    {
        Vector3 positionInCoords = new Vector3(position.x*16, 0, position.y*16) + offset;
        chunkObject = new GameObject("Chunk");
        chunkData = new float[chunkSize,chunkHeight,chunkSize];
        //chunkData = Noise.Get3DNoise(seed, scale, octaves, persistance, lacunarity, positionInCoords,heightParams);
        noiseValues = Noise.Get2DNoise(noiseData, positionInCoords, chunkSize);
        chunkObject.transform.SetParent(parent.transform);
        chunkObject.transform.position += new Vector3(position.x*chunkSize, -128, position.y*chunkSize);
        chunkMeshRenderer = chunkObject.AddComponent<MeshRenderer>();
        chunkMeshFilter = chunkObject.AddComponent<MeshFilter>();
        verticies = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        this.blocksData = blocksData;
        this.position = position;
    }
    public void AddFaceToChunk(Vector3 position, int blockId, int faceType)
    {
        int vertexIndex = verticies.Count;
        float uvStartX;
        float uvSTartY;
        float atlasSize = 256;
        float blockSize = 16;
        int faceIndex = 0;
        BlockType blocks = blocksData.blocks[blockId];

        Vector2[] textures = blocks.atlsatTexture;
        if (faceType == 0 && textures.Length >= 4)
            faceIndex = 3;
        if (faceType == 4 && textures.Length >= 2)
            faceIndex = 1;
        else if (faceType == 5 && textures.Length >= 3)
            faceIndex = 2;
        uvStartX = blocks.atlsatTexture[faceIndex].x;
        uvSTartY = blocks.atlsatTexture[faceIndex].y;
        verticies.Add(VoxelData.voxelVerts[VoxelData.triangles[faceType, 0]] + position);
        verticies.Add(VoxelData.voxelVerts[VoxelData.triangles[faceType, 1]] + position);
        verticies.Add(VoxelData.voxelVerts[VoxelData.triangles[faceType, 2]] + position);
        verticies.Add(VoxelData.voxelVerts[VoxelData.triangles[faceType, 5]] + position);
        triangles.Add(0 + vertexIndex);
        triangles.Add(1 + vertexIndex);
        triangles.Add(2 + vertexIndex);
        triangles.Add(2 + vertexIndex);
        triangles.Add(1 + vertexIndex);
        triangles.Add(3 + vertexIndex);
        Vector2 leftDownUV = new Vector2((uvStartX * blockSize) / atlasSize, (atlasSize - (uvSTartY + 1) * blockSize) / atlasSize);
        uvs.Add(leftDownUV);
        uvs.Add(leftDownUV + new Vector2(0, blockSize / atlasSize));
        uvs.Add(leftDownUV + new Vector2(blockSize / atlasSize, 0));
        uvs.Add(leftDownUV + new Vector2(blockSize / atlasSize, blockSize / atlasSize));
    }
    public void CreateBlocksFromData()
    {
        Vector3[] moves = {
            new Vector3(0,0,-1),
            new Vector3(1,0,0),
            new Vector3(0,0,1),            
            new Vector3(-1,0,0),
            new Vector3(0,1,0),
            new Vector3(0,-1,0),
        };
        for(int x = 0; x < chunkSize; ++x)
        {
            for(int z = 0; z < chunkSize; ++z)
            {
                int surface = (int)noiseValues[3, x, z];
                Debug.Log(surface);
                for(int y=0; y < chunkHeight; ++y)
                {
                    if (y <= surface)
                        chunkData[x, y, z] = 1;
                    else if(y>surface && y<64)
                        chunkData[x, y, z] = 0;
                    else
                        chunkData[x, y, z] = -1;
                }
            }
        }
        for (int z = 0; z < chunkSize; ++z)
        {
            for (int x = 0; x < chunkSize; ++x)
            {
                for (int y = 0; y < chunkHeight; ++y)
                {
                    Vector3 position = new Vector3(x, y, z);
                    int type = 7;
                    if (chunkData[(int)position.x, (int)position.y, (int)position.z] < 0) continue;
                    else if (chunkData[(int)position.x, (int)position.y, (int)position.z] == 0) type = 3;
                    int iter = 0;
                    foreach(Vector3 move in moves)
                    {
                        Vector3 checkingBlock = position + move;
                        if(0<=checkingBlock.x && checkingBlock.x<chunkSize && 0 <= checkingBlock.z && checkingBlock.z < chunkSize && 0 <= checkingBlock.y && checkingBlock.y < chunkHeight)
                        {
                            if (chunkData[(int)checkingBlock.x,(int)checkingBlock.y,(int)checkingBlock.z] < 0)
                            {
                                AddFaceToChunk(position, type, iter);
                            }
                        }
                        ++iter;
                    }
                    if (y == 0)
                    {
                        AddFaceToChunk(position, type, 5);
                    }
                    else if (y == 128)
                    {
                        AddFaceToChunk(position, type, 4);
                    }
                    if (x == 0)
                    {
                        AddFaceToChunk(position, type, 3);
                    }
                    else if (x + 1 == chunkSize)
                    {
                        AddFaceToChunk(position, type, 1);
                    }
                    if (z == 0)
                    {
                        AddFaceToChunk(position, type, 0);
                    }
                    else if (z + 1 == chunkSize)
                    {
                        AddFaceToChunk(position, type, 2);
                    }
                }
            }
        }
 
    }
    public void MakeMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        chunkMeshFilter.mesh = mesh;
        chunkMeshRenderer.material = blocksData.material;
    }
    public void ChangeVisible(bool visible)
    {
        active = visible;
        chunkObject.SetActive(visible);
    }
}