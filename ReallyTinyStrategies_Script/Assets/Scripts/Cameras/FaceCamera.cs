using UnityEngine;

/// <summary>
/// Class <c>FaceCamera</c> is a Unity component script used to manage game object facing the main camera.
/// </summary>
public class FaceCamera : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>mainCameraTransform</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the game scene main camera.
    /// </summary>
    private Transform _mainCameraTransform;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        if (Camera.main is not null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    /// <summary>
    /// This function is called after all Update functions have been called.
    /// </summary>
    private void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward, _mainCameraTransform.rotation * Vector3.up);
    }
}
