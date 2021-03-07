using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.World
{
    public class MeshGenerator
    {
        public Mesh GenerateCube(int cubeSize = 1)
        {
            Mesh quad = new Mesh();
            List<Vector3> vertexList = GetVertices(cubeSize);
            quad.vertices = GetAllPolygons(vertexList);
            quad.uv = GetUVMap();
            quad.triangles = GetTriangles();

            quad.RecalculateBounds();
            quad.RecalculateNormals();
            return quad;
        }

        public Vector3[] GetAllPolygons(List<Vector3> vertexList)
        {
            return new Vector3[]{
                vertexList[0], vertexList[1], vertexList[2], vertexList[3],
                vertexList[0], vertexList[3], vertexList[7], vertexList[4],
                vertexList[4], vertexList[5], vertexList[1], vertexList[0],
                vertexList[6], vertexList[7], vertexList[3], vertexList[2],
                vertexList[5], vertexList[6], vertexList[2], vertexList[1],
                vertexList[7], vertexList[6], vertexList[5], vertexList[4]
            };
        }

        private List<Vector3> GetVertices(int cubeSize)
        {
            return new List<Vector3> {
                new Vector3(-cubeSize * .5f, -cubeSize * .5f, cubeSize * .5f),
                new Vector3(cubeSize * .5f, -cubeSize * .5f, cubeSize * .5f),
                new Vector3(cubeSize * .5f, -cubeSize * .5f, -cubeSize * .5f),
                new Vector3(-cubeSize * .5f, -cubeSize * .5f, -cubeSize * .5f),
                new Vector3(-cubeSize * .5f, cubeSize * .5f, cubeSize * .5f),
                new Vector3(cubeSize * .5f, cubeSize * .5f, cubeSize * .5f),
                new Vector3(cubeSize * .5f, cubeSize * .5f, -cubeSize * .5f),
                new Vector3(-cubeSize * .5f, cubeSize * .5f, -cubeSize * .5f)
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
            int[] triangles = new int[]
            {
                // Cube Bottom Side Triangles
                3, 1, 0,
                3, 2, 1,
                // Cube Left Side Triangles
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

                // Cube Front Side Triangles
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                // Cube Back Side Triangles
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                // Cube Rigth Side Triangles
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                // Cube Top Side Triangles
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
                };
            return triangles;
        }
    }
}
