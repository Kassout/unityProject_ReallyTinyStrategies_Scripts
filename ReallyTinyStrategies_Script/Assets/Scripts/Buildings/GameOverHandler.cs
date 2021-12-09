using System;
using System.Collections.Generic;
using Mirror;

/// <summary>
/// Class <c>GameOverHandler</c> is a Mirror component script used to manage the end of the game behaviour
/// </summary>
public class GameOverHandler : NetworkBehaviour
{
    /// <summary>
    /// Static variable <c>ServerOnGameOver</c> is an action event declaration
    /// for server side functions triggered by a game over event.
    /// </summary>
    public static event Action ServerOnGameOver;
    
    /// <summary>
    /// Instance variable <c>ClientOnGameOver</c> is an action event declaration
    /// for client side functions taking a string as parameter input and triggered by a game over event.
    /// </summary>
    public static event Action<string> ClientOnGameOver;
    
    /// <summary>
    /// Instance variable <c>bases</c> is a list of Mirror <c>UnitBase</c> component scripts representing the unit base building behaviour managers.
    /// </summary>
    private readonly List<UnitBase> _bases = new List<UnitBase>();

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDestroyed += ServerHandleBaseDestroyed;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDestroyed -= ServerHandleBaseDestroyed;
    }

    /// <summary>
    /// This function is responsible for adding a unit base to the list of game bases.
    /// </summary>
    /// <param name="unitBase">A Mirror <c>UnitBase</c> component script representing the unit base to add to the list of bases.</param>
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        _bases.Add(unitBase);
    }
    
    /// <summary>
    /// This function is responsible for removing a unit base from the list of game bases.
    /// </summary>
    /// <param name="unitBase">A Mirror <c>UnitBase</c> component script representing the unit base to remove from the list of bases.</param>
    [Server]
    private void ServerHandleBaseDestroyed(UnitBase unitBase)
    {
        _bases.Remove(unitBase);

        if (_bases.Count != 1)
        {
            return;
        }

        int playerId = _bases[0].connectionToClient.connectionId;
        
        RpcGameOver($"Player {playerId}");
        
        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is called by server using a remote procedure call to display to all the clients who the winner of the game is.
    /// </summary>
    /// <param name="winner">A string message representing the player name winner.</param>
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);    
    }

    #endregion
}
