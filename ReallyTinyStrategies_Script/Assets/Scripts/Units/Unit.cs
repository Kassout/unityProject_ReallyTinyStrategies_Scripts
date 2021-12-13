using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class <c>Unit</c> is a Mirror component script used to manage the general player unit object behaviour.
/// </summary>
[RequireComponent(typeof(Health))]
public class Unit : NetworkBehaviour
{
    public UnitData unitData;

    [HideInInspector] public Animator animator;
    
    /// <summary>
    /// Instance variable <c>health</c> is a Mirror <c>Health</c> component script representing the health manager of the player unit.
    /// </summary>
    private Health _health;
    
    /// <summary>
    /// Instance variable <c>unitMovement</c> is a Mirror <c>UnitMovement</c> component script representing the player unit movement manager.
    /// </summary>
    private UnitMovement _unitMovement;

    /// <summary>
    /// Instance variable <c>targeter</c> is a Mirror <c>Targeter</c> component script representing the player unit targeter behaviour manager.
    /// </summary>
    private Targeter _targeter;

    public bool deadFlag;

    #region Events

    /// <summary>
    /// Instance variable <c>onSelected</c> is a Unity event base executing actions based on unit selection trigger.
    /// </summary>
    [SerializeField] private UnityEvent onSelected;
    
    /// <summary>
    /// Instance variable <c>onDeselected</c> is a Unity event base executing actions based on unit deselection trigger.
    /// </summary>
    [SerializeField] private UnityEvent onDeselected;

    /// <summary>
    /// Static variable <c>ServerOnUnitSpawned</c> is an action event declaration
    /// for server side functions taking Mirror <c>Unit</c> parameter input and triggered by a player unit spawned event.
    /// </summary>
    public static event Action<Unit> ServerOnUnitSpawned;
    
    /// <summary>
    /// Static variable <c>ServerOnUnitDestroy</c> is an action event declaration
    /// for server side functions taking Mirror <c>Unit</c> parameter input and triggered by a player unit destroy event.
    /// </summary>
    public static event Action<Unit> ServerOnUnitDestroy;

    /// <summary>
    /// Static variable <c>AuthorityOnUnitSpawned</c> is an action event declaration
    /// for client side functions taking Mirror <c>Unit</c> parameter input and triggered by a player unit spawned event.
    /// </summary>
    public static event Action<Unit> AuthorityOnUnitSpawned;
    
    /// <summary>
    /// Static variable <c>AuthorityOnUnitDestroy</c> is an action event declaration
    /// for client side functions taking Mirror <c>Unit</c> parameter input and triggered by a player unit destroy event.
    /// </summary>
    public static event Action<Unit> AuthorityOnUnitDestroy;

    #endregion
    
    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _health = GetComponent<Health>();
        _targeter = GetComponent<Targeter>();
        _unitMovement = GetComponent<UnitMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Accessor of the resourceCost attribute.
    /// </summary>
    /// <returns>The integer value of the unit resource cost.</returns>
    public int GetResourceCost()
    {
        return unitData.resourceCost;
    }
    
    /// <summary>
    /// Accessor of the unitMovement attribute.
    /// </summary>
    /// <returns>The <c>UnitMovement</c> component of the unit instance.</returns>
    public UnitMovement GetUnitMovement()
    {
        return _unitMovement;
    }

    /// <summary>
    /// Accessor of the targeter attribute.
    /// </summary>
    /// <returns>The <c>Targeter</c> component of the unit instance.</returns>
    public Targeter GetTargeter()
    {
        return _targeter;
    }
    
    #region Server

    /// <summary>
    /// This function is invoked for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        
        _health.ServerOnDeath += ServerHandleDeath;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        _health.ServerOnDeath -= ServerHandleDeath;
        
        ServerOnUnitDestroy?.Invoke(this);
    }
    
    /// <summary>
    /// This function is responsible for destroying the instance of the player unit.
    /// </summary>
    [Server]
    private void ServerHandleDeath()
    {
        if (!deadFlag)
        {
            deadFlag = true;
            animator.CrossFade("Death", 0.2f);
        }
    }

    public void TriggerDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
    
    #endregion

    #region Client

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time but only for objects the client has authority over..
    /// </summary>
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    /// <summary>
    /// This function is called on every NetworkBehaviour when it is deactivated on a client.
    /// </summary>
    public override void OnStopClient()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        AuthorityOnUnitDestroy?.Invoke(this);
    }

    /// <summary>
    /// This function is responsible for selecting the player targeted unit.
    /// </summary>
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        onSelected?.Invoke();
    }

    /// <summary>
    /// This function is responsible for deselecting the current player targeted unit.
    /// </summary>
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        onDeselected?.Invoke();
    }

    #endregion
}
