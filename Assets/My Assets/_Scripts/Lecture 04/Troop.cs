//Caleb Richardson, Interactive Scripting Lecture 04 Assignment, 9/11/2023

#region Summary
/// <summary>
/// A Unit is a individual GameObject within the Troop
/// A Troop is a group of Units
/// 
/// A scriptable object containing data related a troop this data includes:
/// Amount of units to spawn within a troop, could be a range or a constant number
/// A 5 color array palette the units could be
/// A 5 string array of names the units could be
/// Prefab for the actual gameObjects
/// </summary>
#endregion

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Create New Troop", fileName = "New Troop")]
public class Troop : ScriptableObject
{
    public TroopType troopType;
    [Range(1, 50)]
    public int amountToSpawn;
    [Range(1, 50)]
    public int minAmountToSpawn;
    public int maxAmountToSpawn;
    public bool randomAmountToSpawn;
    public Color[] unitColors = new Color[5];
    public string[] unitNames = new string[5];
    public GameObject unitBasePrefab;
}

[CustomEditor(typeof(Troop))]
public class TroopEditor : Editor
{
    #region SerializedProperties
    SerializedProperty amountToSpawn;
    SerializedProperty minAmountToSpawn;
    SerializedProperty maxAmountToSpawn;
    SerializedProperty randomAmountToSpawn;
    SerializedProperty unitColors;
    SerializedProperty unitNames;
    SerializedProperty unitBasePrefab;
    SerializedProperty troopType;
    #endregion

    private void OnEnable() 
    {
        troopType = serializedObject.FindProperty("troopType");

        amountToSpawn = serializedObject.FindProperty("amountToSpawn");
        maxAmountToSpawn = serializedObject.FindProperty("maxAmountToSpawn");
        minAmountToSpawn = serializedObject.FindProperty("minAmountToSpawn");
        randomAmountToSpawn = serializedObject.FindProperty("randomAmountToSpawn");

        unitColors = serializedObject.FindProperty("unitColors");
        unitNames = serializedObject.FindProperty("unitNames");

        unitBasePrefab = serializedObject.FindProperty("unitBasePrefab");
    }

    public override void OnInspectorGUI()
    {
        Troop troop = (Troop)target;
        serializedObject.Update();

        GUILayout.Label("Troop Type");
        EditorGUILayout.PropertyField(troopType);
        EditorGUILayout.Space();

        GUILayout.Label("Spawn Options", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Amount To Spawn", GUILayout.MaxWidth(150));
        EditorGUILayout.PropertyField(randomAmountToSpawn, GUIContent.none, GUILayout.MaxWidth(150));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if(troop.randomAmountToSpawn)
        {
            int minValue = minAmountToSpawn.intValue;
            int maxValue = maxAmountToSpawn.intValue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Minimum Value", GUILayout.MaxWidth(100));
            minAmountToSpawn.intValue = EditorGUILayout.IntSlider(GUIContent.none, minValue, 1, 50, GUILayout.MaxWidth(150));
            GUILayout.Label("Maximum Value", GUILayout.MaxWidth(100));
            maxAmountToSpawn.intValue = EditorGUILayout.IntSlider(GUIContent.none, maxValue, minValue, 50, GUILayout.MaxWidth(150));
            if(maxValue < minValue) maxAmountToSpawn.intValue = minAmountToSpawn.intValue;
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Unit Amount To Spawn", GUILayout.MaxWidth(150));
            amountToSpawn.intValue = EditorGUILayout.IntSlider(GUIContent.none, amountToSpawn.intValue, 1, 50);
        }


        EditorGUILayout.Space();
        GUILayout.Label("Color Palette", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(unitColors);

        EditorGUILayout.Space();
        GUILayout.Label("Name Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(unitNames);

        EditorGUILayout.Space();    
        EditorGUILayout.PropertyField(unitBasePrefab);
        serializedObject.ApplyModifiedProperties();
    }
}


