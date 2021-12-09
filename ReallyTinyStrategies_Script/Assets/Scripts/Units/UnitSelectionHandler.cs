using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>UnitSelectionHandler</c> is a Unity component script used to manage the unit selection behaviour.
/// </summary>
public class UnitSelectionHandler : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>layerMask</c> is a Unity <c>LayerMask</c> structure representing the layer to ignore for the process.
    /// </summary>
    [SerializeField] private LayerMask layerMask;

    /// <summary>
    /// Instance variable <c>unitSelectionArea</c> is a Unity <c>RectTransform</c> component representing the rectangular transform of the unit selection area object.
    /// </summary>
    [SerializeField] private RectTransform unitSelectionArea;
    
    /// <summary>
    /// Instance variable <c>startPosition</c> is a Unity <c>Vector2</c> component representing the starting position value of the mouse pointer on dragging the unit selection area.
    /// </summary>
    private Vector2 _startPosition;
    
    /// <summary>
    /// Instance variable <c>player</c> is a Unity <c>RTSPlayer</c> component script representing the manager of the networking aspects of the player behaviour.
    /// </summary>
    private RTSPlayer _player;
    
    /// <summary>
    /// Instance variable <c>mainCamera</c> is a Unity <c>Camera</c> component representing the main camera manager of the game.
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// Instance variable <c>selectedUnits</c> is a list of Mirror <c>Unit</c> component scripts representing the selected units of the player.
    /// </summary>
    public List<Unit> selectedUnits { get; } = new List<Unit>();

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _mainCamera = Camera.main;
        
        _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        Unit.AuthorityOnUnitDestroy += AuthorityHandleUnitDestroy;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    /// <summary>
    /// This function is called when a Scene or game ends or when the component script linked game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDestroy -= AuthorityHandleUnitDestroy;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        } 
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    /// <summary>
    /// This function is responsible for initializing the unit selection area drawn by the player and reset it if key isn't pressed anymore.
    /// </summary>
    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            selectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);

        _startPosition = Mouse.current.position.ReadValue();
        
        UpdateSelectionArea();
    }

    /// <summary>
    /// This function is responsible for updating the unit selection area as drawn by the player.
    /// </summary>
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    /// <summary>
    /// This function is responsible for clearing the unit selection area drawn by the player and add all units inside into the <c>selectedUnits</c> list.
    /// </summary>
    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            if (!hit.collider.TryGetComponent(out Unit unit))
            {
                return;
            }

            if (!unit.hasAuthority)
            {
                return;
            }
        
            selectedUnits.Add(unit);

            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.GetMyUnits())
        {
            if (selectedUnits.Contains(unit))
            {
                continue;
            }
            
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && 
                screenPosition.y > min.y && screenPosition.y < max.y)
            {
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
    }
    
    /// <summary>
    /// This function is responsible for 
    /// </summary>
    /// <param name="unit"></param>
    private void AuthorityHandleUnitDestroy(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

    /// <summary>
    /// This function is responsible for disabling the unit selection component on game over event trigger.
    /// </summary>
    /// <param name="winnerName">A string message representing the player name winner.</param>
    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
