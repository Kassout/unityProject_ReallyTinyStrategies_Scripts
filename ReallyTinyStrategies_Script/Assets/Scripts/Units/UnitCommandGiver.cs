using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>UnitCommandGiver</c> is a Unity component script used to manage invokable unit commands and their behaviours.
/// </summary>
public class UnitCommandGiver : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>unitSelectionHandler</c> is a Unity <c>UnitSelectionHandler</c> component script representing the player unit selection manager.
    /// </summary>
    [SerializeField] private UnitSelectionHandler unitSelectionHandler;
    
    /// <summary>
    /// Instance variable <c>layerMask</c> is a Unity <c>LayerMask</c> structure representing the layer to ignore for the process.
    /// </summary>
    [SerializeField] private LayerMask layerMask;

    /// <summary>
    /// Instance variable <c>mainCamera</c> is a Unity <c>Camera</c> component representing the main camera manager of the game.
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _mainCamera = Camera.main;

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
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent(out Targetable target))
        {
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }

    /// <summary>
    /// This function is responsible for invoking a move command from the server.
    /// </summary>
    /// <param name="point">A Unity <c>Vector3</c> component representing the hitting point position of the player pointer on the screen.</param>
    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
    
    /// <summary>
    /// This function is responsible for invoking a target command from the server.
    /// </summary>
    /// <param name="target">A Mirror <c>Targetable</c> component script representing the target game object of the current player unit.</param>
    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.selectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    /// <summary>
    /// This function is responsible for disabling the unit command giver on game over event trigger.
    /// </summary>
    /// <param name="winnerName">A string message representing the player name winner.</param>
    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
