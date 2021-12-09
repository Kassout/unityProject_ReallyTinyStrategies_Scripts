using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>TeamColorSetter</c> is a Mirror component script used to manage the player team entity (Unit, building, etc.) textures settings.
/// </summary>
public class TeamTextureSetter : NetworkBehaviour
{
    /// <summary>
    /// Enum variable <c>TextureTarget</c> representing the different texture potential targets of texture mutation.
    /// </summary>
    private enum TextureTarget
    {
        Unit,
        Building,
        Banner
    };

    /// <summary>
    /// Instance variable of the <c>TextureTarget</c> enumeration, <c>textureTarget</c> represents the texture target of the texture setter.
    /// </summary>
    [SerializeField] private TextureTarget textureTarget = TextureTarget.Unit;
    
    /// <summary>
    /// Instance variable <c>teamTexturesList</c> is a Unity <c>TeamTexturesList</c> scriptable object representing the different texture variations aim to be set on the texture target entities (unit, building, banner).
    /// </summary>
    [SerializeField] private TeamTexturesList teamTexturesList;
    
    /// <summary>
    /// Instance variable <c>textureRenderers</c> is a Unity <c>Renderer</c> component representing the model rendering manager.
    /// </summary>
    [SerializeField] private Renderer[] textureRenderers = Array.Empty<Renderer>();
    
    /// <summary>
    /// Instance variable <c>teamTextureIndex</c> represents the index value of the texture from the textures list to map on the different renderers.
    /// </summary>
    [SyncVar(hook = nameof(HandleTeamTextureUpdate))]
    private int _teamTextureIndex;

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        switch (textureTarget)
        {
            case TextureTarget.Unit:
                _teamTextureIndex = player.GetTeamColorStructure().unitTextureIndex;
                break;
            case TextureTarget.Building:
                _teamTextureIndex = player.GetTeamColorStructure().buildingTextureIndex;
                break;
            case TextureTarget.Banner:
                _teamTextureIndex = player.GetTeamColorStructure().bannerTextureIndex;
                break;
        }
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is responsible for changing the rendering color of every models associated with player team entities (units, buildings, etc.).
    /// </summary>
    /// <param name="oldTextureIndex">An integer value representing the index of the current player team entities associated texture.</param>
    /// <param name="newTextureIndex">An integer value representing the index of the new player team entities associated texture.</param>
    private void HandleTeamTextureUpdate(int oldTextureIndex, int newTextureIndex)
    {
        foreach (Renderer renderer in textureRenderers)
        {
            renderer.material.SetTexture("_BaseMap", teamTexturesList.textures[newTextureIndex]);
        }
    }

    #endregion
}
