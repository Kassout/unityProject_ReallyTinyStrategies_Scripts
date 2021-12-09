using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>RTSPlayer</c> is a Mirror component script used to manage the player networking aspects of the gameplay of the multiplayer game.
/// </summary>
public class RTSPlayer : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>cameraTransform</c> is a Unity <c>Transform</c> component representing the camera position, rotation and scale.
    /// </summary>
    [SerializeField] private Transform cameraTransform;
    
    /// <summary>
    /// Instance variable <c>buildingBlockLayer</c> is a Unity <c>LayerMask</c> structure representing the layer to ignore for the process of placing buildings.
    /// </summary>
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    
    /// <summary>
    /// Instance variable <c>buildings</c> is a list of Mirror <c>Building</c> component scripts representing the different buildings allow to be built by players on the game terrain.
    /// </summary>
    [SerializeField] private Building[] buildings = Array.Empty<Building>();

    /// <summary>
    /// Instance variable <c>buildingRangeLimit</c> represents the maximum range value of the player to place building.
    /// </summary>
    [SerializeField] private float buildingRangeLimit = 5.0f;
    
    /// <summary>
    /// Instance variable <c>resources</c> represents the resources value of the player.
    /// </summary>
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int _resources = 500;

    /// <summary>
    /// Instance variable <c>isPartyOwner</c> represents party owner status of the player.
    /// </summary>
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool _isPartyOwner = false;

    /// <summary>
    /// Instance variable <c>displayName</c> represents the name of the player.
    /// </summary>
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] 
    private string _displayName;

    /// <summary>
    /// Instance variable <c>ClientOnResourcesUpdated</c> is an action event declaration
    /// for client side functions taking one integer as parameter input and triggered by a building resources updated.
    /// </summary>
    public event Action<int> ClientOnResourcesUpdated;

    /// <summary>
    /// Static variable <c>ClientOnInfoUpdated</c> is an action event declaration
    /// for client side functions triggered by a player info updated event.
    /// </summary>
    public static event Action ClientOnInfoUpdated;
    
    /// <summary>
    /// Static variable <c>AuthorityOnPartyOwnerStateUpdate</c> is an action event declaration
    /// for authority side functions taking a boolean input and triggered by a party owner state updated.
    /// </summary>
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdate;

    /// <summary>
    /// Instance variable <c>teamColorProfile</c> is a Unity <c>TeamColorProfile</c> scriptable object representing the team color profile of the player.
    /// </summary>
    private TeamColorProfile _teamColorProfile;
    
    /// <summary>
    /// Instance variable <c>myUnits</c> is a list of Mirror <c>Unit</c> component scripts representing the different units owned by the player.
    /// </summary>
    private readonly List<Unit> _myUnits = new List<Unit>();

    /// <summary>
    /// Instance variable <c>_myBuildings</c> is a list of Mirror <c>Building</c> component scripts representing the different buildings owned by the player.
    /// </summary>
    private readonly List<Building> _myBuildings = new List<Building>();

    /// <summary>
    /// Accessor of the display name.
    /// </summary>
    /// <returns>A string message representing the player's name.</returns>
    public string GetDisplayName()
    {
        return _displayName;
    }
    
    /// <summary>
    /// Accessor of the party owner state.
    /// </summary>
    /// <returns>A boolean value representing the party owner state of the player.</returns>
    public bool GetIsPartyOwner()
    {
        return _isPartyOwner;
    }

    /// <summary>
    /// Accessor of the camera transform.
    /// </summary>
    /// <returns>The Unity <c>Transform</c> component representing the camera position, rotation and scale.</returns>
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    /// <summary>
    /// Accessor of the team color structure.
    /// </summary>
    /// <returns>The Unity <c>TeamColorStructure</c> scriptable object representing the team color structure of the player.</returns>
    public TeamColorProfile GetTeamColorStructure()
    {
        return _teamColorProfile;
    }

    /// <summary>
    /// Accessor of the resources attribute.
    /// </summary>
    /// <returns>The resources quantity integer value of the player instance.</returns>
    public int GetResources()
    {
        return _resources;
    }

    /// <summary>
    /// Mutator of the display name.
    /// </summary>
    /// <param name="displayName">A string message representing the player's name.</param>
    [Server]
    public void SetDisplayName(string displayName)
    {
        _displayName = displayName;
    }
    
    /// <summary>
    /// Mutator of the team color structure.
    /// </summary>
    /// <param name="newTeamColorProfile">A Unity <c>TeamColorStructure</c> scriptable object representing the team color structure to associate the player with.</param>
    [Server]
    public void SetTeamColor(TeamColorProfile newTeamColorProfile)
    {
        _teamColorProfile = newTeamColorProfile;
    }
    
    /// <summary>
    /// Mutator of the resources attribute
    /// </summary>
    /// <param name="resources">An integer value representing the new resources quantity integer value of the player instance.</param>
    [Server]
    public void SetResources(int resources)
    {
        _resources = resources;
    }

    /// <summary>
    /// This function is responsible for getting the player list of units.
    /// </summary>
    /// <returns>A list of Mirror <c>Unit</c> component scripts representing the player list of units.</returns>
    public List<Unit> GetMyUnits()
    {
        return _myUnits;
    }

    /// <summary>
    /// This function is responsible for getting the player list of buildings.
    /// </summary>
    /// <returns>A list of Mirror <c>Building</c> component scripts representing the player list of buildings.</returns>
    public List<Building> GetMyBuildings()
    {
        return _myBuildings;
    }

    /// <summary>
    /// This function is responsible for checking the ability to place a building at a certain position.
    /// </summary>
    /// <param name="buildingCollider">A Unity <c>BoxCollider</c> component representing the collider of the building to place.</param>
    /// <param name="point">A Unity <c>Vector3</c> component representing the position to place the building at.</param>
    /// <returns>A boolean value representing the ability or not to place the building.</returns>
    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity,
            buildingBlockLayer))
        {
            return false;
        }

        foreach (Building building in _myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDestroy += ServerHandleUnitDestroy;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDestroy += ServerHandleBuildingDestroy;
        
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDestroy -= ServerHandleUnitDestroy;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDestroy -= ServerHandleBuildingDestroy;
    }

    /// <summary>
    /// Mutator of the party owner state attribute
    /// </summary>
    /// <param name="state">A boolean value representing the party owner status of the player.</param>
    [Server]
    public void SetPartyOwner(bool state)
    {
        _isPartyOwner = state;
    }

    /// <summary>
    /// This function is responsible for starting the game.
    /// </summary>
    [Command] // Call this from a client to run this function on the server.
    public void CmdStartGame()
    {
        if (!_isPartyOwner)
        {
            return;
        }
        
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }

    /// <summary>
    /// This function is responsible for placing building on the terrain if allowed.
    /// </summary>
    /// <param name="buildingId">An integer value representing the id of the building to build.</param>
    /// <param name="point">A Unity <c>Vector3</c> component representing the position to place the building at.</param>
    [Command] // Call this from a client to run this function on the server.
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null)
        {
            return;
        }

        if (_resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        
        if (!CanPlaceBuilding(buildingCollider, point))
        {
            return;
        }
        
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        
        SetResources(_resources - buildingToPlace.GetPrice());
    }

    /// <summary>
    /// Server side function responsible for adding an instantiated unit in the player list of units.
    /// </summary>
    /// <param name="unit">A Mirror <c>Unit</c> component script representing the instantiated unit.</param>
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        
        _myUnits.Add(unit);
    }

    /// <summary>
    /// Server side function responsible for removing a given unit from the player list of units.
    /// </summary>
    /// <param name="unit">A Mirror <c>Unit</c> component script representing the destroyed unit.</param>
    private void ServerHandleUnitDestroy(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        
        _myUnits.Remove(unit);
    }

    /// <summary>
    /// Server side function responsible for adding an instantiated building in the player list of buildings.
    /// </summary>
    /// <param name="building">A Mirror <c>Building</c> component script representing the instantiated building.</param>
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        
        _myBuildings.Add(building);
    }
    
    /// <summary>
    /// Server side function responsible for removing a given building from the player list of buildings.
    /// </summary>
    /// <param name="building">A Mirror <c>Building</c> component script representing the destroyed building.</param>
    private void ServerHandleBuildingDestroy(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        
        _myBuildings.Remove(building);
    }

    #endregion

    #region Client
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time but only for objects the client has authority over..
    /// </summary>
    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
        {
            return;
        }
        
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDestroy += AuthorityHandleUnitDestroy;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDestroy += AuthorityHandleBuildingDestroy;
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        if (NetworkServer.active)
        {
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        ((RTSNetworkManager)NetworkManager.singleton).players.Add(this);
    }

    /// <summary>
    /// This function is called on every NetworkBehaviour when it is deactivated on a client.
    /// </summary>
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        
        if (!isClientOnly)
        {
            return;
        }
        
        ((RTSNetworkManager)NetworkManager.singleton).players.Remove(this);

        if (!hasAuthority)
        {
            return;
        }
        
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDestroy -= AuthorityHandleUnitDestroy;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDestroy -= AuthorityHandleBuildingDestroy;
    }

    /// <summary>
    /// This function is responsible for invoking functions triggered by a player party owner state updated event.
    /// </summary>
    /// <param name="oldState">A boolean value representing the old party owner state of the player.</param>
    /// <param name="newState">A boolean value representing the new party owner state of the player.</param>
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
        {
            return;
        }

        AuthorityOnPartyOwnerStateUpdate?.Invoke(newState);
    }

    /// <summary>
    /// Client side function responsible for adding an instantiated unit in the player list of units.
    /// </summary>
    /// <param name="unit">A Mirror <c>Unit</c> component script representing the instantiated unit.</param>
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        _myUnits.Add(unit);
    }

    /// <summary>
    /// Client side function responsible for removing a given unit from the player list of units.
    /// </summary>
    /// <param name="unit">A Mirror <c>Unit</c> component script representing the destroyed unit.</param>
    private void AuthorityHandleUnitDestroy(Unit unit)
    {
        _myUnits.Remove(unit);
    }
    
    /// <summary>
    /// Client side function responsible for adding an instantiated building in the player list of buildings.
    /// </summary>
    /// <param name="building">A Mirror <c>Building</c> component script representing the instantiated building.</param>
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        _myBuildings.Add(building);
    }
    
    /// <summary>
    /// Client side function responsible for removing a given building from the player list of buildings.
    /// </summary>
    /// <param name="building">A Mirror <c>Building</c> component script representing the destroyed building.</param>
    private void AuthorityHandleBuildingDestroy(Building building)
    {
        _myBuildings.Remove(building);
    }

    /// <summary>
    /// This function is responsible for invoking function triggered by a resources updated event.
    /// </summary>
    /// <param name="oldResources">An integer value representing the current resources quantity of the player.</param>
    /// <param name="newResources">An integer value representing the new resources quantity of the player.</param>
    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    /// <summary>
    /// This function is responsible for invoking function triggered by a player display name updated event.
    /// </summary>
    /// <param name="oldDisplayName">A string message representing the old player name.</param>
    /// <param name="newDisplayName">A string message representing the new player name.</param>
    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
