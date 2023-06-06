using UnityEngine;

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
