using SFB;
using UnityEngine;
using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if WINDOWS_UWP
using Windows.Storage;
#endif

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

        public void OnCloseClick()
        {
            settingsMenu.SetActive(false);

        }

    /// <summary>
    /// Save the leader object in the Result folder
    /// </summary>
    //TODO add confirmation message
    public void OnSaveClick()
         {
            GameObject[] leaderObjects = GameObject.FindGameObjectsWithTag("leader");
            Debug.Log("onSaveClick, find all leader tag");

            foreach(GameObject leaderObject in leaderObjects) 
            { 
                #if UNITY_EDITOR
                    string localPath = "/Result/" + leaderObject.name + ".obj";
                #else
                    string localPath = "/Assets/Result/" + leaderObject.name + ".obj";
                #endif

                var globalPath = Application.dataPath + localPath;
                Debug.Log(globalPath);

                OBJExporter objExporter = new OBJExporter();
                objExporter.Export(globalPath, leaderObject); //C# function in objexporter
                Debug.Log("Save obj " + leaderObject.name);
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
            string[] objPaths = StandaloneFileBrowser.OpenFilePanel("Choose object", Application.dataPath + "/Assets", extensions, true);
        #else
            string[] objPaths = StandaloneFileBrowser.OpenFilePanel("Choose object", Application.dataPath + "/Assets/Assets/", extensions, true);
        #endif

        string objPath = "";
        //To not make the programme crash if nothing is selected
        if(objPaths.Length>0)
        {
            objPath = objPaths[0];

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
            }else
            {
                #if UNITY_EDITOR
                    folderPath = Application.dataPath + "/Resources/Ring/";
                #else
                    folderPath = Application.dataPath + "/Assets/Resources/Ring/";
                #endif
            }

            string localPath = "\\" + obj.name + ".obj";

            OBJExporter objExporter = new OBJExporter();

            //Save the object at the right place to use it easly after
            objExporter.Export(folderPath + localPath, obj); //C# function in objexporter
            Debug.Log(folderPath + localPath);


            //DEBUG TEST
            //Destroy(obj);

            ////Reimport all after normalisation of the export
            //Utility.createMetaDataFile(folderPath + localPath);

            //GameObject new_obj = objLoader.Load(folderPath + localPath);

            //Utility.addImportantComponent(new_obj);

        }
        
    }

    //TODO
    public void OnDisplayClick()
        {
            Debug.Log("onDisplayClick");

        }

    /// <summary>
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(320, 10, 100, 30), "Import"))
        {
           OnImportClick();

        }

        if (GUI.Button(new Rect(420, 10, 100, 30 ), "Save"))
        {
            OnSaveClick();

        }

    }
}
