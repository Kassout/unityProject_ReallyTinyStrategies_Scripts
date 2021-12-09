using Mirror;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>ResourcesUI</c> is a Unity component script used to manage the player resources quantity UI element.
/// </summary>
public class ResourcesUI : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>resourcesText</c> is a Unity <c>TMP_Text</c> component representing the text UI element aim at displaying the resources quantity of the player.
    /// </summary>
    [SerializeField] private TMP_Text resourcesText = null;

    /// <summary>
    /// Instance variable <c>player</c> is a Unity <c>RTSPlayer</c> component representing the player general manager.
    /// </summary>
    private RTSPlayer _player;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        
        ClientHandleResourcesUpdated(_player.GetResources());
                
        _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    /// <summary>
    /// This function is called when a Scene or game ends or when the component script linked game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    /// <summary>
    /// This function is responsible for updating the resources UI text.
    /// </summary>
    /// <param name="resources">An integer value representing the resources quantity of the player to display.</param>
    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
