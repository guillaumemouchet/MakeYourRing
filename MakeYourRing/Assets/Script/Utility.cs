using Dummiesman;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

public class Utility : MonoBehaviour
{
    public enum jewelType
    {
        Ring,
        Bracelet
    }

    /// <summary>
    /// Create the global path with the local path of where are stored the assets
    /// </summary>
    /// <param name="type">Type of our item, will change the paths</param>
    private static string chooseEnablePath(jewelType type)
    {
        string localPath = "";
        switch (type)
        {
            case jewelType.Ring:
                {
#if UNITY_EDITOR
                    localPath = "/Resources/Ring/";
#else
                        localPath = "/Assets/Resources/Ring/";
#endif
                }
                break;

            case jewelType.Bracelet:
                {
#if UNITY_EDITOR
                    localPath = "/Resources/Bracelet/";
#else
                        localPath = "/Assets/Resources/Bracelet/";
#endif
                }
                break;
        }

        var globalPath = Application.dataPath + localPath;
        return globalPath;
    }

    /// <summary>
    /// Create the path to load the asset at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    /// <param name="objFileList">List of all the .obj path</param>
    /// <param name="type">Type of our item, will change the paths</param>
    private static string chooseClickPath(jewelType type, List<string> objFileList, int i)
    {
        string localPath = "";
        switch (type)
        {

            case jewelType.Ring:
                {
#if UNITY_EDITOR
                    localPath = "/Resources/Ring/" + objFileList[i] + ".obj";
#else
                    localPath = "/Assets/Resources/Ring/" + objFileList[i] + ".obj";
#endif
                }
                break;

            case jewelType.Bracelet:
                {
#if UNITY_EDITOR
                    localPath = "/Resources/Bracelet/" + objFileList[i] + ".obj";
#else
                    localPath = "/Assets/Resources/Bracelet/" + objFileList[i] + ".obj";
#endif
                }
                break;


        }

        var globalPath = Application.dataPath + localPath;
        return globalPath;
    }


    /// <summary>
    /// Counts the number of file that are .obj in our folder
    /// </summary>
    /// <param name="type">Type of our item, will change the paths</param>
    /// <returns type="int">The size of our list</returns>
    public static int countNumberOfButton(jewelType type)
    {
        var globalPath = chooseEnablePath(type);

        //Get all files with .obj
        var info = new DirectoryInfo(globalPath);
        var fileInfo = info.GetFiles("*.obj").ToList<FileInfo>();
        return fileInfo.Count;
    }

