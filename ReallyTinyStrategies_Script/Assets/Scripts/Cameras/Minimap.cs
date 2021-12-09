using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>Minimap</c> is a Unity component script used to manage the general mini map behaviour.
/// </summary>
public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    /// <summary>
    /// Instance variable <c>minimapRect</c> is a Unity <c>RectTransform</c> component representing the rectangle dimensions, position, rotation and scale of the game mini map.
    /// </summary>
    [SerializeField] private RectTransform minimapRect;
    
    /// <summary>
    /// Instance variable <c>mapScale</c> represents the scale size of the mini map.
    /// </summary>
    [SerializeField] private float mapScale = 20.0f;
    
    /// <summary>
    /// Instance variable <c>offset</c> represents the z coordinate offset value of the camera position.
    /// </summary>
    [SerializeField] private float offset = -6.0f;

    /// <summary>
    /// Instance variable <c>playerCameraTransform</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the game camera.
    /// </summary>
    private Transform _playerCameraTransform;

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (_playerCameraTransform != null)
        {
            return;
        }

        if (NetworkClient.connection.identity == null)
        {
            return;
        }

        _playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }

    /// <summary>
    /// This function is responsible for moving the camera.
    /// </summary>
    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePos, null, out Vector2 localPoint))
        {
            return;
        }

        Rect rect = minimapRect.rect;
        Vector2 lerp = new Vector2((localPoint.x - rect.x) / rect.width, 
            (localPoint.y - rect.y) / rect.height);

        Vector3 newCameraPos = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x),
            _playerCameraTransform.position.y, Mathf.Lerp(-mapScale, mapScale, lerp.y));

        _playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offset);
    }

    /// <summary>
    /// <c>IPointerDownHandler</c> callback function responsible for triggering actions on mouse button down.
    /// </summary>
    /// <param name="eventData">A Unity <c>PointerEventData</c> event payload associated with pointer events.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    /// <summary>
    /// <c>IPointerDownHandler</c> callback function responsible for triggering actions on pointer drag.
    /// </summary>
    /// <param name="eventData">A Unity <c>PointerEventData</c> event payload associated with pointer events.</param>
    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
