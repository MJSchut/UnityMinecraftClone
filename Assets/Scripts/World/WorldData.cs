using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftClone.util;
using System.Linq;

namespace MinecraftClone.World
{
    public class WorldData
    {
        public List<Chunk> chunks = new List<Chunk>();
        public static int[] ChunkSize = new int[] { 32, 32, 32 };
        private FastNoiseLite fastNoise = new FastNoiseLite();
        private FastNoiseLite fastNoise2 = new FastNoiseLite();

        public WorldData(WorldGenerator worldGen)
        {
            fastNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoise.SetFractalOctaves(worldGen.NoiseOctaves);
            fastNoise.SetFrequency(worldGen.NoiseFrequency);
            fastNoise.SetSeed(Mathf.RoundToInt(Random.value * 10000f));

            fastNoise2.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoise2.SetFractalOctaves(worldGen.NoiseOctaves2);
            fastNoise2.SetFrequency(worldGen.NoiseFrequency2);
            fastNoise2.SetFractalType(FastNoiseLite.FractalType.Ridged);
            fastNoise2.SetFractalLacunarity(3f);
            fastNoise.SetCellularJitter(1.5f);
            fastNoise2.SetSeed(Mathf.RoundToInt(Random.value * 10000f));
        }

        public bool GenerateChunk(Vector3 chunkPosition)
        {
            Chunk match = GetChunk(chunkPosition);

            if (match == null)
            {
                chunks.Add(new Chunk(chunkPosition));
                return true;
            }
            return false;  
        }

        public Chunk GetChunk(Vector3 chunkPosition)
        {
            return chunks.Find(chunk => chunk.ChunkPosition.x == chunkPosition.x &
                chunk.ChunkPosition.y == chunkPosition.y &
                chunk.ChunkPosition.z == chunkPosition.z);
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
                    int yx = Mathf.RoundToInt(
                        GenerateHeightMap(xoffset, zoffset) * chunks[index].ChunkData.GetLength(1));

                    for (int yi = 0; yi < yx; yi++)
                    {
                        chunks[index].ChunkData[x, yi, z].SetType(VoxelType.GROUND);
                    }

                    for (int y = 0; y < chunks[index].ChunkData.GetLength(1); y++)
                    {
                        if (chunks[index].ChunkData[x, y, z].type == VoxelType.GROUND)
                            chunks[index].ChunkData[x, y, z].SetType(GenerateCaveMap(xoffset, y, zoffset));
                    }
                }
            }
        }

        public float GenerateHeightMap(int x, int z)
        {
            float baseLayer = (fastNoise.GetNoise(x, z) + 1)/2;
            float secondaryLayer = Mathf.Clamp(((fastNoise2.GetNoise(x * 0.5f, z * .5f) + 1) / 1.6f), 0, 0.99f);
            float secondaryMask = fastNoise2.GetNoise(x * .001f, z * .001f) + 0.2f;

            baseLayer += (secondaryLayer * secondaryMask);
            baseLayer /= 2;


            return Mathf.Clamp(baseLayer, 0.01f, 0.99f);
        }

        public VoxelType GenerateCaveMap(int x, int y, int z)
        {
            VoxelType thisType = VoxelType.GROUND;

            float caveNoise1 = fastNoise.GetNoise(x * 3f, y * 2, z * 3f);
            float caveMask = fastNoise2.GetNoise(x * .001f, y * .3f, z * .001f) - 0.4f;

            if (caveNoise1 > caveMask)
                thisType = VoxelType.AIR;

            return thisType;
        }

        public void SetChunkSize(int[] chunkSize) => ChunkSize = chunkSize;
    }
}

