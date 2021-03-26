using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MinecraftClone.World
{
    public class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator instance { get; private set; }
        public int[,,] WorldData;

        public int NoiseOctaves = 3;
        public float NoiseFrequency = 1f;
        public int NoiseOctaves2 = 3;
        public float NoiseFrequency2 = 1f;

        public Vector3Int ChunkDimensions = new Vector3Int(32, 32, 32);
        public Material GroundMaterial;
        public Queue<GameObject> gameObjectPool = new Queue<GameObject>();
        public int GameObjectPoolLength = 500;

        MeshGenerator meshGenerator = new MeshGenerator();
        WorldData worldData;
        Queue<MeshData> ChunkDataQueue = new Queue<MeshData>();
        Queue<Vector3> ChunkPoolQueue = new Queue<Vector3>();

        List<Vector3> ActiveChunks = new List<Vector3>();
        Dictionary<Vector3, GameObject> PositionToGameObjectDict = new Dictionary<Vector3, GameObject>();

        public void Awake()
        {
            instance = this;
            worldData = new WorldData(this);
            worldData.SetChunkSize(new int[] { ChunkDimensions.x, ChunkDimensions.y, ChunkDimensions.z });
        }

        public void Start()
        {
            for (int i = 0; i < GameObjectPoolLength; i++)
                gameObjectPool.Enqueue(CreateEmptyObject());
        }

        public GameObject CreateEmptyObject()
        {
            GameObject pooledObject = new GameObject();
            pooledObject.transform.parent = transform;
            return SetEmptyGameObjectComponents(pooledObject);
        }

        public GameObject CreateEmptyObject(GameObject pooledObject)
        {
            return SetEmptyGameObjectComponents(pooledObject);
        }

        private GameObject SetEmptyGameObjectComponents(GameObject pooledObject)
        {
            MeshRenderer mr;

            if (pooledObject.GetComponent<MeshRenderer>() == null)
                mr = pooledObject.AddComponent<MeshRenderer>();
            else
                mr = pooledObject.GetComponent<MeshRenderer>();

            mr.material = GroundMaterial;

            MeshFilter mf;
            if (pooledObject.GetComponent<MeshFilter>() == null)
            {
                mf = pooledObject.AddComponent<MeshFilter>();
                mf.mesh = new Mesh();
            }
            else
            {
                mf = pooledObject.GetComponent<MeshFilter>();
                mf.mesh.Clear();
            }

            MeshCollider mc;
            if (pooledObject.GetComponent<MeshCollider>() == null)
            {
                mc = pooledObject.AddComponent<MeshCollider>();
                mc.sharedMesh = new Mesh();
            }
            else
            {
                mc = pooledObject.AddComponent<MeshCollider>();
                mc.sharedMesh.Clear();
            }

            Rigidbody rig;
            if (pooledObject.GetComponent<Rigidbody>() == null)
            {
                rig = pooledObject.AddComponent<Rigidbody>();
                rig.isKinematic = true;
                rig.useGravity = false;
            }
            else
            {
                rig = pooledObject.GetComponent<Rigidbody>();
                rig.isKinematic = true;
                rig.useGravity = false;
            }


            pooledObject.name = "Pooled object";
            pooledObject.SetActive(false);
            return pooledObject;
        }

        public void Update()
        {
            if (ChunkDataQueue.Count > 1)
                GenerateChunkObject(ChunkDataQueue.Dequeue());

            if (ChunkPoolQueue.Count > 1)
                PoolChunk(ChunkPoolQueue.Dequeue());
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

        public MeshData GenerateChunkMesh(Vector3 position, bool queue=true, bool regen=false)
        {
            if (ActiveChunks.Contains(position) && regen == false)
                return null;

            MeshData meshData = meshGenerator.GenerateChunkMesh(
                worldData, position);

            meshData.position = position;
            if(queue == true)
                ChunkDataQueue.Enqueue(meshData);

            if (regen == false)
                ActiveChunks.Add(position);

            return meshData;
        }

        public bool GenerateChunkData(Vector3 position)
        {
            if (worldData == null)
                Awake();

            if (worldData.GenerateChunk(position) == false)
                return false;

            int newIndex = worldData.chunks.Count - 1;
            worldData.GenerateChunkWithNoiseMap(newIndex);

            return true;
        }

        public GameObject GenerateChunkObject(MeshData meshData)
        {
            GameObject go;
            if (gameObjectPool.Count > 0)
                go = gameObjectPool.Dequeue();
            else
                go = CreateEmptyObject();

            SetMeshData(go, meshData);

            Vector3 position = meshData.position;
            go.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
            go.transform.parent = transform;
            go.SetActive(true);

            go.transform.position = new Vector3(position.x * ChunkDimensions.x, position.y * ChunkDimensions.y, position.z * ChunkDimensions.z);

            RegisterChunk(go, position);

            return go;
        }

        public void SetMeshData(GameObject go, MeshData meshData) {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            mf.mesh.vertices = meshData.Vertices.ToArray();
            mf.mesh.triangles = meshData.Triangles.ToArray();
            mf.mesh.uv = meshData.Uvs.ToArray();

            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();

            MeshCollider mc = go.GetComponent<MeshCollider>();
            mc.sharedMesh = mf.mesh;
        }

        public void PoolChunk(Vector3 position)
        {
            PositionToGameObjectDict.TryGetValue(position, out GameObject thisObject);
            if (thisObject != null)
            {
                CreateEmptyObject(thisObject);
                UnregisterChunk(position);
            }
        }

        public void RegisterChunk(GameObject go, Vector3 position)
        {
            PositionToGameObjectDict[position] = go;
        }

        public void UnregisterChunk(Vector3 position)
        {
            gameObjectPool.Enqueue(PositionToGameObjectDict[position]);
            PositionToGameObjectDict.Remove(position);
        }

        public void GenerateChunk(Vector3 position)
        {
            MeshData meshData = new MeshData();
            GenerateChunkData(position);
            GenerateChunkMesh(position, false);

            GenerateChunkObject(meshData);
        }

        public void UpdateVoxelData(Vector3 chunkPosition, Vector3Int voxelPosition, VoxelType newType)
        {
            if (worldData == null)
                Awake();

            Chunk chunk = worldData.GetChunk(chunkPosition);
            int xlength = chunk.ChunkData.GetLength(0);
            int ylength = chunk.ChunkData.GetLength(1);
            int zlength = chunk.ChunkData.GetLength(2);

            int voxelX = voxelPosition.x;
            int voxelY = voxelPosition.y;
            int voxelZ = voxelPosition.z;

            if (voxelX < 0 || voxelX >= xlength)
                return;

            if (voxelY < 0 || voxelY >= ylength)
                return;

            if (voxelZ < 0 || voxelZ >= zlength)
                return;

            chunk.ChunkData[voxelX, voxelY, voxelZ].SetType(newType);

            MeshData meshData = GenerateChunkMesh(chunkPosition, false, true);
            SetMeshData(PositionToGameObjectDict[chunkPosition], meshData);

            Vector3 neighbourPosition = Vector3.zero;

            if (voxelX == 0)
                neighbourPosition = new Vector3(chunkPosition.x - 1, chunkPosition.y, chunkPosition.z);

            else if (voxelX == xlength)
                neighbourPosition = new Vector3(chunkPosition.x + 1, chunkPosition.y, chunkPosition.z);

            if (neighbourPosition != Vector3.zero)
            {
                MeshData neighbourData = GenerateChunkMesh(neighbourPosition, false, true);
                SetMeshData(PositionToGameObjectDict[neighbourPosition], neighbourData);
            }

            neighbourPosition = Vector3.zero;

            if (voxelZ == 0)
                neighbourPosition = new Vector3(chunkPosition.x, chunkPosition.y, chunkPosition.z - 1);

            else if (voxelZ == zlength)
                neighbourPosition = new Vector3(chunkPosition.x, chunkPosition.y, chunkPosition.z + 1);

            if (neighbourPosition != Vector3.zero)
            {
                MeshData neighbourData = GenerateChunkMesh(neighbourPosition, false, true);
                SetMeshData(PositionToGameObjectDict[neighbourPosition], neighbourData);
            }
        }

        public void GenerateChunksAndSurroundingChunks(Vector3 position, int rings = 1)
        {
            // generate one unit further than mesh to make sure meshes are complete
            for (int x = -rings - 1; x <= rings + 1; x++)
            {
                for (int z = -rings - 1; z <= rings + 1; z++)
                {
                    GenerateChunkData(new Vector3(position.x + x, position.y, position.z + z));
                }
            }

            for (int x = -rings; x <= rings; x++)
            {
                for (int z = -rings; z <= rings; z++)
                {
                    GenerateChunkMesh(new Vector3(position.x + x, position.y, position.z + z));
                }
            }
        }

        public void PoolChunksTooFarAway(Vector3 position, int rings = 1)
        {
            if (ActiveChunks.Count == 0)
                return;

            List<int> toRemove = new List<int>();
            for (int i = ActiveChunks.Count - 1; i >= 0; i--)
            {
                Vector3 chunkposition = ActiveChunks[i];

                if (Mathf.Abs(position.x - chunkposition.x) > (float)rings ||
                    Mathf.Abs(position.z - chunkposition.z) > (float)rings)
                {
                    ChunkPoolQueue.Enqueue(chunkposition);
                    toRemove.Add(i);
                }
            }

            if (toRemove.Count > 0)
            {
                toRemove.Sort();
                toRemove.Reverse();
                toRemove.ForEach(i => ActiveChunks.RemoveAt(i));
            }    
        }

        public void DestroyAll()
        {
            for (int i = transform.childCount-1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
