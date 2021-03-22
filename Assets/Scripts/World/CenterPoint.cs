using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MinecraftClone.World;

public class CenterPoint : MonoBehaviour
{

    WorldGenerator worldGen;
    Vector3 currentChunk = Vector3.zero;

    Thread WorldGenThread = null;
    EventWaitHandle WorldGenHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

    void CheckForWorldUpdate()
    {
        worldGen.GenerateChunkDataAndSurroundingChunks(new Vector3(currentChunk.x, 0, currentChunk.z), 3);
    }

    void RunWorldGenThread()
    {
        while (true) {
            WorldGenHandle.WaitOne();
            CheckForWorldUpdate();
        }
    }

    // Start is called before the first frame update 
    void Start()
    {
        transform.localPosition = new Vector3(8, 85, 8);
        worldGen = FindObjectOfType<WorldGenerator>();
        worldGen.GenerateChunkAndSurroundingChunks(new Vector3(currentChunk.x, 0, currentChunk.z), 4);

        WorldGenThread = new Thread(RunWorldGenThread);
        WorldGenThread.Start();
    }

    private void Update()
    {
        float chunkX = Mathf.Round(transform.position.x / worldGen.ChunkDimensions.x);
        float chunkZ = Mathf.Round(transform.position.z / worldGen.ChunkDimensions.z);

        if (currentChunk != new Vector3(chunkX, 0, chunkZ))
            WorldGenHandle.Set();

        currentChunk = new Vector3(chunkX, 0, chunkZ);
    }
}
