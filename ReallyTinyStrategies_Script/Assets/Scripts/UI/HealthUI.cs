using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>HealthUI</c> is a Unity component script used to manage the player units health UI element.
/// </summary>
[RequireComponent(typeof(Health))]
public class HealthUI : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>health</c> is a Mirror <c>Health</c> component representing the health manager of the player unit.
    /// </summary>
    private Health _health;
    
    /// <summary>
    /// Instance variable <c>healthBarParent</c> is a Unity <c>GameObject</c> object representing the health UI element.
    /// </summary>
    [SerializeField] private GameObject healthBarParent;
    
    /// <summary>
    /// Instance variable <c>healthBarUI</c> is a Unity <c>Image</c> component representing the health UI bar image element.
    /// </summary>
    [SerializeField] private Image healthBarUI;

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _health = GetComponent<Health>();
        
        _health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    /// <summary>
    /// This function is called when a Scene or game ends or when the component script linked game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        _health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    /// <summary>
    /// This function is called when the mouse enters the collider.
    /// </summary>
    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    /// <summary>
    /// This function is called when the mouse exits the collider.
    /// </summary>
    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    /// <summary>
    /// This function is responsible for updating the health UI element on unit health quantity updates.
    /// </summary>
    /// <param name="currentHealth">An integer value representing the current health quantity of the unit.</param>
    /// <param name="maxHealth">An integer value representing the max health quantity of the unit.</param>
    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarUI.fillAmount = (float) currentHealth / maxHealth;
    }
}
