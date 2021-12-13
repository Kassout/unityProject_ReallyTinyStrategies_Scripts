using Mirror;
using Mirror.Examples.Tanks;
using UnityEngine;

/// <summary>
/// Class <c>UnitFiring</c> is a Mirror component script used to manage the player unit attack behaviour.
/// </summary>
public class UnitAttack : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>targeter</c> is a Mirror <c>Targeter</c> component script representing the targeter behaviour manager of the player unit.
    /// </summary>
    private Targeter _targeter;
    
    /// <summary>
    /// Instance variable <c>projectilePrefab</c> is a Unity <c>GameObject</c> object representing the projectile prefabricated game object aim at be fired by the player unit.
    /// </summary>
    private GameObject _projectilePrefab;
    
    /// <summary>
    /// Instance variable <c>projectileSpawnPoint</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the spawn point of the projectile fired by the player unit.
    /// </summary>
    [SerializeField] private Transform projectileSpawnPoint;
    
    /// <summary>
    /// Instance variable <c>fireRange</c> represents the maximum range value the player unit can fire projectile to.
    /// </summary>
    private float _attackRange;
    
    /// <summary>
    /// Instance variable <c>fireRate</c> represents the rate value the player unit can fire projectile at.
    /// </summary>
    private float _attackRate;
    
    /// <summary>
    /// Instance variable <c>rotationSpeed</c> represents the rotation speed of the player unit.
    /// </summary>
    private float _rotationSpeed;

    /// <summary>
    /// Instance variable <c>lastFireTime</c> represents the elapsed time value since the last projectile fired by the player unit.
    /// </summary>
    private float _lastFireTime;

    private void Awake()
    {
        _targeter = GetComponent<Targeter>();

        UnitData unitData = GetComponent<Unit>().unitData;

        _projectilePrefab = unitData.projectilePrefab;
        _attackRange = unitData.attackRange;
        _attackRate = unitData.attackRate;
        _rotationSpeed = unitData.rotationSpeed;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    [ServerCallback]
    private void Update()
    {
        if (!GetComponent<Unit>().deadFlag)
        {
            Targetable target = _targeter.GetTarget();
        
            if (!target)
            {
                return;
            }
        
            if (!CanFireAtTarget())
            {
                return;
            }
        
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / _attackRate) + _lastFireTime)
            {
                GetComponent<Unit>().animator.CrossFade("Attack", 0.2f);
                _lastFireTime = Time.time;
            }
        }
    }

    public void TriggerAttack()
    {
        Quaternion projectileRotation = Quaternion.LookRotation(_targeter.GetTarget().GetAimAtPoint().position - projectileSpawnPoint.position);
            
        GameObject projectileInstance = Instantiate(_projectilePrefab, projectileSpawnPoint.position, projectileRotation);
        
        projectileInstance.GetComponent<UnitProjectile>().InitiliazeStats(GetComponent<Unit>().unitData);
        
        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }

    /// <summary>
    /// This server side function is responsible for evaluating if the unit can fire at target.
    /// </summary>
    /// <returns>A boolean value representing the fire ability status of the unit.</returns>
    [Server]
    private bool CanFireAtTarget()
    {
        return (_targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= _attackRange * _attackRange;
    }
}