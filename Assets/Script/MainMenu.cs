using UnityEngine;


/// <summary>
/// The main Menu is on the handMenu the closing and the opening is already done
/// </summary>
public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject inventoryMenu;


    public GameObject lastItem;


    /// <summary>
    /// Open the inventory Panel
    /// </summary>
    public void OnInventoryClick()
    {
        inventoryMenu.SetActive(true);
    }

    /// <summary>
    /// Delete last selected item
    /// </summary>
    public void OnDeleteClick()
    {
        Debug.Log("OnDeleteClicked, find selected Item");
        Destroy(lastItem);
    }

    /// <summary>
    /// Open the settings Panel
    /// </summary>
    public void OnSettingsClick()
    {
        settingsMenu.SetActive(true);
    }
}
