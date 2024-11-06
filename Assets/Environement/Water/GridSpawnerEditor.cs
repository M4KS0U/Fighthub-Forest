using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSpawner))]
public class GridSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Appelle la base pour dessiner l'inspecteur par défaut
        base.OnInspectorGUI();

        GridSpawner gridSpawner = (GridSpawner)target;

        if (GUILayout.Button("Spawn Grid"))
        {
            // Appelle la méthode SpawnGrid de GridSpawner
            gridSpawner.SpawnGrid();
        }

        if (GUILayout.Button("Destroy Children"))
        {
            // Appelle la méthode DestroyChildren de GridSpawner
            gridSpawner.DestroyChildren();
        }
    }
}
