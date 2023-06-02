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


    public void OnRingClick()
    {
        ringMenu.SetActive(true);
    }

    public void OnBraceletClick()
    {
        braceletMenu.SetActive(true);

    }

    public void onCloseClick()
    {
        inventoryMenu.SetActive(false);
    }

}
