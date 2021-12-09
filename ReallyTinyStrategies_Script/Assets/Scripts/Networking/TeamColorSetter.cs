using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>TeamColorSetter</c> is a Mirror component script used to manage the player team color settings.
/// </summary>
public class TeamColorSetter : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>colorRenderers</c> is a Unity <c>Renderer</c> component representing the model rendering manager.
    /// </summary>
    [SerializeField] private Renderer[] colorRenderers = Array.Empty<Renderer>();
    
    /// <summary>
    /// Instance variable <c>teamColor</c> is a Unity <c>Color</c> component representing the player team color.
    /// </summary>
    [SyncVar(hook = nameof(HandleTeamColorUpdate))]
    private Color _teamColor;

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        _teamColor = player.GetTeamColorStructure().color;
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is responsible for changing the rendering color of every models associated with player team entities (units, buildings, etc.).
    /// </summary>
    /// <param name="oldColor">A Unity <c>Color</c> component representing the current player team color.</param>
    /// <param name="newColor">A Unity <c>Color</c> component representing the new player team color.</param>
    private void HandleTeamColorUpdate(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    #endregion
}
