using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MinecraftClone.World;

public class CenterPoint : MonoBehaviour
{
    public int InitalRings = 7;
    public int UpdateRings = 7;
    public int DeactivateRings = 10;

    WorldGenerator worldGen;
    Vector3 currentChunk = Vector3.zero;

    Thread WorldGenThread = null;
    Thread DeactivateThread = null;

    void RunWorldGenThread()
    {
        while (true)
            worldGen.GenerateChunksAndSurroundingChunks(new Vector3(currentChunk.x, 0, currentChunk.z), UpdateRings);
    }

    void RunWorldDeactivateThread()
    {
        while (true)
            worldGen.PoolChunksTooFarAway(new Vector3(currentChunk.x, 0, currentChunk.z), DeactivateRings);
    }

    // Start is called before the first frame update 
    void Start()
    {
        transform.localPosition = new Vector3(8, 85, 8);
        worldGen = FindObjectOfType<WorldGenerator>();
        worldGen.GenerateChunksAndSurroundingChunks(new Vector3(currentChunk.x, 0, currentChunk.z), InitalRings);

        WorldGenThread = new Thread(RunWorldGenThread);
        WorldGenThread.Start();

        DeactivateThread = new Thread(RunWorldDeactivateThread);
        DeactivateThread.Start();
    }

    private void Update()
    {
        float chunkX = Mathf.Round(transform.position.x / worldGen.ChunkDimensions.x);
        float chunkZ = Mathf.Round(transform.position.z / worldGen.ChunkDimensions.z);

        currentChunk = new Vector3(chunkX, 0, chunkZ);
    }
}
