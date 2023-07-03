using SFB;
using UnityEngine;
using Dummiesman;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.UI;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

#if WINDOWS_UWP
using Windows.Storage;
#endif

/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

/** This is the credits for all used icons from flaticon in agreament with their license
 * Save icon : designed by Yogi Aprelliyanto from Flaticon (https://www.flaticon.com/free-icon/diskette_2874091)
 * Import icon : designed by Pixel perfect from Flaticon (https://www.flaticon.com/free-icon/import_3199068)
 * Display icon : designed by Freepik from Flaticon (https://www.flaticon.com/free-icon/mannequin_998774)
 **/

//TODO Toujours ajouter le dossier asset fournis dans le build, c'est le que le User devra venir stocker ses eléments (Ring, Bracelet, Result), l'autre asset en plus est juste pour le Debug
public class SettingsMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject settingsMenu;

    [SerializeField]
    private GameObject hand;

    [SerializeField]
    private GameObject confirmationMessage;

    public void OnCloseClick()
    {
        settingsMenu.SetActive(false);
    }

    /// <summary>
    /// Save the leader object in the Result folder
    /// </summary>
    public void OnSaveClick()
    {
        GameObject[] leaderObjects = GameObject.FindGameObjectsWithTag("leader");
        Debug.Log("onSaveClick, find all leader tag");
        bool saved = false;
        string localPath = "";
        foreach (GameObject leaderObject in leaderObjects)
        {
#if UNITY_EDITOR
            localPath = "/Result/";
#else
                    localPath = "/Assets/Result/";
#endif

            ////remove the follower the time of the save
            //leaderObject.GetComponent<MergeJewel>().follower.transform.SetParent(null, true);

            var globalPath = Application.dataPath + localPath + leaderObject.name + ".obj";

            GameObjectExporterToObj objExporter = new GameObjectExporterToObj();
            objExporter.Export(leaderObject, globalPath);

            ////re-add the follower
            //leaderObject.GetComponent<MergeJewel>().follower.transform.SetParent(leaderObject.transform, true);

            saved = true;

            Debug.Log("Save obj " + leaderObject.name);
        }

        //Add a positive of negative confirmation message if a leader was found
        confirmationMessage.SetActive(true);

        //Set the instructions in front of the player
        Transform cameratransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        confirmationMessage.transform.position = new Vector3(cameratransform.position.x, cameratransform.position.y, cameratransform.position.z) + cameratransform.forward * 0.4f - cameratransform.up * 0.1f;
        confirmationMessage.transform.rotation = new Quaternion(cameratransform.rotation.x, cameratransform.rotation.y, cameratransform.rotation.z, cameratransform.rotation.w);

        if (saved)
        {
            confirmationMessage.GetNamedChild("Title").GetComponent<Text>().text = "Saved GameObjects";
            confirmationMessage.GetNamedChild("Description").GetComponent<Text>().text = "Your Leader GameObjects have been saved in the following folder : ";
            confirmationMessage.GetNamedChild("Path").GetComponent<Text>().text = Application.dataPath + localPath;

        }
        else
        {
            confirmationMessage.GetNamedChild("Title").GetComponent<Text>().text = "Failed Saving GameObjects";
            confirmationMessage.GetNamedChild("Description").GetComponent<Text>().text = "No leader where found in the scene, you need to fuze at least two gameObject together to save them";
        }
    }


    /// <summary>
    /// Import an .obj as a Gameobject in the scene and save it in the Resources folder.
    /// Add a .meta file for his PNG and JPG files
    /// </summary>
    public void OnImportClick()
    {

        // Open file with filter to choose .obj to import
        var extensions = new[] {
             new ExtensionFilter("Object file", "obj"),
         };
#if UNITY_EDITOR
        string[] objPaths = StandaloneFileBrowser.OpenFilePanel("Choose object smaller than 10 000 ko", Application.dataPath + "/Assets", extensions, true);
#else
            string[] objPaths = StandaloneFileBrowser.OpenFilePanel("Choose object", Application.dataPath + "/Assets/Assets/", extensions, true);
#endif

        string objPath = "";
        //To not make the programme crash if nothing is selected
        if (objPaths.Length > 0)
        {
            objPath = objPaths[0];
            FileInfo fi = new FileInfo(objPath);

            //TODO add error message
            if (fi.Length <= 20000000) // 40 000 ko
            {
                OBJLoader objLoader = new OBJLoader();

                Utility.createMetaDataFile(objPath);

                //Load the new object and give him his Components
                GameObject obj = objLoader.Load(objPath);

                obj = Utility.addImportantComponent(obj);


                //Choose if it's a bracelet or a ring
#if UNITY_EDITOR
                string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Save as a Ring or Bracelet", Application.dataPath + "/Resources", true);
#else
                string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Save as a Ring or Bracelet", Application.dataPath + "/Assets/Resources", true);
#endif

                string folderPath = "";
                //To not make the programme crash if nothing is selected -> Default is Ring
                if (folderPaths.Length > 0)
                {
                    folderPath = folderPaths[0];
                }
                else
                {
#if UNITY_EDITOR
                    folderPath = Application.dataPath + "/Resources/Ring/";
#else
                    folderPath = Application.dataPath + "/Assets/Resources/Ring/";
#endif
                }

                string localPath = "\\" + obj.name + ".obj";

                GameObjectExporterToObj gameObjectExporterToObj = new GameObjectExporterToObj();
                gameObjectExporterToObj.Export(obj, folderPath + localPath);
            }
            else
            {
                Debug.Log("File is too big");
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("File too big", "File must be smaller than 20'000 ko", "ok");
#else
                confirmationMessage.SetActive(true);

                //Set the instructions in front of the player
                Transform cameratransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
                confirmationMessage.transform.position = new Vector3(cameratransform.position.x, cameratransform.position.y, cameratransform.position.z) + cameratransform.forward * 0.4f - cameratransform.up * 0.1f;
                confirmationMessage.transform.rotation = new Quaternion(cameratransform.rotation.x, cameratransform.rotation.y, cameratransform.rotation.z, cameratransform.rotation.w);


                confirmationMessage.GetNamedChild("Title").GetComponent<Text>().text = "Load GameObject failed";
                confirmationMessage.GetNamedChild("Description").GetComponent<Text>().text = "Your object file must be smaller than 20'000ko, try creating it in smaller parts ";
#endif
            }

        }
    }

    /// <summary>
    /// Display a hand to see the creation
    /// Add the last selected item near the finger
    /// The player still need to adjust the position the display it right
    /// </summary>
    public void OnDisplayClick()
    {
        hand.SetActive(true);
        try
        {
            GameObject tryObject = GameObject.FindWithTag("handMenu").GetComponent<MainMenu>().lastItem;
            GameObject position = GameObject.Find("position");

            //Set the root item (leader) near the position of the finger
            tryObject.transform.root.localPosition = position.transform.position;
            tryObject.transform.root.localRotation = Quaternion.identity;

            tryObject.transform.root.transform.SetParent(position.transform);

            tryObject.GetComponent<MergeJewel>().enableFollower();
        }
        catch (Exception e)// No tryObject where found
        {
            Debug.Log(e);
            return;
        }

    }


#if DEBUG_MODE

    /// <summary>
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(320, 10, 100, 30), "Import"))
        {
            OnImportClick();
        }

        if (GUI.Button(new Rect(420, 10, 100, 30), "Save"))
        {
            OnSaveClick();
        }
    }
#endif
}
