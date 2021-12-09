using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Class <c>BuildingButton</c> is a Unity component script used to manage the building button behaviour.
/// </summary>
public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// Instance variable <c>building</c> is a Mirror <c>Building</c> component script representing the building manager.
    /// </summary>
    [SerializeField] private Building building;
    
    /// <summary>
    /// Instance variable <c>iconImage</c> is a Unity <c>Image</c> component representing the UI icon of the building.
    /// </summary>
    [SerializeField] private Image iconImage;
    
    /// <summary>
    /// Instance variable <c>priceText</c> is a Unity <c>TMP_Text</c> component representing the UI text name of the building.
    /// </summary>
    [SerializeField] private TMP_Text priceText;
    
    /// <summary>
    /// Instance variable <c>layerMask</c> is a Unity <c>LayerMask</c> structure representing the layer to ignore for the process.
    /// </summary>
    [SerializeField] private LayerMask floorMask = new LayerMask();

    /// <summary>
    /// Instance variable <c>mainCamera</c> is a Unity <c>Camera</c> component representing the main scene camera.
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// Instance variable <c>buildingCollider</c> is a Unity <c>BoxCollider</c> component representing the collider of the building.
    /// </summary>
    private BoxCollider _buildingCollider;
    
    /// <summary>
    /// Instance variable <c>player</c> is a Unity <c>RTSPlayer</c> component representing the player general manager.
    /// </summary>
    private RTSPlayer _player;
    
    /// <summary>
    /// Instance variable <c>buildingPreviewInstance</c> is a Unity <c>GameObject</c> object representing the model preview of the building.
    /// </summary>
    private GameObject _buildingPreviewInstance;
    
    /// <summary>
    /// Instance variable <c>buildingRendererInstance</c> is a Unity <c>Renderer</c> component representing the model rendering of the preview building.
    /// </summary>
    private Renderer _buildingRendererInstance;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _mainCamera = Camera.main;

        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
        
        _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        _buildingCollider = building.GetComponent<BoxCollider>();
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (!_buildingPreviewInstance)
        {
            return; 
        }
        
        UpdateBuildingPreview();
    }
    
    /// <summary>
    /// <c>IPointerDownHandler</c> callback function responsible for triggering actions on mouse button down.
    /// </summary>
    /// <param name="eventData">A Unity <c>PointerEventData</c> event payload associated with pointer events.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (_player.GetResources() < building.GetPrice())
        {
            return;
        }

        _buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();
        
        _buildingPreviewInstance.SetActive(false);
    }

    /// <summary>
    /// <c>IPointerUpHandler</c> callback function responsible for triggering actions on mouse button up.
    /// </summary>
    /// <param name="eventData">A Unity <c>PointerEventData</c> event payload associated with pointer events.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buildingPreviewInstance == null)
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            _player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        
        Destroy(_buildingPreviewInstance);
    }

    /// <summary>
    /// This function is responsible for updating the building preview position regarding the pointer position on screen.
    /// </summary>
    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            return;
        }

        _buildingPreviewInstance.transform.position = hit.point;

        if (!_buildingPreviewInstance.activeSelf)
        {
            _buildingPreviewInstance.SetActive(true);
        }

        Color color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;
        
        _buildingRendererInstance.material.SetColor("_BaseColor", color);
    }
}
