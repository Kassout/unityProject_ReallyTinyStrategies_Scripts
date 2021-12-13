using UnityEditor;
using UnityEngine;

/// <summary>
/// Class <c>UnitDataEditor</c> is a Unity editor script used to change the <c>UnitData</c> component inspector display depending on enum values choice.
/// </summary>
[CustomEditor(typeof(UnitData)), CanEditMultipleObjects]
public class UnitDataEditor : Editor
{
    /// <summary>
    /// Serialized properties of <c>UnitData</c> attributes.
    /// </summary>
    public SerializedProperty
        resourceCostProp,
        maxHealthProp,
        agentTypeProp,
        speedProp,
        angularSpeedProp,
        accelerationProp,
        stoppingDistanceProp,
        rotationSpeedProp,
        chaseRangeProp,
        attackRangeProp,
        attackRateProp,
        damageToDealProp,
        unitTypeProp,
        projectilePrefabProp,
        destroyAfterSecondsProp,
        launchForceProp;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        resourceCostProp = serializedObject.FindProperty("resourceCost");
        maxHealthProp = serializedObject.FindProperty("maxHealth");
        agentTypeProp = serializedObject.FindProperty("agentType");
        speedProp = serializedObject.FindProperty("speed");
        angularSpeedProp = serializedObject.FindProperty("angularSpeed");
        accelerationProp = serializedObject.FindProperty("acceleration");
        stoppingDistanceProp = serializedObject.FindProperty("stoppingDistance");
        rotationSpeedProp = serializedObject.FindProperty("rotationSpeed");
        chaseRangeProp = serializedObject.FindProperty("chaseRange");
        attackRangeProp = serializedObject.FindProperty("attackRange");
        attackRateProp = serializedObject.FindProperty("attackRate");
        damageToDealProp = serializedObject.FindProperty("damageToDeal");
        unitTypeProp = serializedObject.FindProperty("unitType");
        projectilePrefabProp = serializedObject.FindProperty("projectilePrefab");
        destroyAfterSecondsProp = serializedObject.FindProperty("destroyAfterSeconds");
        launchForceProp = serializedObject.FindProperty("launchForce");
    } 
    
    /// <summary>
    /// This function is responsible to make a custom inspector display for the associated type of component.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(resourceCostProp, new GUIContent("resourceCost"));
        EditorGUILayout.PropertyField(maxHealthProp, new GUIContent("maxHealth"));
        EditorGUILayout.PropertyField(agentTypeProp, new GUIContent("agentType"));
        EditorGUILayout.PropertyField(speedProp, new GUIContent("speed"));
        EditorGUILayout.PropertyField(angularSpeedProp, new GUIContent("angularSpeed"));
        EditorGUILayout.PropertyField(accelerationProp, new GUIContent("acceleration"));
        EditorGUILayout.PropertyField(stoppingDistanceProp, new GUIContent("stoppingDistance"));
        EditorGUILayout.PropertyField(rotationSpeedProp, new GUIContent("rotationSpeed"));
        EditorGUILayout.PropertyField(chaseRangeProp, new GUIContent("chaseRange"));
        EditorGUILayout.PropertyField(attackRangeProp, new GUIContent("attackRange"));
        EditorGUILayout.PropertyField(attackRateProp, new GUIContent("attackRate"));
        EditorGUILayout.PropertyField(damageToDealProp, new GUIContent("damageToDeal"));
        
        EditorGUILayout.PropertyField(unitTypeProp);

        UnitData.UnitType ut = (UnitData.UnitType)unitTypeProp.enumValueIndex;

        switch (ut)
        {
            case UnitData.UnitType.Range:
                EditorGUILayout.PropertyField(projectilePrefabProp, new GUIContent("projectilePrefab"));
                EditorGUILayout.PropertyField(destroyAfterSecondsProp, new GUIContent("destroyAfterSeconds"));
                EditorGUILayout.PropertyField(launchForceProp, new GUIContent("launchForce"));
                break;
            
            case UnitData.UnitType.Melee:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
