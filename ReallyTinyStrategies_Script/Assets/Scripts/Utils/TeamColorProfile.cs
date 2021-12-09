using UnityEngine;

/// <summary>
/// Class <c>TeamColorProfile</c> is a scriptable object containing the profile of colors and textures for a team.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TeamColorProfile", order = 1)]
public class TeamColorProfile: ScriptableObject
{
    /// <summary>
    /// Instance variable <c>color</c> is a Unity <c>Color</c> component representing the team color.
    /// </summary>
    public Color color;

    /// <summary>
    /// Instance variable <c>buildingTextureIndex</c> represents the index value of a building texture from one instance of a <c>TeamTexturesList</c> scriptable object.
    /// </summary>
    public int buildingTextureIndex;

    /// <summary>
    /// Instance variable <c>unitTextureIndex</c> represents the index value of a unit texture from one instance of a <c>TeamTexturesList</c> scriptable object.
    /// </summary>
    public int unitTextureIndex;

    /// <summary>
    /// Instance variable <c>bannerTextureIndex</c> represents the index value of a banner texture from one instance of a <c>TeamTexturesList</c> scriptable object.
    /// </summary>
    public int bannerTextureIndex;
}