    /// <summary>
    /// Create a dynamic button for each found .obj in our folder up to nine by page, or else pagination is made
    /// </summary>
    /// <param name="objFileList">List of all the .obj path</param>
    /// <param name="prefabButton">The button to copy, to have already some components</param>
    /// <param name="buttonList">List of all the button, used to destroy them when closed</param>
    /// <param name="parent">parent object of the prefab, used to have position</param>
    /// <param name="type">Type of our item, will change the paths</param>
    /// <param name="currentPage">We can only display 9 elements at a time, so we need to have pagination</param>
    public static void createButton(List<string> objFileList, GameObject prefabButton, List<GameObject> buttonList, GameObject parent, jewelType type, int currentPage)
    {
        var nbElementPerPage = 9; //max number by panel, 
        var globalPath = chooseEnablePath(type);

        //Get all files with .obj
        var info = new DirectoryInfo(globalPath);
        var fileInfo = info.GetFiles("*.obj").ToList<FileInfo>();
        int count = Math.Min(nbElementPerPage, fileInfo.Count - nbElementPerPage * (currentPage - 1));

        var subList = fileInfo.GetRange((currentPage - 1) * nbElementPerPage, count); //Create a sublist of the element we want to display

        int i = 0;
        int j = 0;
        foreach (FileInfo obj in subList)
        {
            string name = Path.GetFileNameWithoutExtension(obj.Name);

            objFileList.Add(name);

            //wrap back after 3 elements
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
            int tempI = i + (currentPage - 1) * nbElementPerPage;
            jewelType tempType = type;
            new_btn_prefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { onItemClick(tempI, objFileList, tempType); }); // change to temp value to not have the reference but the value only
            i++;

        }
    }



    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    /// <param name="objFileList">List of all the .obj path</param>
    /// <param name="type">Type of our item, will change the paths</param>
    private static void onItemClick(int i, List<string> objFileList, jewelType type)
    {
        //Debug.Log("On itemClick" + i);
        var globalPath = chooseClickPath(type, objFileList, i);

        OBJLoader objLoader = new OBJLoader();
        //Load the file
        GameObject obj = objLoader.Load(globalPath);

        obj = addImportantComponent(obj);

    }


    /// <summary>
    /// This method add all the Components to an .obj created in runtime
    /// </summary>
    /// <param name="obj">newly created GameObject</param>
    /// <returns>Returns the same GameObject, normally not used</returns>
    public static GameObject addImportantComponent(GameObject obj)
    {
        obj.AddComponent<MergeJewel>();
        obj.AddComponent<Rigidbody>();
        obj.tag = "jewel";
        obj.layer = LayerMask.NameToLayer("Jewel");
        obj.AddComponent<MeshCollider>();
        obj.GetComponent<MeshCollider>().convex = false;


        //Need to combine the collider with the children of the object to have only one of the good size and not multiple
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
        Array.Clear(meshFilters, 0, meshFilters.Length);
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        //Resize the collider
        obj.GetComponent<MeshCollider>().sharedMesh = mesh;

        //Sometimes the childs are invisible so we set them all active
        Transform objTransform = obj.GetComponent<Transform>();
        for (int j = 0; j < objTransform.childCount; j++)
        {
            objTransform.GetChild(j).gameObject.SetActive(true);
        }

        //So they don't fall nor go other space
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        //Those componenta are those to move and interact with an Object
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<ObjectManipulator>();
        obj.AddComponent<NearInteractionGrabbable>();

        //Change the size to not be bigger than 0.3 in the relative world size
        Vector3 size = obj.GetComponent<MeshCollider>().bounds.size;
        if (Math.Abs(size.x) > 0.3f)
        {
            float ratio = (size.x) / 0.3f;
            obj.transform.localScale = new Vector3(obj.transform.localScale.x / ratio, obj.transform.localScale.y / ratio, obj.transform.localScale.z / ratio);
        }

        ////Place the object in front of the player
        Transform cameratransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        obj.transform.position = new Vector3(cameratransform.position.x, cameratransform.position.y, cameratransform.position.z) + cameratransform.forward * 0.2f;
        obj.transform.rotation = new Quaternion(cameratransform.rotation.x, cameratransform.rotation.y, cameratransform.rotation.z, cameratransform.rotation.w);

        return obj;
    }


    /// <summary>
    /// Create a Meta file with Readable at true for each PNG and JPG in the folder
    /// </summary>
    /// <param name="objPath">Folder where our object is</param>
    public static void createMetaDataFile(string objPath)
    {
        //Create at runtime ".meta" files for each PNG and JPG
        string directoryFolder = Path.GetDirectoryName(objPath);

        //Only PNG and JPG files are done for know, more can easly be added if necessary
        IEnumerable<string> files = Directory.EnumerateFiles(directoryFolder, "*.*", SearchOption.AllDirectories)
            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.EndsWith(".obj", StringComparison.OrdinalIgnoreCase) && !file.EndsWith(".mtl", StringComparison.OrdinalIgnoreCase));

        //Create a meta file foreach png or jpg files
        foreach (string file in files)
        {
            //Create a new meta file
#if UNITY_EDITOR
            string ExamplePath = "/Resources/EXAMPLE.txt";
#else
                string ExamplePath = "/Assets/Resources/EXAMPLE.txt";
#endif

            string contentMeta = File.ReadAllText(Application.dataPath + ExamplePath);

            string exportNameMeta = file + ".meta";

            System.IO.File.WriteAllText(exportNameMeta, contentMeta);
        }
    }
}
