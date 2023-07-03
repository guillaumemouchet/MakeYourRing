using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

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
        Debug.Log("OnDeleteClicked, find last selected Item");
        Destroy(lastItem);

        //Detach all children, only destroy the item
        List<GameObject> listChildren = new List<GameObject>();
        lastItem.GetChildGameObjects(listChildren);

        foreach (GameObject child in listChildren)
        {
            child.transform.SetParent(null, true);
        }

    }

    /// <summary>
    /// Open the settings Panel
    /// </summary>
    public void OnSettingsClick()
    {
        settingsMenu.SetActive(true);
    }
}
