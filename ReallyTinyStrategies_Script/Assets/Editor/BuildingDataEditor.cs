using UnityEditor;
using UnityEngine;

/// <summary>
/// Class <c>BuildingDataEditor</c> is a Unity editor script used to change the <c>BuildingData</c> component inspector display depending on enum values choice.
/// </summary>
[CustomEditor(typeof(BuildingData)), CanEditMultipleObjects]
public class BuildingDataEditor : Editor
{
    /// <summary>
    /// Serialized properties of <c>BuildingData</c> attributes.
    /// </summary>
    public SerializedProperty
        idProp,
        iconProp,
        buildingPreviewProp,
        priceProp,
        maxHealthProp,
        buildingTypeProp,
        resourcesPerIntervalProp,
        intervalProp,
        unitPrefabProp,
        maxUnitQueueProp,
        spawnMoveRangeProp,
        unitSpawnDurationProp;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        idProp = serializedObject.FindProperty("id");
        iconProp = serializedObject.FindProperty("icon");
        buildingPreviewProp = serializedObject.FindProperty("buildingPreview");
        priceProp = serializedObject.FindProperty("price");
        maxHealthProp = serializedObject.FindProperty("maxHealth");
        buildingTypeProp = serializedObject.FindProperty("buildingType");
        resourcesPerIntervalProp = serializedObject.FindProperty("resourcesPerInterval");
        intervalProp = serializedObject.FindProperty("interval");
        unitPrefabProp = serializedObject.FindProperty("unitPrefab");
        maxUnitQueueProp = serializedObject.FindProperty("maxUnitQueue");
        spawnMoveRangeProp = serializedObject.FindProperty("spawnMoveRange");
        unitSpawnDurationProp = serializedObject.FindProperty("unitSpawnDuration");
    }

    /// <summary>
    /// This function is responsible to make a custom inspector display for the associated type of component.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(idProp, new GUIContent("id"));
        EditorGUILayout.PropertyField(iconProp, new GUIContent("icon"));
        EditorGUILayout.PropertyField(buildingPreviewProp, new GUIContent("buildingPreview"));
        EditorGUILayout.PropertyField(priceProp, new GUIContent("price"));
        EditorGUILayout.PropertyField(maxHealthProp, new GUIContent("maxHealth"));
        
        EditorGUILayout.PropertyField(buildingTypeProp);

        BuildingData.BuildingType bt = (BuildingData.BuildingType)buildingTypeProp.enumValueIndex;

        switch (bt)
        {
            case BuildingData.BuildingType.ResourceGenerator:
                EditorGUILayout.PropertyField(resourcesPerIntervalProp, new GUIContent("resourcesPerInterval"));
                EditorGUILayout.PropertyField(intervalProp, new GUIContent("interval"));
                break;
            
            case BuildingData.BuildingType.UnitSpawner:
                EditorGUILayout.PropertyField(unitPrefabProp, new GUIContent("unitPrefab"));
                EditorGUILayout.PropertyField(maxUnitQueueProp, new GUIContent("maxUnitQueue"));
                EditorGUILayout.PropertyField(spawnMoveRangeProp, new GUIContent("spawnMoveRange"));
                EditorGUILayout.PropertyField(unitSpawnDurationProp, new GUIContent("unitSpawnDuration"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
