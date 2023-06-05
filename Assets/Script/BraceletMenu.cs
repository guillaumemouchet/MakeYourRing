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
    private GameObject settingsMenu;

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
        #if UNITY_EDITOR
            string localPath = "/Resources/Bracelet/";
        #else
            string localPath = "/Assets/Resources/Bracelet/";
        #endif

        var globalPath = Application.dataPath + localPath;
        Debug.Log(globalPath);


        //Get all files with .obj
        var info = new DirectoryInfo(globalPath);
        var fileInfo = info.GetFiles("*.obj").ToList<FileInfo>();
        int i = 0;
        int j = 0;
        foreach (FileInfo obj in fileInfo)
        {
            string name = Path.GetFileNameWithoutExtension(obj.Name);
            Debug.Log("onEnable " + name);

            objFileList.Add(name); //To use them later

            //Retour à la ligne après 3
            if (i % 3 == 0 && i != 0)
            {
                j++;
            }

            // Changer the position of the new buttons
            Vector3 targetPosition = prefabButton.transform.position;
            targetPosition.x += 0.04f * (i - 3 * j);
            targetPosition.y -= 0.04f * j;

            //Create the new button from a prefab
            GameObject new_btn_prefab = Instantiate(prefabButton, targetPosition, Quaternion.identity, parent.transform);
            new_btn_prefab.GetComponent<ButtonConfigHelper>().MainLabelText = name;
            new_btn_prefab.SetActive(true);

            buttonList.Add(new_btn_prefab); //Used to destroy them later


            //Create a listener to have an action linked to the right file

            Debug.Log("Add Listener on " + i);
            int tempI = i;
            new_btn_prefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { onItemClick(tempI); }); // change to temp value to not have the reference but the value only
            i++;

        }

    }


    public void OnButtonClick()
    {
        Debug.Log("Debug purpous");
    }


    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    public void onItemClick(int i)
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

        settingsMenu.GetComponent<SettingsMenu>().addImportantComponent(obj); //Add the important Components

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
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(220, 110, 100, 30), "item click 0"))
        {
            onItemClick(0);

        }



    }
}
