using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.util;

namespace MinecraftClone.World
{
    public class WorldData
    {
        public List<Chunk> chunks = new List<Chunk>();
        public static int[] ChunkSize = new int[] { 32, 32, 32 };
        private FastNoiseLite fastNoise = new FastNoiseLite();

        public WorldData(WorldGenerator worldGen)
        {
            fastNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoise.SetFractalOctaves(worldGen.NoiseOctaves);
            fastNoise.SetFrequency(worldGen.NoiseFrequency);
            fastNoise.SetSeed(Mathf.RoundToInt(Random.value * 10000f));
        }

        public bool GenerateChunk(Vector3 chunkPosition)
        {
            Chunk match = chunks.Find(chunk =>
                chunk.ChunkPosition.x == chunkPosition.x &
                chunk.ChunkPosition.y == chunkPosition.y &
                chunk.ChunkPosition.z == chunkPosition.z);

            if (match == null)
            {
                chunks.Add(new Chunk(chunkPosition));
                return true;
            }
            return false;  
            
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
                    int xoffset = Mathf.RoundToInt(x + (chunks[index].ChunkPosition.x * ChunkSize[0]));
                    int zoffset = Mathf.RoundToInt(z + (chunks[index].ChunkPosition.z * ChunkSize[2]));
                    int y = Mathf.RoundToInt(
                        GenerateHeightMap(xoffset, zoffset) * chunks[index].ChunkData.GetLength(1));

                    for (int yi = 0; yi < y; yi++)
                    {
                        chunks[index].ChunkData[x, yi, z].SetType(VoxelType.GROUND);
                    }
                }
            }
        }

        public float GenerateHeightMap(int x, int z)
        {
            return Mathf.Clamp(((fastNoise.GetNoise(x * 0.5f, z * 0.5f) + 1)/2 + (fastNoise.GetNoise(x * 0.2f, z * 0.2f) + 1) / 2)/2, 0, 1);
        }

        public void SetChunkSize(int[] chunkSize) => ChunkSize = chunkSize;
    }
}

