using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class WorldData
    {
        public List<Chunk> chunks = new List<Chunk>();
        public static int[] ChunkSize = new int[] { 32, 32, 32 };

        private WorldGenerator worldGen;

        public WorldData(WorldGenerator worldGen)
        {
            this.worldGen = worldGen;
        }

        public void GenerateChunk(Vector3 chunkPosition)
        {
            chunks.Add(new Chunk(chunkPosition));
        }

        public void GenerateRandomChunk(int index)
        {
            for (int x = 0; x < chunks[index].ChunkData.GetLength(0); x++)
            {
                for (int y = 0; y < chunks[index].ChunkData.GetLength(1); y++)
                {
                    for (int z = 0; z < chunks[index].ChunkData.GetLength(2); z++)
                    {
                        if (Random.value < 0.8)
                            chunks[index].ChunkData[x, y, z].SetType(VoxelType.GROUND);

                        // set neighbours here
                    }
                }
            }
        }

        public void GenerateChunkWithNoiseMap(int index)
        {
            for (int x = 0; x < chunks[index].ChunkData.GetLength(0); x++)
            {
                for (int z = 0; z < chunks[index].ChunkData.GetLength(2); z++)
                {
                    int y = Mathf.RoundToInt(
                        GenerateHeightMap(x, z) * chunks[index].ChunkData.GetLength(1) * 0.8f);

                    for (int yi = 0; yi < y; yi++)
                    {
                        chunks[index].ChunkData[x, yi, z].SetType(VoxelType.GROUND);
                    }
                }
            }
        }

        public float GenerateHeightMap(int x, int z)
        {
            float y = 0;
            for (int i = 0; i < worldGen.octaveFrequencyAndAmplitude.Count; i++)
                y += worldGen.octaveFrequencyAndAmplitude[i].x * Mathf.PerlinNoise(
                     (float)worldGen.octaveFrequencyAndAmplitude[i].y * (float)x,
                     (float)worldGen.octaveFrequencyAndAmplitude[i].y * (float)z);

            return y;
        }

        public void SetChunkSize(int[] chunkSize) => ChunkSize = chunkSize;
    }
}

