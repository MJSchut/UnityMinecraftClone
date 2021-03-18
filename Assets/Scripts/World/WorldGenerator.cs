using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MinecraftClone.World
{
    public class WorldGenerator : MonoBehaviour
    {
        public int[,,] WorldData;
        public int NoiseOctaves = 3;
        public float NoiseFrequency = 1f;
        public Vector3Int ChunkDimensions = new Vector3Int(32, 32, 32);
        public Material GroundMaterial;

        MeshGenerator meshGenerator = new MeshGenerator();
        WorldData worldData;

        public void Awake()
        {
            worldData = new WorldData(this);
            worldData.SetChunkSize(new int[] { ChunkDimensions.x, ChunkDimensions.y, ChunkDimensions.z });
        }

        public void GenerateSingleMesh()
        {
            GenerateCube(Vector3.zero);
        }

        private void GenerateCube(Vector3 position)
        {
            Mesh singleQuad = meshGenerator.GenerateCube();

            GameObject go = new GameObject();
            go.name = "Single quad";
            go.transform.parent = transform;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = GroundMaterial;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = singleQuad;

            go.transform.position = position;
        }

        public void GenerateChunk(Vector3 position)
        {
            if (worldData == null)
                Awake();
           
            if (worldData.GenerateChunk(position) == false)
                return;

            int newIndex = worldData.chunks.Count - 1;
            worldData.GenerateChunkWithNoiseMap(newIndex);

            Mesh chunkMesh = meshGenerator.GenerateChunkMesh(worldData.chunks[newIndex].ChunkData);

            GameObject go = new GameObject();
            go.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
            go.transform.parent = transform;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = GroundMaterial;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = chunkMesh;

            go.transform.position = new Vector3(position.x * ChunkDimensions.x, position.y * ChunkDimensions.y, position.z * ChunkDimensions.z);
        }

        public void GenerateChunkAndSurroundingChunks(Vector3 position)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    GenerateChunk(new Vector3(position.x + x, position.y, position.z + z));
                }
            }
        }

        public void DestroyAll()
        {
            for (int i = transform.childCount-1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
