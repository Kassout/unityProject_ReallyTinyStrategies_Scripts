using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>Targeter</c> is a Mirror component script used to manager the targeter behaviour of the player units.
/// </summary>
public class Targeter : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>target</c> is a Mirror <c>Targetable</c> component script representing the targetable behaviour manager of the player unit target.
    /// </summary>
    private Targetable _target;

    /// <summary>
    /// Accessor of the target attribute.
    /// </summary>
    /// <returns>The <c>Targetable</c> component of the unit instance.</returns>
    public Targetable GetTarget()
    {
        return _target;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    #region Server

    /// <summary>
    /// This function is used by the server on a client to check for a valid target and then assign it's focus for the player unit.
    /// </summary>
    /// <param name="targetGameObject">A Unity <c>GameObject</c> object representing the unit to target.</param>
    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent(out Targetable target))
        {
            return;
        }

        this._target = target;
    }

    /// <summary>
    /// This function is responsible for clearing the target of the player unit.
    /// </summary>
    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    /// <summary>
    /// This server side function is responsible for clearing targets of all units for a player who lose.
    /// </summary>
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    #endregion
}
