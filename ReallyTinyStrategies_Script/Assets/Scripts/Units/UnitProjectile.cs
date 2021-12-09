using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>UnitProjectile</c> is a Mirror component script used to manage the unit projectile general behaviour.
/// </summary>
public class UnitProjectile : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>damageToDeal</c> represents the damage value dealt by the projectile on collision.
    /// </summary>
    [SerializeField] private int damageToDeal;
    
    /// <summary>
    /// Instance variable <c>destroyAfterSeconds</c> represents the lifespan value in seconds of the projectile.
    /// </summary>
    [SerializeField] private float destroyAfterSeconds = 5.0f;
    
    /// <summary>
    /// Instance variable <c>launchForce</c> represents the force magnitude of the projectile launch on spawn.
    /// </summary>
    [SerializeField] private float launchForce = 10.0f;
        
    /// <summary>
    /// Instance variable <c>rigidbody</c> is a Unity <c>RigidBody</c> component representing the rigidbody of the game object instance.
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.forward * launchForce;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    /// <summary>
    /// This method is called when another object enters a trigger collider attached to this object.
    /// </summary>
    /// <param name="other">A <c>Collider</c> Unity component representing the collider of the object that it collides with.</param>
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
            {
                return;
            }

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(damageToDeal);
            }
            
            DestroySelf();
        }
    }

    /// <summary>
    /// This server side function is responsible for destroying the current instance of the game object.
    /// </summary>
    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
