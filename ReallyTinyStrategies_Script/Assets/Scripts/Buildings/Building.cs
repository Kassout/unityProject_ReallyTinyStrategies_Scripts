using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>Building</c> is a Mirror component script used to manage the general player building object behaviour.
/// </summary>
[RequireComponent(typeof(Health))]
public class Building : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>buildingData</c> is a Unity <c>BuildingData</c> scriptable object containing the different building data.
    /// </summary>
    public BuildingData buildingData;

    /// <summary>
    /// Static variable <c>ServerOnBuildingSpawned</c> is an action event declaration
    /// for server side functions taking Mirror <c>Building</c> parameter input and triggered by a player building spawned event.
    /// </summary>
    public static event Action<Building> ServerOnBuildingSpawned;
    
    /// <summary>
    /// Static variable <c>ServerOnBuildingDestroy</c> is an action event declaration
    /// for server side functions taking Mirror <c>Building</c> parameter input and triggered by a player building destroy event.
    /// </summary>
    public static event Action<Building> ServerOnBuildingDestroy;

    /// <summary>
    /// Static variable <c>AuthorityOnBuildingSpawned</c> is an action event declaration
    /// for client side functions taking Mirror <c>Building</c> parameter input and triggered by a player building spawned event.
    /// </summary>
    public static event Action<Building> AuthorityOnBuildingSpawned;
    
    /// <summary>
    /// Static variable <c>AuthorityOnBuildingDestroy</c> is an action event declaration
    /// for client side functions taking Mirror <c>Building</c> parameter input and triggered by a player building destroy event.
    /// </summary>
    public static event Action<Building> AuthorityOnBuildingDestroy;

    /// <summary>
    /// Accessor of the buildingPreview attribute.
    /// </summary>
    /// <returns>The building preview <c>GameObject</c> of the building instance.</returns>
    public GameObject GetBuildingPreview()
    {
        return buildingData.buildingPreview;
    }

    /// <summary>
    /// Accessor of the icon attribute.
    /// </summary>
    /// <returns>The building icon <c>Sprite</c> component of the building instance.</returns>
    public Sprite GetIcon()
    {
        return buildingData.icon;
    }

    /// <summary>
    /// Accessor of the id attribute.
    /// </summary>
    /// <returns>The building id integer value of the building instance.</returns>
    public int GetId()
    {
        return buildingData.id;
    }

    /// <summary>
    /// Accessor of the price attribute.
    /// </summary>
    /// <returns>The building price integer value of the building instance.</returns>
    public int GetPrice()
    {
        return buildingData.price;
    }

    #region Server

    /// <summary>
    /// This function is invoked for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        ServerOnBuildingDestroy?.Invoke(this);
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time but only for objects the client has authority over..
    /// </summary>
    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
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
        
        AuthorityOnBuildingDestroy?.Invoke(this);
    }

    #endregion
}
