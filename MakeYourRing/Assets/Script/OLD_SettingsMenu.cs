using SFB;
using UnityEngine;
using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if WINDOWS_UWP
using Windows.Storage;
#endif


//TODO Toujours ajouter le dossier asset fournis dans le build, c'est le que le User devra venir stocker ses eléments (Ring, Bracelet, Result), l'autre asset en plus est jsute pour le Debug
public class OLD_SettingsMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject settingsMenu;

    [SerializeField]
    private GameObject debugText;
    //Since its a handMenu see how to close and open it
    public void OnCloseClick()
    {
        settingsMenu.SetActive(false);

    }

    public void OnSaveClick()
    {
        settingsMenu.SetActive(false);

        //https://docs.unity3d.com/ScriptReference/PrefabUtility.SaveAsPrefabAsset.html // Works only in Editor mode
        GameObject[] leaderObjects = GameObject.FindGameObjectsWithTag("leader");
        Debug.Log("onSaveClick, find all leader tag");

        foreach (GameObject leaderObject in leaderObjects)
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

    public void OnImportClick()
    {
        var objPath = "";


        // on passe directement dehors au moment ou on click dessus donc le waitUntilFinished à True ne fonctionne pas
        // U:\\Users\\guillaume.mouchet@he-arc.ch\\Documents\\MakeYourRing\\Bracelets\\chandra_default.obj
        // ON ne pourrait écrire que depuis le persistant data path
        // Application.persistentDataPath
        //On ne peut pas OUVRIR LE PERISTANT DATAPATH

#if ENABLE_WINMD_SUPPORT

                UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
                    {
                        //var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(Application.persistentDataPath);

                        var filepicker = new  Windows.Storage.Pickers.FileOpenPicker();
                        //filepicker.InitialLocation = folder;

                        // filepicker.FileTypeFilter.Add("*");
                        filepicker.FileTypeFilter.Add(".obj");

                        var file = await filepicker.PickSingleFileAsync();
                        UnityEngine.WSA.Application.InvokeOnAppThread(async () => 
                        {
                            paths = file.Path;



                            /*

                            var storageFolder = await StorageFolder.GetFolderFromPathAsync(paths);

                            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("Engagement Ring.obj");

                            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);

                            debugLog.GetComponent<TextMeshPro>().text = text;
                            */

                            //paths = "U:\\Users\\guillaume.mouchet@he-arc.ch\\Documents\\MakeYourRing\\Bracelets\\chandra_default.obj";

                            /*
                                    OBJLoader objLoader = new OBJLoader();
                            
                                    debugLog.GetComponent<TextMeshPro>().text = paths;
                                    GameObject obj = objLoader.Load(paths);

                                    debugLog.GetComponent<TextMeshPro>().text = "finished?";


                            
                                    ////Set it in scene

                                    ////TODO : ADD ALL THE IMPORTANT SCRIPTS

                                    obj = addImportantComponent(obj);
                            
                                    obj.SetActive(true);
                            */
                        }, false);
                    }, false);


#else
        // Open file with filter
        var extensions = new[] {
             new ExtensionFilter("Object file", "obj"),
         };
        objPath = StandaloneFileBrowser.OpenFilePanel("Choose object", "", extensions, true)[0];

        //Create Object fonctionne en build normalement, il lui manque juste le chemin d'acces

        debugText.GetComponent<TextMeshProUGUI>().text = objPath;

        OBJLoader objLoader = new OBJLoader();

        //need to wait on the paths        
        GameObject obj = objLoader.Load(objPath);

        addImportantComponent(obj);


        //Choisir si on l'ajoute dans Ring ou Bracelet Resource

        string folderPath = "";
        folderPath = StandaloneFileBrowser.OpenFolderPanel("Save as a Ring or Bracelet", "Assets/Resources", true)[0];
        string localPath = "\\" + obj.name + ".obj";

        string sourcePath = objPath;
        OBJExporter objExporter = new OBJExporter();  //Seems to work only in editor mode
        objExporter.Export(folderPath + localPath, obj); //C# function in objexporter
        Debug.Log(folderPath);



#endif



        /*
         * Pas utilisable car PrefabUtility utilise UnityEditor et donc ne donctionne pas sur le Hololens 2
         * Les objets seront toujours en tant que .obj, si cela fonctionne pour l'import
         * 
         * 
        // //Save in new prefabs
        // string localPath = "/"+obj.name + ".prefab";

        // paths = StandaloneFileBrowser.OpenFolderPanel("Save prefab", "Assets/Resources", true);
        // var globalPath = paths[0] + localPath;

        // // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess = false;
            //PrefabUtility.SaveAsPrefabAsset(obj, globalPath, out prefabSuccess);

            //Eviter les objets avec des enfantss
            // THIS OBJECT CHILDERN ARE MODIFIED WHEN REIMPORTED but
            //Use this function to create a Prefab Asset at the given path from the given GameObject, including any childen in the Scene without modifying the input objects.

            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);

            //Save the Renderer and filter
            //LE NOM DE L'oobjet c'est que moyen ouf vu qu'il peut y avoir 2 enfant, ajouter un indice en plus??
            // de l'autre cote on a acces que au nom
            //meshfilters = obj.GetComponentsInChildren<MeshFilter>().ToList<MeshFilter>();
            //meshRenderer = obj.GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();

            Debug.Log("Create Asset");
            //Save the meshfilters
            //int i = 0;
            //foreach (MeshFilter filter in meshfilters)
            //{
            //    var savePath = "Assets/Resources/Bracelet/" + obj.name + i + ".mesh";
            //    Debug.Log("Saved Filter to:" + savePath);

            //    //AssetDatabase.CreateAsset(filter.mesh, savePath);
            //    // filterDict.Add(obj.name + i, filter);
            //    //AssetDatabase.SaveAssets();
            //    i++;
            //}
            //Save the meshRenderers

            //i = 0;
            //foreach (MeshRenderer renderer in meshRenderer)
            //{
            //    var savePath = "Assets/Resources/Bracelet/" + obj.name + i + ".mesh";
            //    Debug.Log("Saved Renderer to:" + savePath);
            //    //AssetDatabase.CreateAsset(renderer.material, savePath);
            //    //AssetDatabase.SaveAssets();
            //    i++;
            //}

            //foreach (var item in filterDict)
            //{
            //    Debug.Log(item.Key + item.Value);
            //}
        */
    }
    public void OnDisplayClick()
    {
        Debug.Log("onDisplayClick");

    }


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
        float ratio = obj.transform.localScale.z / 0.05f;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x / ratio, obj.transform.localScale.y / ratio, obj.transform.localScale.z / ratio); //DEBUG PURPOUS
        obj.transform.position = new Vector3(0, -0.1f, 1);
        obj.transform.Rotate(90, 0, 0);
        return obj;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(220, 30, 100, 30), "import"))
        {
            OnImportClick();

        }

        if (GUI.Button(new Rect(220, 70, 100, 30), "save"))
        {
            OnSaveClick();

        }

    }
}
