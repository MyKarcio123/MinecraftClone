using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static int chunkSize = 16;
    public static int chunkHeight = 384;
    public Vector2 position;
    GameObject chunkObject;
    int[,,] chunkData;
    MeshRenderer chunkMeshRenderer;
    MeshFilter chunkMeshFilter;
    List<Vector3> verticies;
    List<Vector2> uvs;
    List<int> triangles;
    BlocksData blocksData;
    public bool active = true;
    public Chunk(GameObject parent, BlocksData blocksData,Vector2 position)
    {
        chunkObject = new GameObject("Chunk");
        chunkData = new int[chunkSize, chunkHeight, chunkSize];
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
    public void FillChunkData()
    {
        for (int y = 0; y <= 128; ++y)
        {
            for (int x = 0; x < chunkSize; ++x)
            {
                for (int z = 0; z < chunkSize; ++z)
                {
                    if (y > 0 && y < 117)
                    {
                        chunkData[x, y, z] = 1;
                    }
                    else if (y == 0)
                    {
                        chunkData[x, y, z] = 7;
                    }
                    else if (y == 128)
                    {
                        chunkData[x, y, z] = 2;
                    }
                    else
                    {
                        chunkData[x, y, z] = 3;
                    }
                }
            }
        }
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
        for (int y = 0; y <= 128; ++y)
        {
            for (int x = 0; x < chunkSize; ++x)
            {
                for (int z = 0; z < chunkSize; ++z)
                {
                    Vector3 position = new Vector3(x, y, z);
                    int type = chunkData[x, y, z];
                    if (y == 0)
                    {
                        AddFaceToChunk(position, type, 5);
                    }
                    else if (y == 128)
                    {
                        AddFaceToChunk(position, type, 4);
                    }
                    /*
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
                    */
                }
            }
        }
    }
    public void MakeMesh()
    {
        Mesh mesh = new Mesh();
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