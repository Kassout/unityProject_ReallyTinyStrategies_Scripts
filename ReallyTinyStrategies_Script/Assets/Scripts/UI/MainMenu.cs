using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

/// <summary>
/// Class <c>MainMenu</c> is a Unity component script used to manage the main menu UI element.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>landingPagePanel</c> is a Unity <c>GameObject</c> representing the landing page panel UI element.
    /// </summary>
    [SerializeField] private GameObject landingPagePanel;

    /// <summary>
    /// Instance variable <c>useSteam</c> represents the steam usage status of the game.
    /// </summary>
    [SerializeField] private bool useSteam = false;
    
    /// <summary>
    /// Instance variable <c>lobbyCreated</c> is a Steamworks <c>Callback</c> class representing the action context structure of <c>LobbyCreated_t</c> event triggered.
    /// </summary>
    protected Callback<LobbyCreated_t> lobbyCreated;
    
    /// <summary>
    /// Instance variable <c>gameLobbyJoinRequested</c> is a Steamworks <c>Callback</c> class representing the action context structure of <c>GameLobbyJoinRequested_t</c> event triggered.
    /// </summary>
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    
    /// <summary>
    /// Instance variable <c>lobbyEntered</c> is a Steamworks <c>Callback</c> class representing the action context structure of <c>LobbyEnter_t</c> event triggered.
    /// </summary>
    protected Callback<LobbyEnter_t> lobbyEntered;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        if (!useSteam)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
        else
        {
            NetworkManager.singleton.GetComponent<KcpTransport>().enabled = true;
            NetworkManager.singleton.GetComponent<FizzySteamworks>().enabled = false;
        }
    }
    
    /// <summary>
    /// This function is responsible for creating and starting the host of a game lobby.
    /// </summary>
    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
        
        NetworkManager.singleton.StartHost();
    }

    /// <summary>
    /// Callback function of the lobby created event triggered by the steamworks API.
    /// </summary>
    /// <param name="callback">A steamworks <c>LobbyCreated_t</c> structure containing information provided to callbacks about what triggered an action.</param>
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }
        
        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress",
            SteamUser.GetSteamID().ToString());
    }

    /// <summary>
    /// Callback function of the game lobby join requested event triggered by the steamworks API.
    /// </summary>
    /// <param name="callback">A steamworks <c>GameLobbyJoinRequested_t</c> structure containing information provided to callbacks about what triggered an action.</param>
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// Callback function of the lobby enter event triggered by the steamworks API.
    /// </summary>
    /// <param name="callback">A steamworks <c>LobbyEnter_t</c> structure containing information provided to callbacks about what triggered an action.</param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        landingPagePanel.SetActive(false);
    }
}
