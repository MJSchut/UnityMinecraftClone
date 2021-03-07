using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class WorldGenerator : MonoBehaviour
    {
        public int[,,] WorldData;
        public Vector3Int ChunkDimensions = new Vector3Int(32, 32, 32);
        public Material GroundMaterial;

        MeshGenerator meshGenerator = new MeshGenerator();

        public void GenerateSingleMesh()
        {
            GenerateCube(Vector3.zero);
        }

        private void GenerateCube(Vector3 position)
        {
            Mesh singleQuad = meshGenerator.GenerateCube(new List<int>());

            GameObject go = new GameObject();
            go.name = "Single quad";
            go.transform.parent = transform;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = GroundMaterial;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = singleQuad;

            go.transform.position = position;
        }

        public void GenerateWorld()
        {
            GenerateRandomWorldData();

            for (int x = 0; x < ChunkDimensions.x; x++)
            {
                for (int y = 0; y < ChunkDimensions.y; y++)
                {
                    for (int z = 0; z < ChunkDimensions.z; z++)
                    {
                        if (WorldData[x, y, z] == 1)
                        {
                            GenerateCube(new Vector3(x, y, z));
                        }
                    }
                }
            }
        }

        private void GenerateRandomWorldData()
        {
            WorldData = new int[ChunkDimensions.x, ChunkDimensions.y, ChunkDimensions.z];

            for (int x = 0; x < ChunkDimensions.x; x++)
            {
                for (int y = 0; y < ChunkDimensions.y; y++)
                {
                    for (int z = 0; z < ChunkDimensions.z; z++)
                    {
                        WorldData[x, y, z] = Random.value < 0.5f ? 1 : 0;
                    }
                }
            }
        }

        public void DestroyAll()
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
        }
    }
}
