using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class MeshGenerator
    {
        private List<Vector3> vertexList = new List<Vector3>();
        private int verticesPerPolygon = 4;
        private int sidesPerCube = 6;
        private System.Random random = new System.Random();

        private const int bottomIndex = 0;
        private const int leftIndex = 1;
        private const int frontIndex = 2;
        private const int backIndex = 3;
        private const int rightIndex = 4;
        private const int topIndex = 5;

        private Dictionary<int, int[]> polygonToVertex =
            new Dictionary<int, int[]>();

        public MeshGenerator() {
            vertexList = GetVertices();

            polygonToVertex[bottomIndex] = new int[4] { 0, 1, 2, 3 };
            polygonToVertex[leftIndex] = new int[4] { 3, 7, 4, 0 };
            polygonToVertex[frontIndex] = new int[4] { 0, 4, 5, 1 };
            polygonToVertex[backIndex] = new int[4] { 2, 6, 7, 3 };
            polygonToVertex[rightIndex] = new int[4] { 1, 5, 6, 2 };
            polygonToVertex[topIndex] = new int[4] { 4, 7, 6, 5 };
        }

        public Mesh GenerateCube()
        {
            Mesh quad = new Mesh();

            quad.vertices = GetPolygons(Vector3.zero);
       
            quad.uv = GetUVMap(new Vector2(1f, 19f));
            quad.triangles = GetTriangles(6).ToArray();

            quad.RecalculateBounds();
            quad.RecalculateNormals();
            return quad;
        }

        public MeshData GenerateChunkMesh(WorldData worldData, Vector3 position)
        {
            Voxel[,,] chunkData = worldData.GetChunk(position).ChunkData;

            MeshData meshData = new MeshData();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            int numberOfFaces = 0;

            for (int x = 0; x < chunkData.GetLength(0); x++)
            {
                for (int y = 0; y < chunkData.GetLength(1); y++) 
                {
                    for (int z = 0; z < chunkData.GetLength(2); z++)
                    {
                        if (chunkData[x, y, z].type == VoxelType.GROUND)
                        {
                            Vector3 offset = new Vector3(x, y, z);

                            if (y != 0)
                            {
                                if (chunkData[x, y - 1, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(BottomPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(4f, 19f)));
                                    numberOfFaces++;
                                }   
                            }

                            if (y != chunkData.GetLength(1) - 1)
                            {
                                if (chunkData[x, y + 1, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(TopPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(1f, 19f)));
                                    numberOfFaces++;
                                } 
                            }

                            if (x != 0)
                            {
                                if (chunkData[x - 1, y, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(LeftPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                } 
                            } else // x is 0
                            {
                                Chunk neighbour = worldData.GetChunk(new Vector3(position.x - 1, position.y, position.z));

                                if (neighbour?.ChunkData[chunkData.GetLength(0) - 1, y, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(LeftPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }
                            }

                            if (x != chunkData.GetLength(0) - 1)
                            {
                                if (chunkData[x + 1, y, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(RightPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }  
                            }
                            else // x is equal to chunk width
                            {
                                Chunk neighbour = worldData.GetChunk(new Vector3(position.x + 1, position.y, position.z));
                                if (neighbour?.ChunkData[0, y, z].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(RightPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }
                            }

                            if (z != 0)
                            {
                                if (chunkData[x, y, z - 1].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(FrontPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                } 
                            } else // z is 0
                            {
                                Chunk neighbour = worldData.GetChunk(new Vector3(position.x, position.y, position.z - 1));
                                if (neighbour?.ChunkData[x, y, chunkData.GetLength(2) - 1].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(FrontPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }
                            }

                            if (z != chunkData.GetLength(2) - 1)
                            {
                                if (chunkData[x, y, z + 1].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(BackPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }
                            }
                            else // z is equal to chunk length
                            {
                                Chunk neighbour = worldData.GetChunk(new Vector3(position.x, position.y, position.z + 1));
                                if (neighbour?.ChunkData[x, y, 0].type == VoxelType.AIR)
                                {
                                    vertices.AddRange(BackPolygon(offset));
                                    uvs.AddRange(GetUVMap(new Vector2(3f, 19f)));
                                    numberOfFaces++;
                                }
                            }
                        }
                    }
                }
            }

            meshData.Vertices = vertices;
            meshData.Triangles = GetTriangles(numberOfFaces);
            meshData.Uvs = uvs;

            return meshData;
        }

        public Vector3[] GetPolygons(Vector3 offset)
        {
            Vector3[] vertices = new Vector3[(verticesPerPolygon * sidesPerCube)];

            for (int i = 0; i < sidesPerCube; i++)
            {
                Vector3[] thisPolyGon = GetPolygon(i, offset);
                for (int j = 0; j < verticesPerPolygon; j++)
                    vertices[verticesPerPolygon * i + j] = thisPolyGon[j];
            }

            return vertices;
        }

        public Vector3[] GetPolygon(int i, Vector3 offset)
        {
            int[] polygonIndices = polygonToVertex[i];
            Vector3[] vertices = new Vector3[4];
            int vertextIndex = 0;
            foreach (var index in polygonIndices)
            {
                vertices[vertextIndex] = vertexList[index] + offset;
                vertextIndex++;
            }
            return vertices;
        }

        public Vector3[] BottomPolygon(Vector3 offset)
        {
            return GetPolygon(bottomIndex, offset);
        }

        public Vector3[] LeftPolygon(Vector3 offset)
        {
            return GetPolygon(leftIndex, offset);
        }

        public Vector3[] FrontPolygon(Vector3 offset)
        {
            return GetPolygon(frontIndex, offset);
        }

        public Vector3[] BackPolygon(Vector3 offset)
        {
            return GetPolygon(backIndex, offset);
        }

        public Vector3[] RightPolygon(Vector3 offset)
        {
            return GetPolygon(rightIndex, offset);
        }

        public Vector3[] TopPolygon(Vector3 offset)
        {
            return GetPolygon(topIndex, offset);
        }

        private List<Vector3> GetVertices()
        {
            return new List<Vector3> {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
            };
        }

        private Vector2[] GetUVMap(Vector2 texturePos)
        {
            float textureSize = 1f / 20f;

            Vector2 _00_COORDINATES = new Vector2(texturePos.x * textureSize, texturePos.y * textureSize);
            Vector2 _10_COORDINATES = new Vector2((texturePos.x + 1) * textureSize, texturePos.y * textureSize);
            Vector2 _01_COORDINATES = new Vector2(texturePos.x * textureSize, (texturePos.y + 1) * textureSize);
            Vector2 _11_COORDINATES = new Vector2((texturePos.x + 1) * textureSize, (texturePos.y + 1) * textureSize);

            return new Vector2[] { _10_COORDINATES, _11_COORDINATES, _01_COORDINATES, _00_COORDINATES };
        }

        private List<int> GetTriangles(int faces)
        {
            List<int> triangles = new List<int>();

            for (int i = 0; i < faces; i++)
            {
                triangles.AddRange(new int[] {
                    1 + 4 * i, 2 + 4 * i, 3 + 4 * i,
                    3 + 4 * i, 0 + 4 * i, 1 + 4 * i,
                });

            }

            return triangles;
        }
    }
}
