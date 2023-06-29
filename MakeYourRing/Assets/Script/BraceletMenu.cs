using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System;
using UnityEngine;

/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

public class BraceletMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject btnBack;

    [SerializeField]
    private GameObject btnForward;

    [SerializeField]
    private GameObject braceletMenu;

    [SerializeField]
    private GameObject prefabButton;

    [SerializeField]
    private GameObject parent;

    private List<string> objFileList = new List<string>();

    private List<GameObject> buttonList = new List<GameObject>();

    public int numberOfPages;
    public int currentPage;

    private double maxNbElementPerPages = 9.0;

    void OnDisable()
    {
        objFileList.Clear();
    }



    /// <summary>
    /// Create the first page for the pagination, as well as couting the max number of pages
    /// </summary>
    void OnEnable()
    {
        currentPage = 1;
        //Pagination
        numberOfPages = (int)Math.Ceiling(Utility.countNumberOfButton(Utility.jewelType.Bracelet) / maxNbElementPerPages);

        ////Create a button for each .obj in our resource folder
        Utility.createButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet, currentPage);
        btnBack.SetActive(false);
        btnForward.SetActive(numberOfPages > 1);

    }


    /// <summary>
    /// Change the current pagination and display other GameObjects
    /// Change visibily of the buttons depending on the number of pages
    /// </summary>
    public void onBackClick()
    {

        foreach (GameObject btn in buttonList)
        {
            Destroy(btn);
        }
        currentPage--;
        //Display new buttons
        Utility.createButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet, currentPage);

        btnForward.SetActive(currentPage < numberOfPages);//Current page is smaller than the max number of page, we cant still go forward

        btnBack.SetActive(currentPage > 1);  //Current page bigger than the min, we can come back
    }

    /// <summary>
    /// Change the current pagination and display other GameObjects
    /// Change visibily of the buttons depending on the number of pages
    /// </summary>
    public void onFrontClick()
    {
        foreach (GameObject btn in buttonList)
        {
            Destroy(btn);
        }
        currentPage++;
        //display new
        Utility.createButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet, currentPage);

        btnForward.SetActive(currentPage < numberOfPages);//Current page is smaller than the max number of page, we cant still go forward

        btnBack.SetActive(currentPage > 1);  //Current page bigger than the min, we can come back

    }


    /// <summary>
    /// On the close of the panel we want to destroy the button and create new one each time
    /// </summary>
    public void onCloseClick()
    {
        foreach (GameObject btn in buttonList)
        {
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
#else
            string localPath = "/Assets/Resources/Bracelet/" + objFileList[i] + ".obj";
#endif
        var globalPath = Application.dataPath + localPath;

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
