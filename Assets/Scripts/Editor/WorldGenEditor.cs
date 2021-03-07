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

            if (GUILayout.Button("Generate World"))
                worldGen.GenerateWorld();

            if (GUILayout.Button("Remove all meshes"))
                worldGen.DestroyAll();
        }
    }

}
