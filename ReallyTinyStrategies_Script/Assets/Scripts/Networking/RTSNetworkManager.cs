using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>RTSNetworkManager</c> is a Mirror component script used to manage the general networking aspects of the multiplayer game.
/// </summary>
public class RTSNetworkManager : NetworkManager
{
    /// <summary>
    /// Instance variable <c>unitSpawnerPrefab</c> is a Unity <c>GameObject</c> object representing the unit spawner prefabricated.
    /// </summary>
    [SerializeField] private GameObject unitBasePrefab;

    /// <summary>
    /// Instance variable <c>gameOverHandlerPrefab</c> is a Mirror <c>GameOverHandler</c> component script representing the game end behaviour manager.
    /// </summary>
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    /// <summary>
    /// Instance variable <c>teamColors</c> is a list of Unity <c>TeamColorStructure</c> scriptable objects representing the different team color variations available for each player.
    /// </summary>
    [SerializeField] private List<TeamColorProfile> teamColors;
    
    /// <summary>
    /// Static variable <c>ClientOnConnected</c> is an action event declaration
    /// for client side functions triggered by a player connection event.
    /// </summary>
    public static event Action ClientOnConnected;
    
    /// <summary>
    /// Static variable <c>ClientOnDisconnected</c> is an action event declaration
    /// for client side functions triggered by a player disconnection event.
    /// </summary>
    public static event Action ClientOnDisconnected;

    /// <summary>
    /// Instance variable <c>isGameInProgress</c> represents the in progress status of the game.
    /// </summary>
    private bool _isGameInProgress = false;

    /// <summary>
    /// Instance variable <c>players</c> is a list of Mirror <c>RTSPlayer</c> component scripts representing the different players and their game managers in the game.
    /// </summary>
    public List<RTSPlayer> players { get; } = new List<RTSPlayer>();
    
    #region Server

    /// <summary>
    /// This function is called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection data from client.</param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!_isGameInProgress)
        {
            return;
        }
        
        conn.Disconnect();
    }

    /// <summary>
    /// This function is called on the server when a new client disconnects.
    /// <para>Unity calls this on the Server when a Client disconnects from the Server. Use an override to tell the NetworkManager what to do when a client disconnects from the server.</para>
    /// </summary>
    /// <param name="conn">Connection data from client.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        players.Remove(player);
        
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        players.Clear();

        _isGameInProgress = false;
    }

    /// <summary>
    /// This function is responsible for starting the game and load the appropriate scene for the players to play.
    /// </summary>
    public void StartGame()
    {
        if (players.Count < 2)
        {
            return;
        }

        _isGameInProgress = true;
        
        ServerChangeScene("Map_01");
    }

    /// <summary>
    /// This function is called on the server when a client adds a new player.
    /// The default implementation for this function creates a new player object from the playerPrefab.
    /// </summary>
    /// <param name="conn">Connection data from client.</param>
    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        
        players.Add(player);
        
        player.SetDisplayName($"Player {players.Count}");
        
        player.SetTeamColor(teamColors[(int)Random.Range(0f, teamColors.Count - 1)]);
        
        player.SetPartyOwner(players.Count == 1);
    }

    /// <summary>
    /// This server side function is called on when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">A string message representing the name of the new scene.</param>
    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (RTSPlayer player in players)
            {
                GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                
                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    /// <param name="conn">Connection data to the server.</param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        ClientOnConnected?.Invoke();
    }

    /// <summary>
    /// Called on the client when disconnect from a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    /// <param name="conn">Connection data to the server.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        ClientOnDisconnected?.Invoke();
    }

    /// <summary>
    /// Called on client from StopClient to reset the Authenticator
    /// <para>Client message handlers should be unregistered in this method.</para>
    /// </summary>
    public override void OnStopClient()
    {
        players.Clear();
    }

    #endregion
}
