using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class Voxel
    {
        private Vector3 position = new Vector3(0, 0, 0);
        public VoxelType type { get; private set; } = VoxelType.AIR;
        private List<int> neighbours = new List<int>();

        public Voxel(Vector3 position, VoxelType type, List<int> neighbours)
        {
            this.position = position;
            this.type = type;
            this.neighbours = neighbours;
        }

        public void SetType(VoxelType type)
        {
            this.type = type;
        }
    }
}
