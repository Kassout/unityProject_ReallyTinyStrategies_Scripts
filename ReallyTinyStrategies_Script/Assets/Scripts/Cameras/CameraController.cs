using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>CameraController</c> is a Mirror component script used to manage the general main camera behaviour.
/// </summary>
public class CameraController : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>playerCameraTransform</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the game camera.
    /// </summary>
    [SerializeField] private Transform playerCameraTransform;

    /// <summary>
    /// Instance variable <c>speed</c> represents the speed value of the camera movement.
    /// </summary>
    [SerializeField] private float speed = 20.0f;

    /// <summary>
    /// Instance variable <c>screenBorderThickness</c> represents the border thickness value of the screen.
    /// </summary>
    [SerializeField] private float screenBorderThickness = 10.0f;
    
    /// <summary>
    /// Instance variable <c>screenXLimits</c> represents the x coordinate limit value of the screen.
    /// </summary>
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    
    /// <summary>
    /// Instance variable <c>screenZLimits</c> represents the z coordinate limit value of the screen.
    /// </summary>
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    /// <summary>
    /// Instance variable <c>previousInput</c> is a Unity <c>Vector2</c> component representing the keyboard input binding movement values.
    /// </summary>
    private Vector2 _previousInput;
    
    /// <summary>
    /// Instance variable <c>controls</c> is a Unity <c>Controls</c> input actions collection representing the different action input bindings of the game.
    /// </summary>
    private Controls _controls;
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time but only for objects the client has authority over..
    /// </summary>
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        _controls = new Controls();

        _controls.Player.MoveCamera.performed += SetPreviousInput;
        _controls.Player.MoveCamera.canceled += SetPreviousInput;
        
        _controls.Enable();
    }
    
    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();
    }

    /// <summary>
    /// This function is responsible for updating the camera position.
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 position = playerCameraTransform.position;

        if (_previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            position += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            position += new Vector3(_previousInput.x, 0f, _previousInput.y) * speed * Time.deltaTime;
        }

        position.x = Mathf.Clamp(position.x, screenXLimits.x, screenXLimits.y);
        position.z = Mathf.Clamp(position.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = position;
    }

    /// <summary>
    /// This function is responsible for assessing the previous input vector value from the associated action input bindings.
    /// </summary>
    /// <param name="ctx">An InputAction <c>CallbackContext</c> structure containing information provided to action callbacks about what triggered an action.</param>
    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        _previousInput = ctx.ReadValue<Vector2>();
    }
}
