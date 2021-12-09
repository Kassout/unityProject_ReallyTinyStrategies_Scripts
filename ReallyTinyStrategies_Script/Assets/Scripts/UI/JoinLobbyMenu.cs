using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>JoinLobbyMenu</c> is a Unity component script used to manage the join lobby menu UI element.
/// </summary>
public class JoinLobbyMenu : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>landingPagePanel</c> is a Unity <c>GameObject</c> representing the landing page panel UI element.
    /// </summary>
    [SerializeField] private GameObject landingPagePanel;

    /// <summary>
    /// Instance variable <c>addressInput</c> is a Unity <c>TMP_InputField</c> component representing the address input UI element.
    /// </summary>
    [SerializeField] private TMP_InputField addressInput;

    /// <summary>
    /// Instance variable <c>joinButton</c> is a Unity <c>Button</c> component representing the join button UI element.
    /// </summary>
    [SerializeField] private Button joinButton;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled.
    /// </summary>
    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    /// <summary>
    /// This function is responsible for making a player joining the lobby on called.
    /// </summary>
    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    /// <summary>
    /// This function is responsible for updating the active state of the different UI elements of the panel on client connected event trigger.
    /// </summary>
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
    
    /// <summary>
    /// This function is responsible for updating the active state of the different UI elements of the panel on client disconnected event trigger.
    /// </summary>
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
