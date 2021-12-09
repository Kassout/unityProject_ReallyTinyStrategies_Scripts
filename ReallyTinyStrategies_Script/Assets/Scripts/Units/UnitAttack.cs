using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>UnitFiring</c> is a Mirror component script used to manage the player unit attack behaviour.
/// </summary>
public class UnitAttack : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>targeter</c> is a Mirror <c>Targeter</c> component script representing the targeter behaviour manager of the player unit.
    /// </summary>
    [SerializeField] private Targeter targeter;
    
    /// <summary>
    /// Instance variable <c>projectilePrefab</c> is a Unity <c>GameObject</c> object representing the projectile prefabricated game object aim at be fired by the player unit.
    /// </summary>
    [SerializeField] private GameObject projectilePrefab;
    
    /// <summary>
    /// Instance variable <c>projectileSpawnPoint</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the spawn point of the projectile fired by the player unit.
    /// </summary>
    [SerializeField] private Transform projectileSpawnPoint;
    
    /// <summary>
    /// Instance variable <c>fireRange</c> represents the maximum range value the player unit can fire projectile to.
    /// </summary>
    [SerializeField] private float fireRange = 5.0f;
    
    /// <summary>
    /// Instance variable <c>fireRate</c> represents the rate value the player unit can fire projectile at.
    /// </summary>
    [SerializeField] private float fireRate = 1.0f;
    
    /// <summary>
    /// Instance variable <c>rotationSpeed</c> represents the rotation speed of the player unit.
    /// </summary>
    [SerializeField] private float rotationSpeed = 20.0f;

    /// <summary>
    /// Instance variable <c>lastFireTime</c> represents the elapsed time value since the last projectile fired by the player unit.
    /// </summary>
    private float _lastFireTime;

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        
        if (!target)
        {
            return;
        }
        
        if (!CanFireAtTarget())
        {
            return;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + _lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);
            
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            
            _lastFireTime = Time.time;
        }
    }

    /// <summary>
    /// This server side function is responsible for evaluating if the unit can fire at target.
    /// </summary>
    /// <returns>A boolean value representing the fire ability status of the unit.</returns>
    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }
}