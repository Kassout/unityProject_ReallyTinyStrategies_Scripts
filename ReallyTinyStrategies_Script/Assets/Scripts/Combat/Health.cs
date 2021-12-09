using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>Health</c> is a Mirror component script used to manage the player units life.
/// </summary>
public class Health : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>maxHealth</c> represents the maximum quantity of health own by the unit instance.
    /// </summary>
    [SerializeField] private int maxHealth = 100;

    /// <summary>
    /// Instance variable <c>currentHealth</c> represents the current quantity of health own by the unit instance.
    /// </summary>
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int _currentHealth;

    /// <summary>
    /// Instance variable <c>ServerOnDie</c> is an action event declaration for server side functions triggered by a player unit dying event.
    /// </summary>
    public event Action ServerOnDeath;

    /// <summary>
    /// Instance variable <c>ClientOnHealthUpdated</c> is an action event declaration
    /// for client side functions taking two integers as parameter inputs and triggered by a player units or buildings health updated event.
    /// </summary>
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        _currentHealth = maxHealth;
        
        UnitBase.ServerOnPlayerLose += ServerHandlePlayerLose;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerLose -= ServerHandlePlayerLose;
    }

    /// <summary>
    /// This server side function is responsible for reduce health of every units and buildings to zero for a player who lose.
    /// </summary>
    /// <param name="connectionId">An integer value representing the player id.</param>
    [Server]
    private void ServerHandlePlayerLose(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId)
        {
            return;
        }
        
        DealDamage(_currentHealth);
    }

    /// <summary>
    /// This server side function is responsible for dealing damage to the unit instance.
    /// </summary>
    /// <param name="damageAmount">An integer value representing the quantity of damage to inflict to the unit instance.</param>
    [Server]
    public void DealDamage(int damageAmount)
    {
        if (_currentHealth == 0)
        {
            return;
        }

        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

        if (_currentHealth != 0)
        {
            return;
        }
        
        ServerOnDeath?.Invoke();
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is responsible for invoking functions triggered by health updated event.
    /// </summary>
    /// <param name="oldHealth">An integer value representing the last health value of the unit.</param>
    /// <param name="newHealth">An integer value representing the current health value of the unit.</param>
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
