using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class WorldGenerator : MonoBehaviour
    {
        public int[,,] WorldData;
        public List<Vector2> octaveFrequencyAndAmplitude = new List<Vector2>() { new Vector2(1f, 0.1f) };
        public Vector3Int ChunkDimensions = new Vector3Int(32, 32, 32);
        public Material GroundMaterial;

        MeshGenerator meshGenerator = new MeshGenerator();

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

        public void GenerateChunk()
        {
            WorldData worldData = new WorldData(this);
            worldData.SetChunkSize(new int[] { ChunkDimensions.x, ChunkDimensions.y, ChunkDimensions.z });
            worldData.GenerateChunk(Vector3.zero);
            worldData.GenerateChunkWithNoiseMap(0);

            Mesh chunkMesh = meshGenerator.GenerateChunkMesh(worldData.chunks[0].ChunkData);

            GameObject go = new GameObject();
            go.name = "World Mesh";
            go.transform.parent = transform;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = GroundMaterial;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = chunkMesh;

            go.transform.position = Vector3.zero;
        }

        public void DestroyAll()
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
        }
    }
}
