using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    int renderDistance = 2;
    public BlocksData blocksData;
    public Transform player;
    Vector2 currentChunk = new Vector2(0, 0);
    Dictionary<Vector2, Chunk> chunkList = new Dictionary<Vector2, Chunk>();
    List<Chunk> activeChunks = new List<Chunk>();
    private void Start()
    {
        SetActiveChunks();
    }
    private void Update()
    {
        if ((int)player.position.x / Chunk.chunkSize != currentChunk.x || (int)player.position.z / Chunk.chunkSize != currentChunk.y)
        {
            currentChunk = new Vector2(player.position.x, player.position.z);
            ClearChunks();
            SetActiveChunks();
        }
    }
    void SetActiveChunks()
    {
        for (int i = -renderDistance; i <= renderDistance; ++i)
        {
            for (int j = -renderDistance; j <= renderDistance; ++j)
            {
                Vector2 position = new Vector2((int)player.position.x / Chunk.chunkSize +i, (int)player.position.z / Chunk.chunkSize+j);
                Chunk currentChunk;
                if (chunkList.TryGetValue(position, out currentChunk))
                {
                    if (!currentChunk.active)
                    {
                        currentChunk.ChangeVisible(true);
                        activeChunks.Add(currentChunk);
                    }
                }
                else
                {
                    MakeChunk(position);
                }
            }
        }
    }
    void ClearChunks()
    {
        for(int i = 0; i < activeChunks.Count; ++i)
        {
            activeChunks[i].ChangeVisible(false);
        }
        activeChunks.Clear();
    }
    void MakeChunk(Vector2 position)
    {
        Chunk currentChunk = new Chunk(gameObject, blocksData,position);
        currentChunk.FillChunkData();
        currentChunk.CreateBlocksFromData();
        currentChunk.MakeMesh();
        chunkList.Add(position, currentChunk);
        activeChunks.Add(currentChunk);
    }

}
