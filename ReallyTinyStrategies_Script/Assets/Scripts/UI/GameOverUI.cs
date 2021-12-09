using Mirror;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>GameOverUI</c> is a Unity component script used to manage the game over UI element.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>gameOverUIParent</c> is a Unity <c>GameObject</c> object representing the game over UI element.
    /// </summary>
    [SerializeField] private GameObject gameOverUIParent;
    
    /// <summary>
    /// Instance variable <c>winnerNameText</c> is a Unity <c>TMP_Text</c> component representing the text UI element aim at displaying the winner name.
    /// </summary>
    [SerializeField] private TMP_Text winnerNameText;
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    /// <summary>
    /// This function is called when a Scene or game ends or when the component script linked game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    /// <summary>
    /// This function is called on button click and responsible for making the player leaving the game.
    /// </summary>
    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    } 

    /// <summary>
    /// This function is responsible for displaying the game over UI and showing the player name winner.
    /// </summary>
    /// <param name="winner">A string message representing the player name winner.</param>
    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won!";
        
        gameOverUIParent.SetActive(true);
    }
}
