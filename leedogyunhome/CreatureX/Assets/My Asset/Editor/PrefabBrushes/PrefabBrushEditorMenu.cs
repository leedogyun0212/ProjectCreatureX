using UnityEditor.Tilemaps;
using UnityEditor;
using UnityEngine;

public class PrefabBrushEditorMenu : MonoBehaviour
{
    [MenuItem("Assets/Create/Brushes/Prefab Brush")]
    public static void CreatePrefabBrushAsset()
    {
        var brush = ScriptableObject.CreateInstance<PrefabBrush>();
        AssetDatabase.CreateAsset(brush, "Assets/New Prefab Brush.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = brush;
    }

    [MenuItem("Assets/Create/Brushes/Base Prefab Brush")]
    public static void CreateBaseBrush()
    {
        var brush = ScriptableObject.CreateInstance<BasePrefabBrush>();
        AssetDatabase.CreateAsset(brush, "Assets/New Base Prefab Brush.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = brush;
    }
}