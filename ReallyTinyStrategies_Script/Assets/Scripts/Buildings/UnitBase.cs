using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>UnitBase</c> is a Mirror component script used to manage the unit base behaviour of the game.
/// </summary>
public class UnitBase : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>health</c> is a Mirror <c>Health</c> component script representing the health manager of the unit base building.
    /// </summary>
    [SerializeField] private Health health;

    /// <summary>
    /// Static variable <c>ServerOnBaseSpawned</c> is an action event declaration
    /// for server side functions taking Mirror <c>UnitBase</c> parameter input and triggered by a unit base spawn event.
    /// </summary>TODO
    public static event Action<int> ServerOnPlayerLose;
    
    /// <summary>
    /// Static variable <c>ServerOnBaseSpawned</c> is an action event declaration
    /// for server side functions taking Mirror <c>UnitBase</c> parameter input and triggered by a unit base spawn event.
    /// </summary>
    public static event Action<UnitBase> ServerOnBaseSpawned;
    
    /// <summary>
    /// Static variable <c>ServerOnBaseDestroyed</c> is an action event declaration
    /// for server side functions taking Mirror <c>UnitBase</c> parameter input and triggered by a unit base destroy event.
    /// </summary>
    public static event Action<UnitBase> ServerOnBaseDestroyed;
    
    #region Server
    
    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        health.ServerOnDeath += ServerHandleDeath;
        
        ServerOnBaseSpawned?.Invoke(this);
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        ServerOnBaseDestroyed?.Invoke(this);
        
        health.ServerOnDeath -= ServerHandleDeath;
    }
    
    /// <summary>
    /// This function is responsible for destroying the instance of the unit base building.
    /// </summary>
    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerLose?.Invoke(connectionToClient.connectionId);
        
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
