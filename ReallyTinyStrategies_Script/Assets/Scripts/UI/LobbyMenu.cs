using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class <c>LobbyMenu</c> is a Unity component script used to manage the lobby menu UI element.
/// </summary>
public class LobbyMenu : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>lobbyUI</c> is a Unity <c>GameObject</c> representing the lobby UI element.
    /// </summary>
    [SerializeField] private GameObject lobbyUI;
    
    /// <summary>
    /// Instance variable <c>startGameButton</c> is a Unity <c>Button</c> component representing the start game button UI element.
    /// </summary>
    [SerializeField] private Button startGameButton;
    
    /// <summary>
    /// Instance variable <c>playerNameTexts</c> is a list of Unity <c>TMP_Text</c> components representing the different player name texts UI elements.
    /// </summary>
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    public int minimumPlayerToPlay;
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdate += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    /// <summary>
    /// This function is called when a Scene or game ends or when the component script linked game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdate -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    /// <summary>
    /// This function is responsible for updating the active state of the different UI elements of the UI panel on client connected event trigger.
    /// </summary>
    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player...";
        }

        startGameButton.interactable = players.Count >= minimumPlayerToPlay;
    }

    /// <summary>
    /// This function is responsible for hide/show the start game button on party owner state update event trigger.
    /// </summary>
    /// <param name="state">A boolean value representing the party owner state of the player.</param>
    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    /// <summary>
    /// This function is responsible for starting the game.
    /// </summary>
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    /// <summary>
    /// This function is responsible for leaving the lobby.
    /// </summary>
    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();    
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
