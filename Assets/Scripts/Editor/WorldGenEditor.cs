using UnityEditor;
using UnityEngine;

using MinecraftClone.World;

namespace MinecraftClone.EditorScripts
{
    [CustomEditor(typeof(WorldGenerator))]
    public class CatEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WorldGenerator worldGen = target as WorldGenerator;
            if (GUILayout.Button("Generate Single mesh"))
                worldGen.GenerateSingleMesh();

            if (GUILayout.Button("Generate Single Chunk"))
                worldGen.GenerateChunk(new Vector3(0, 0, 0));

            if (GUILayout.Button("Generate Nine chunks"))
            {
                worldGen.Awake();
                for (int x = -1; x < 2; x++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        worldGen.GenerateChunk(new Vector3(x, 0, z));
                    }
                }
                
            }

            if (GUILayout.Button("Remove all meshes"))
                worldGen.DestroyAll();

            
        }
    }

}
