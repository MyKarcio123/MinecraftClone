using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    int renderDistance = 5;
    public BlocksData blocksData;
    public NoiseData noiseData;
    public Transform player;
    Vector2 currentChunk = new Vector2(0, 0);
    Dictionary<Vector2, Chunk> chunkList = new Dictionary<Vector2, Chunk>();
    List<Chunk> activeChunks = new List<Chunk>();
    [Header("Noise Properties")]
    public float noiseScale;

    public int octaves;

    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector3 offset;

    public AnimationCurve heightCurve;
    private void Start()
    {
        SetActiveChunks(Vector2.zero);
    }
    private void Update()
    {
        Vector2 position = new Vector2(Mathf.FloorToInt(player.position.x / Chunk.chunkSize), Mathf.FloorToInt(player.position.z / Chunk.chunkSize));
        if (position.x != currentChunk.x || position.y != currentChunk.y)
        {
            currentChunk = new Vector2(player.position.x, player.position.z);
            ClearChunks();
            SetActiveChunks(position);
        }
    }
    void SetActiveChunks(Vector2 position)
    {
        for (int i = -renderDistance; i <= renderDistance; ++i)
        {
            for (int j = -renderDistance; j <= renderDistance; ++j)
            {
                Vector2 newPosition = new Vector2(i,j)+position;
                Chunk currentChunk;
                if (chunkList.TryGetValue(newPosition, out currentChunk))
                {
                    if (!currentChunk.active)
                    {
                        currentChunk.ChangeVisible(true);
                        activeChunks.Add(currentChunk);
                    }
                }
                else
                {
                    MakeChunk(newPosition);
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
        Chunk currentChunk = new Chunk(gameObject, blocksData, position, noiseData);
        currentChunk.CreateBlocksFromData();
        currentChunk.MakeMesh();
        chunkList.Add(position, currentChunk);
        activeChunks.Add(currentChunk);
    }
}
