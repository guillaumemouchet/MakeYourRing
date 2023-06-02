using UnityEngine;


//TODO : Code like Bracelet Menu with some small modifications
public class RingMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject ringMenu;


    public void onCloseClick()
    {
        ringMenu.SetActive(false);
    }
}
