using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class BraceletMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject braceletMenu;

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
        Utility.createButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet);

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
        braceletMenu.SetActive(false);

    }

    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    public void onItemClickDebug(int i)
    {
        Debug.Log("On itemClick" + i);

        #if UNITY_EDITOR
            string localPath = "/Resources/Bracelet/" + objFileList[i] + ".obj";
            var globalPath = Application.dataPath + localPath;
        #else
            string localPath = "/Assets/Resources/Bracelet/" + objFileList[i] + ".obj";
            var globalPath = Application.dataPath + localPath;
        #endif

        Debug.Log(globalPath);

        OBJLoader objLoader = new OBJLoader();
        //Load the file
        GameObject obj = objLoader.Load(globalPath);

        Utility.addImportantComponent(obj); //Add the important Components

    }


    /// <summary>
    /// This method adds quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(370, 40, 150, 40), "item click bracelet 0"))
        {
            onItemClickDebug(0);

        }

    }
}
