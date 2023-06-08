using UnityEngine;

/** This is the credits for all used icons from flaticon in agreament with their license
 * Inventory icon : designed by Yogi Aprelliyanto from Flaticon (https://www.flaticon.com/free-icon/box_9853880)
 * Delete icon : designed by Kiranshastry from Flaticon (https://www.flaticon.com/free-icon/delete_1214428)
 * Settings icon : Part of the unicode E713 (https://uifabricicons.azurewebsites.net)
 **/

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