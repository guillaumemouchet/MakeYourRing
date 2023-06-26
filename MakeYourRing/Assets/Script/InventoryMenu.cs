using UnityEngine;


/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

/** This is the credits for all used icons from flaticon in agreament with their license
 * Ring icon : designed by Freepik from Flaticon (https://www.flaticon.com/free-icon/ring-with-precious-stone_50372)
 * Bracelet icon : designed by Darius Dan from Flaticon (https://www.flaticon.com/free-icon/bracelet_1319854)
 * Bracelet_1 icon : designed by Ayub Irawan from Flaticon (https://www.flaticon.com/free-icon/bracelet_7966247)
 **/

/// <summary>
/// Used for naviation purpous only
/// </summary>
public class InventoryMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject ringMenu;
    [SerializeField]
    private GameObject braceletMenu;
    [SerializeField]
    private GameObject inventoryMenu;


    /// <summary>
    /// Open the Ring Panel
    /// </summary>
    public void OnRingClick()
    {
        ringMenu.SetActive(true);
    }
    /// <summary>
    /// Open the Braclet Panel
    /// </summary>
    public void OnBraceletClick()
    {
        braceletMenu.SetActive(true);
    }

    /// <summary>
    /// Close this Panel
    /// </summary>
    public void onCloseClick()
    {
        inventoryMenu.SetActive(false);
    }

}
