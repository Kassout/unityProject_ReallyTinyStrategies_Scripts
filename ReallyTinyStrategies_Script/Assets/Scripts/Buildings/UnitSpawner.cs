using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>UnitSpawner</c> is a Mirror component script used to manage the unit spawning behaviour of the game.
/// </summary>
[RequireComponent(typeof(Transform), typeof(Building))]
public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Instance variable <c>health</c> is a Mirror <c>Health</c> component representing the health manager of the unit spawner building.
    /// </summary>
    private Health _health;
    
    /// <summary>
    /// Instance variable <c>unitPrefab</c> is a Unity <c>GameObject</c> component representing the unit prefabricated object to be instantiated by the unit spawner.
    /// </summary>
    private Unit _unitPrefab;
    
    /// <summary>
    /// Instance variable <c>unitSpawnPoint</c> is a Unity <c>Transform</c> component representing the spawn position of the unit to instantiate.
    /// </summary>
    [SerializeField] private Transform unitSpawnPoint;

    /// <summary>
    /// Instance variable <c>remainingUnitsText</c> is a Unity <c>TMP_Text</c> component representing the UI text of remaining units.
    /// </summary>
    [SerializeField] private TMP_Text remainingUnitsText;
    
    /// <summary>
    /// Instance variable <c>unitProgressImage</c> is a Unity <c>Image</c> component representing the UI image of unit progression.
    /// </summary>
    [SerializeField] private Image unitProgressImage;
    
    /// <summary>
    /// Instance variable <c>maxUnitQueue</c> represents the maximum size value of unit to be stored in the unit spawner queue.
    /// </summary>
    private int _maxUnitQueue;
    
    /// <summary>
    /// Instance variable <c>spawnMoveRange</c> represents the range value of unit movements around the spawn point of the unit spawner.
    /// </summary>
    private float _spawnMoveRange;
    
    /// <summary>
    /// Instance variable <c>unitSpawnDuration</c> represents the duration value of unit to spawn on unit spawner.
    /// </summary>
    private float _unitSpawnDuration;

    /// <summary>
    /// Instance variable <c>queuedUnits</c> represents the number of units in the queue of the unit spawner.
    /// </summary>
    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int _queuedUnits;
    
    /// <summary>
    /// Instance variable <c>timer</c> represents the time measurement value since last unit spawn.
    /// </summary>
    [SyncVar]
    private float _unitTimer;

    /// <summary>
    /// Instance variable <c>progressImageVelocity</c> represents the progression velocity value of the unit spawner to update the progress image.
    /// </summary>
    private float _progressImageVelocity;

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _health = GetComponent<Health>();

        BuildingData buildingData = GetComponent<Building>().buildingData;

        _unitPrefab = buildingData.unitPrefab;
        _maxUnitQueue = buildingData.maxUnitQueue;
        _spawnMoveRange = buildingData.spawnMoveRange;
        _unitSpawnDuration = buildingData.unitSpawnDuration;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        _health.ServerOnDeath += ServerHandleDeath;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        _health.ServerOnDeath -= ServerHandleDeath;
    }

    /// <summary>
    /// This function is responsible for producing units in respect of the different conditions of time, resources and queue limits.
    /// </summary>
    [Server]
    private void ProduceUnits()
    {
        if (_queuedUnits == 0)
        {
            return;
        }

        _unitTimer += Time.deltaTime;
        
        if (_unitTimer < _unitSpawnDuration)
        {
            return;
        }
        
        GameObject unitInstance = Instantiate(_unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        _queuedUnits--;
        _unitTimer = 0.0f;
    }

    /// <summary>
    /// This function is responsible for destroying the instance of the unit spawner building.
    /// </summary>
    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    /// <summary>
    /// This function is responsible for triggering a unit spawning action.
    /// </summary>
    [Command] // Call this from a client to run this function on the server.
    private void CmdSpawnUnit()
    {
        if (_queuedUnits == _maxUnitQueue)
        {
            return;
        }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < _unitPrefab.GetResourceCost())
        {
            return;
        }

        _queuedUnits++;
        
        player.SetResources(player.GetResources() - _unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    /// <summary>
    /// This function is responsible for updating the timer progression UI of the unit spawner.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        float newProgress = _unitTimer / _unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress,
                ref _progressImageVelocity, 0.1f);
        }
    }

    /// <summary>
    /// <c>IPointerClickHandler</c> callback function responsible for triggering actions on click.
    /// </summary>
    /// <param name="eventData">A Unity <c>PointerEventData</c> event payload associated with pointer events.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }
        
        CmdSpawnUnit();
    }

    /// <summary>
    /// This function is responsible for invoking functions triggered by the queued units updated.
    /// </summary>
    /// <param name="oldUnits">An integer value representing the old number of units in the queue.</param>
    /// <param name="newUnits">An integer value representing the new number of units in the queue.</param>
    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }

    #endregion
}
