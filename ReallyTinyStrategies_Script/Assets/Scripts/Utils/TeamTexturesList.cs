using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>TeamTexturesList</c> is a scriptable object containing different texture variations link to a single entities type (unit, building, banner).
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TeamTexturesList", order = 2)]
public class TeamTexturesList : ScriptableObject
{
    /// <summary>
    /// Instance variable <c>textures</c> is a Unity <c>Texture</c> component representing different texture variations.
    /// </summary>
    public List<Texture> textures;
}
