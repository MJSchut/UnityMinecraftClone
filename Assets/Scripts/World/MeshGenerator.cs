using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class MeshGenerator
    {
        private List<Vector3> vertexList = new List<Vector3>();
        private int verticesPerPolygon = 4;
        private int sidesPerCube = 6;

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

        public Mesh GenerateCube(List<int> neighbours)
        {
            Mesh quad = new Mesh();

            if (neighbours == null){
                quad.vertices = GetPolygons(new List<int>());
            } else
            {
                quad.vertices = GetPolygons(neighbours);
            }
            
            quad.uv = GetUVMap();
            quad.triangles = GetTriangles();

            quad.RecalculateBounds();
            quad.RecalculateNormals();
            return quad;
        }

        public Vector3[] GetPolygons(List<int> neighbours)
        {
            Vector3[] vertices = new Vector3[(verticesPerPolygon * sidesPerCube) - neighbours.Count];

            for (int i = 0; i < sidesPerCube; i++)
            {
                Vector3[] thisPolyGon = GetPolygon(i);
                for (int j = 0; j < verticesPerPolygon; j++)
                    vertices[verticesPerPolygon * i + j] = thisPolyGon[j];
            }

            return vertices;
        }

        public Vector3[] GetPolygon(int i)
        {
            int[] polygonIndices = polygonToVertex[i];
            Vector3[] vertices = new Vector3[4];
            int vertextIndex = 0;
            foreach (var index in polygonIndices)
            {
                vertices[vertextIndex] = vertexList[index];
                vertextIndex++;
            }
            return vertices;
        }

        public Vector3[] BottomPolygon()
        {
            return GetPolygon(bottomIndex);
        }

        public Vector3[] LeftPolygon()
        {
            return GetPolygon(leftIndex);
        }

        public Vector3[] FrontPolygon()
        {
            return GetPolygon(frontIndex);
        }

        public Vector3[] BackPolygon()
        {
            return GetPolygon(backIndex);
        }

        public Vector3[] RightPolygon()
        {
            return GetPolygon(rightIndex);
        }

        public Vector3[] TopPolygon()
        {
            return GetPolygon(topIndex);
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

        private Vector2[] GetUVMap()
        {
            Vector2 _00_CORDINATES = new Vector2(0f, 0f);
            Vector2 _10_CORDINATES = new Vector2(1f, 0f);
            Vector2 _01_CORDINATES = new Vector2(0f, 1f);
            Vector2 _11_CORDINATES = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
                // Bottom
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
                // Left
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
                // Front
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
                // Back
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
                // Right
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
                // Top
                _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            };

            return uvs;
        }

        private int[] GetTriangles()
        {
            List<int> triangles = new List<int>();

            for (int i = 0; i < sidesPerCube; i++)
            {
                triangles.AddRange(new int[] {
                    3 + 4 * i, 1 + 4 * i, 0 + 4 * i,
                    3 + 4 * i, 2 + 4 * i, 1 + 4 * i});

            }

            return triangles.ToArray();
        }
    }
}
