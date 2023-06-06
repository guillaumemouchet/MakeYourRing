using Dummiesman;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Utility : MonoBehaviour
{


    public enum jewelType
    {
        Ring,
        Bracelet
    }

    /// <summary>
    /// Create the path with the path of where are stored or item
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
        Debug.Log(globalPath);
        return globalPath;
    }

    /// <summary>
    /// Create the path to create a certain item at index i
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
        Debug.Log(globalPath);
        return globalPath;
    }

    /// <summary>
    /// Create a dynamic button for each found .obj in our folder
    /// </summary>
    /// <param name="objFileList">List of all the .obj path</param>
    /// <param name="prefabButton">The button to copy, to have already some components</param>
    /// <param name="buttonList">List of all the button, used to destroy them when closed</param>
    /// <param name="parent">parent object of the prefab, used to have position</param>
    /// <param name="type">Type of our item, will change the paths</param>

    public static void createButton(List<string> objFileList, GameObject prefabButton, List<GameObject> buttonList, GameObject parent, jewelType type)
    {

        var globalPath = chooseEnablePath(type);

        //Get all files with .obj
        var info = new DirectoryInfo(globalPath);
        var fileInfo = info.GetFiles("*.obj").ToList<FileInfo>();
        int i = 0;
        int j = 0;
        foreach (FileInfo obj in fileInfo)
        {
            string name = Path.GetFileNameWithoutExtension(obj.Name);
            
            //Debug.Log("onEnable " + name);

            objFileList.Add(name); //To use them later

            //wrap back after 3 elements
            if (i % 3 == 0 && i != 0)
            {
                j++;
            }
            //TODO find a solution for when we have more than 9 elements because they go out of the Panel

            // Changer the position of the new buttons
            Vector3 targetPosition = prefabButton.transform.position;
            targetPosition.x += 0.04f * (i - 3 * j);
            targetPosition.y -= 0.04f * j;

            //Create the new button from a prefab
            GameObject new_btn_prefab = Instantiate(prefabButton, targetPosition, Quaternion.identity, parent.transform);
            new_btn_prefab.GetComponent<ButtonConfigHelper>().MainLabelText = name;
            new_btn_prefab.SetActive(true);

            buttonList.Add(new_btn_prefab); //Used to destroy them later


            //Debug.Log("Add Listener on " + i);

            //Create a listener to have an action linked to the right file
            int tempI = i;
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

        addImportantComponent(obj);
    }


    /// <summary>
    /// This method add all the Components to an .obj created in runtime
    /// </summary>
    /// <param name="obj">newly created GameObject</param>
    /// <returns>Returns the same GameObject, normally not usefull</returns>
    public static GameObject addImportantComponent(GameObject obj)
    {
        obj.AddComponent<MergeJewel>();
        obj.AddComponent<Rigidbody>();
        obj.tag = "jewel";
        obj.AddComponent<MeshCollider>();
        obj.GetComponent<MeshCollider>().convex = true; //Neceserry for something don't remember what


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
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        //Resize the collider
        obj.GetComponent<MeshCollider>().sharedMesh = mesh;

        //Sometimes the childs are invisible so we set them all active
        Transform children = obj.GetComponent<Transform>();
        for (int j = 0; j < children.childCount; j++)
        {
            children.GetChild(j).gameObject.SetActive(true);
        }

        //So they don't fall nor go other space
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = true;

        //Those component are those to move and interact with an Object
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<ObjectManipulator>();
        obj.AddComponent<NearInteractionGrabbable>();

        //Change the size to not be bigger than 0.5 in the relative world size
        Vector3 size = obj.GetComponent<MeshCollider>().bounds.size;
        if (Math.Abs(size.x) > 0.5f)
        {
            float ratio = size.x / 0.5f; //X devient 0.5
            obj.transform.localScale = new Vector3(obj.transform.localScale.x / ratio, obj.transform.localScale.y / ratio, obj.transform.localScale.z / ratio);
        }

        //Place the object in front of the player
        Transform cameratransform = GameObject.Find("UIRaycastCamera").transform;
        obj.transform.position = new Vector3(cameratransform.position.x, cameratransform.position.y, cameratransform.position.z) + cameratransform.forward; 
        obj.transform.rotation = cameratransform.rotation;


        return obj;
    }
}
