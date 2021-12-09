using Mirror;
using UnityEngine;

/// <summary>
/// Class <c>Targetable</c> is a Mirror component script used to manager the targetable behaviour of the player units.
/// </summary>
public class Targetable : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>aimAtPoint</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the aim at point targeted by enemy targeter units.
    /// </summary>
    [SerializeField] private Transform aimAtPoint;

    /// <summary>
    /// This function is responsible for getting the <c>Transform</c> component of the targetable game object.
    /// </summary>
    /// <returns>The <c>Transform</c> component of the targetable unit instance.</returns>
    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
