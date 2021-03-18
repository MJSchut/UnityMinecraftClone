using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.World;

public class CenterPoint : MonoBehaviour
{
    WorldGenerator worldGen;
    Vector3 currentChunk = Vector3.zero;

    // Start is called before the first frame update 
    void Start()
    {
        transform.localPosition = new Vector3(36, 85, 36);
        worldGen = FindObjectOfType<WorldGenerator>();
        worldGen.GenerateChunkAndSurroundingChunks(Vector3.zero);
    }

    private void Update()
    {
        float chunkX = Mathf.Round(transform.position.x / worldGen.ChunkDimensions.x);
        float chunkZ = Mathf.Round(transform.position.z / worldGen.ChunkDimensions.z);

        if (currentChunk.x != chunkX | currentChunk.z != chunkZ)
        {
            worldGen.GenerateChunkAndSurroundingChunks(new Vector3(chunkX, 0, chunkZ));
            currentChunk = new Vector3(chunkX, 0, chunkZ);
        }
        
    }
}
