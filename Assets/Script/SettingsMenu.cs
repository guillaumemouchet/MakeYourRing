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

            //TODO Change how we choose what to save
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

            //Creer à la volée des .meta pour tous le JPG ou png qu'on trouve
            string directoryFolder = Path.GetDirectoryName(objPath);

            //On fait que les .png et .jpg pour l'instant
            IEnumerable<string> files = Directory.EnumerateFiles(directoryFolder, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .Where(file => !file.EndsWith(".obj", StringComparison.OrdinalIgnoreCase) && !file.EndsWith(".mtl", StringComparison.OrdinalIgnoreCase));

            //Create a meta file foreach png or jpg
            foreach (string file in files)
            {
                Debug.Log("nom du fichier " + file);

                //Create a new meta file
                #if UNITY_EDITOR
                    string ExamplePath = "/Resources/EXAMPLE.txt";
                #else
                    string ExamplePath = "/Assets/Resources/EXAMPLE.txt";
                #endif

                Debug.Log("EXAMPLE file " + Application.dataPath + ExamplePath);
                string contentMeta = File.ReadAllText(Application.dataPath + ExamplePath);

                string exportNameMeta = file + ".meta";
                Debug.Log("exportNameMeta Meta " + exportNameMeta);

                System.IO.File.WriteAllText(exportNameMeta, contentMeta);
            }

            //Load the new object and give him his Components
            GameObject obj = objLoader.Load(objPath);

            addImportantComponent(obj);


            //Choose if it's a bracelet or a ring
            #if UNITY_EDITOR
                string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Save as a Ring or Bracelet", Application.dataPath + "/Resources", true);
            #else
                string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Save as a Ring or Bracelet", Application.dataPath + "/Assets/Resources", true);
            #endif

            string folderPath = "";
            //To not make the programme crash if nothing is selected -> Default is Bracelet
            if (folderPaths.Length > 0)
            {
                folderPath = folderPaths[0];
            }else
            {
                #if UNITY_EDITOR
                    folderPath = Application.dataPath + "/Resources/Bracelet/";
                #else
                    folderPath = Application.dataPath + "/Assets/Resources/Bracelet/";
                #endif
            }

            string localPath = "\\" + obj.name + ".obj";

            string sourcePath = objPath;
            OBJExporter objExporter = new OBJExporter();

            //Save the object at the right place to use it easly after
            objExporter.Export(folderPath + localPath, obj); //C# function in objexporter
            Debug.Log(folderPath);
        }
        
    }

    //TODO
    public void OnDisplayClick()
        {
            Debug.Log("onDisplayClick");

        }

    /// <summary>
    /// This method add all the Components to an .obj created in runtime
    /// </summary>
    /// <param name="obj">newly created GameObject</param>
    /// <returns>Returns the same GameObject, normally not usefull</returns>
    public GameObject addImportantComponent(GameObject obj)
    {
        obj.AddComponent<MergeJewel>();
        obj.AddComponent<Rigidbody>();
        obj.tag = "jewel";
        obj.AddComponent<MeshCollider>();
        obj.GetComponent<MeshCollider>().convex = true;


        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        //Resize the collider
        obj.GetComponent<MeshCollider>().sharedMesh = mesh;

        Transform children = obj.GetComponent<Transform>();
        for (int j = 0; j < children.childCount; j++)
        {
            children.GetChild(j).gameObject.SetActive(true);
        }

        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<ObjectManipulator>();
       
        obj.AddComponent<NearInteractionGrabbable>();

        //TODO see to change the size to have not too big things

                //if (Math.Abs(obj.transform.localScale.z)> 0.05f)
                //{
                //    float ratio =  obj.transform.localScale.z / 0.05f; //Z devient 0.5
                //    obj.transform.localScale = new Vector3(obj.transform.localScale.x / ratio, obj.transform.localScale.y / ratio, obj.transform.localScale.z / ratio); //DEBUG PURPOUS
                //}

        obj.transform.position = new Vector3(0, -0.1f, 1); //DEBUG

        if (obj.transform.rotation.x < 90) //DEBUG
        {
            obj.transform.Rotate(90, 0, 0);
        }

        return obj;
    }


    /// <summary>
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(220, 30 , 100, 30), "import"))
        {
           OnImportClick();

        }

        if (GUI.Button(new Rect(220, 70, 100, 30 ), "save"))
        {
            OnSaveClick();

        }

    }
}
