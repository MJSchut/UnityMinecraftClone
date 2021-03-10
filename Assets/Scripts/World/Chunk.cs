using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class Chunk
    {
        public Vector3 ChunkPosition = new Vector3(0, 0, 0);
        public Voxel[,,] ChunkData;

        public Chunk(Vector3 ChunkPosition)
        {
            this.ChunkPosition = ChunkPosition;

            ChunkData =
                new Voxel[WorldData.ChunkSize[0], WorldData.ChunkSize[1], WorldData.ChunkSize[2]];

            for (int x = 0; x < ChunkData.GetLength(0); x++)
            {
                for (int y = 0; y < ChunkData.GetLength(1); y++)
                {
                    for (int z = 0; z < ChunkData.GetLength(2); z++)
                    {
                        ChunkData[x, y, z] = new Voxel(new Vector3(x, y, z), VoxelType.AIR, new List<int>());
                    }
                }
            }
        }
    }
}
