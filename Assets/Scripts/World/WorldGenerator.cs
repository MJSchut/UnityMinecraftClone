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
        public Queue<GameObject> gameObjectPool = new Queue<GameObject>();
        public int GameObjectPoolLength = 500;

        MeshGenerator meshGenerator = new MeshGenerator();
        WorldData worldData;
        Queue<MeshData> ChunkDataQueue = new Queue<MeshData>();

        public void Awake()
        {
            worldData = new WorldData(this);
            worldData.SetChunkSize(new int[] { ChunkDimensions.x, ChunkDimensions.y, ChunkDimensions.z });

            
        }

        public void Start()
        {
            for (int i = 0; i < GameObjectPoolLength; i++)
            {
                gameObjectPool.Enqueue(CreateEmptyObject());
            }
        }

        public GameObject CreateEmptyObject()
        {
            GameObject pooledObject = new GameObject();
            pooledObject.transform.parent = transform;

            MeshRenderer mr = pooledObject.AddComponent<MeshRenderer>();
            mr.material = GroundMaterial;

            MeshFilter mf = pooledObject.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();

            pooledObject.name = "Pooled object";
            pooledObject.SetActive(false);

            return pooledObject;
        }

        public void LateUpdate()
        {
            if (ChunkDataQueue.Count > 1)
            {
                GenerateChunkObject(ChunkDataQueue.Dequeue());
            }
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

        public MeshData GenerateChunkData(Vector3 position, bool queue=true)
        {
            if (worldData == null)
                Awake();

            if (worldData.GenerateChunk(position) == false)
                return null;

            int newIndex = worldData.chunks.Count - 1;
            worldData.GenerateChunkWithNoiseMap(newIndex);

            MeshData meshData = meshGenerator.GenerateChunkMesh(worldData.chunks[newIndex].ChunkData);
            meshData.position = position;
            if(queue == true)
                ChunkDataQueue.Enqueue(meshData);
            return meshData;
        }

        public GameObject GenerateChunkObject(MeshData meshData)
        {
            GameObject go;
            if (gameObjectPool.Count > 0)
                go = gameObjectPool.Dequeue();
            else
                go = CreateEmptyObject();

            MeshFilter mf = go.GetComponent<MeshFilter>();
            mf.mesh.vertices = meshData.Vertices.ToArray();
            mf.mesh.triangles = meshData.Triangles.ToArray();
            mf.mesh.uv = meshData.Uvs.ToArray();

            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();

            Vector3 position = meshData.position;
            go.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
            go.transform.parent = transform;
            go.SetActive(true);

            go.transform.position = new Vector3(position.x * ChunkDimensions.x, position.y * ChunkDimensions.y, position.z * ChunkDimensions.z);
            return go;
        }

        public void GenerateChunk(Vector3 position)
        {
            MeshData meshData = new MeshData();
            meshData = GenerateChunkData(position, false);

            GenerateChunkObject(meshData);
        }

        public void GenerateChunkAndSurroundingChunks(Vector3 position, int rings=1)
        {
            for (int x = -rings; x <= rings; x++)
            {
                for (int z = -rings; z <= rings; z++)
                {
                    GenerateChunk(new Vector3(position.x + x, position.y, position.z + z));
                }
            }
        }

        public void GenerateChunkDataAndSurroundingChunks(Vector3 position, int rings = 1)
        {
            for (int x = -rings; x <= rings; x++)
            {
                for (int z = -rings; z <= rings; z++)
                {
                    GenerateChunkData(new Vector3(position.x + x, position.y, position.z + z));
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
