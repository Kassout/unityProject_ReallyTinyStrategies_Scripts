using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Class <c>UnitData</c> is a scriptable object containing the different information defining a unit behaviours.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitData", order = 2)]
public class UnitData : ScriptableObject
{
    /// <summary>
    /// Instance variable <c>resourceCost</c> represents the resource cost value of the unit for his generation at unit spawner.
    /// </summary>
    [Header("Main unit data")]
    public int resourceCost = 10;
    
    /// <summary>
    /// Instance variable <c>maxHealth</c> represents the maximum quantity of health own by the unit instance.
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// Enum variable <c>AgentType</c> representing the different nav mesh agent type available in the game.
    /// </summary>
    public enum AgentType
    {
        Humanoid = 0,
        Machine = 1
    }
    
    /// <summary>
    /// Instance variable of the <c>AgentType</c> enumeration, <c>agentType</c> represents the nav mesh gent type of the unit.
    /// </summary>
    [Header("Unit movement data")]
    public AgentType agentType; 
    
    /// <summary>
    /// Instance variable <c>speed</c> represents the speed value of the player unit.
    /// </summary>
    public float speed = 5.0f;

    /// <summary>
    /// Instance variable <c>angularSpeed</c> represents the angular speed value of the player unit.
    /// </summary>
    public float angularSpeed = 120.0f;

    /// <summary>
    /// Instance variable <c>acceleration</c> represents the acceleration value of the player unit.
    /// </summary>
    public float acceleration = 8.0f;

    /// <summary>
    /// Instance variable <c>stoppingDistance</c> represents the stopping distance value of the player unit.
    /// </summary>
    public float stoppingDistance = 1.75f;
    
    /// <summary>
    /// Instance variable <c>rotationSpeed</c> represents the rotation speed of the player unit.
    /// </summary>
    public float rotationSpeed = 20.0f;

    /// <summary>
    /// Instance variable <c>chaseRange</c> represents the maximum range value a unit will follow a target.
    /// </summary>
    public float chaseRange = 10f;
    
    /// <summary>
    /// Instance variable <c>attackRange</c> represents the maximum range value the player unit can fire projectile to.
    /// </summary>
    [Header("Unit attack data")]
    public float attackRange = 5.0f;
    
    /// <summary>
    /// Instance variable <c>attackRate</c> represents the rate value the player unit can fire projectile at.
    /// </summary>
    public float attackRate = 1.0f;
    
    /// <summary>
    /// Instance variable <c>damageToDeal</c> represents the damage value dealt by the projectile on collision.
    /// </summary>
    public int damageToDeal;

    /// <summary>
    /// Enum variable <c>UnitType</c> representing the different unit type available in the game.
    /// </summary>
    public enum UnitType
    {
        Melee,
        Range
    }

    /// <summary>
    /// Instance variable of the <c>UnitType</c> enumeration, <c>unitType</c> represents the type of the unit.
    /// </summary>
    [Header("Specific unit data")]
    public UnitType unitType;

    /// <summary>
    /// Instance variable <c>projectilePrefab</c> is a Unity <c>GameObject</c> object representing the projectile prefabricated game object aim at be fired by the player unit.
    /// </summary>
    public GameObject projectilePrefab;

    /// <summary>
    /// Instance variable <c>destroyAfterSeconds</c> represents the lifespan value in seconds of the projectile.
    /// </summary>
    public float destroyAfterSeconds = 5.0f;
    
    /// <summary>
    /// Instance variable <c>launchForce</c> represents the force magnitude of the projectile launch on spawn.
    /// </summary>
    public float launchForce = 10.0f;
}
