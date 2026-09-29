using UnityEditor;
using UnityEngine;

namespace MarketTycoon.EditorTools
{
    public class FloorGridGenerator : EditorWindow
    {
        private GameObject _floorPrefab;
        private int _width = 14;
        private int _length = 14;
        private float _tileSize = 1f;

        [MenuItem("Tools/Market Tycoon/Generate Floor Grid")]
        public static void ShowWindow()
        {
            GetWindow<FloorGridGenerator>("Floor Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Store Floor Grid Generator", EditorStyles.boldLabel);

            _floorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor Prefab", _floorPrefab, typeof(GameObject), false);
            _width = EditorGUILayout.IntField("Width (X)", _width);
            _length = EditorGUILayout.IntField("Length (Z)", _length);
            _tileSize = EditorGUILayout.FloatField("Tile Size", _tileSize);

            if (GUILayout.Button("Generate Floor"))
            {
                GenerateGrid();
            }
        }

        private void GenerateGrid()
        {
            if (_floorPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a floor prefab first!", "OK");
                return;
            }

            // Create or replace parent container
            GameObject root = GameObject.Find("Environment_Floor");
            if (root != null)
            {
                Undo.DestroyObjectImmediate(root);
            }

            root = new GameObject("Environment_Floor");
            Undo.RegisterCreatedObjectUndo(root, "Generate Floor Grid");

            // Center grid around (0, 0, 0)
            float startX = -(_width * _tileSize) / 2f + (_tileSize / 2f);
            float startZ = -(_length * _tileSize) / 2f + (_tileSize / 2f);

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                {
                    Vector3 position = new Vector3(startX + (x * _tileSize), 0f, startZ + (z * _tileSize));
                    GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(_floorPrefab, root.transform);
                    tile.transform.position = position;
                    tile.transform.rotation = Quaternion.identity;
                }
            }

            Selection.activeGameObject = root;
            Debug.Log($"[FloorGenerator] Successfully created {_width}x{_length} floor grid.");
        }
    }
}