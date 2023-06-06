using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


//TODO : Code like Bracelet Menu with some small modifications
public class RingMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject ringMenu;

    [SerializeField]
    private GameObject prefabButton;

    [SerializeField]
    private GameObject parent;

    public List<string> objFileList = new List<string>();

    private List<GameObject> buttonList = new List<GameObject>();

    void OnDisable()
    {
        objFileList.Clear();
    }

    void OnEnable()
    {
        ////Create a button for each .obj in our resource folder
        Utility.createButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Ring);       

    }

    /// <summary>
    /// On the close of the panel we want to destroy the button and create new one each time
    /// </summary>
    public void onCloseClick()
    {
        foreach (GameObject btn in buttonList)
        {
            Debug.Log("DESTOY");
            Destroy(btn);
        }
        ringMenu.SetActive(false);


    }

    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    public void onItemClickDebug(int i)
    {
        Debug.Log("On itemClick" + i);

        #if UNITY_EDITOR
                string localPath = "/Resources/Ring/" + objFileList[i] + ".obj";
        #else
                string localPath = "/Assets/Resources/Ring/" + objFileList[i] + ".obj";
        #endif
        var globalPath = Application.dataPath + localPath;

        Debug.Log(globalPath);

        OBJLoader objLoader = new OBJLoader();
        //Load the file
        GameObject obj = objLoader.Load(globalPath);

        Utility.addImportantComponent(obj); //Add the important Components
    }

    /// <summary>
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(330, 110, 150, 40), "item click ring 0"))
        {
            onItemClickDebug(0);
        }
    }
}
