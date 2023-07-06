using Dummiesman;
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

    /*=============================================================================
     |                               Attributes
     *===========================================================================*/
    public int numberOfPages;
    public int currentPage;

    [SerializeField]
    private GameObject btnBack;

    [SerializeField]
    private GameObject btnForward;

    [SerializeField]
    private GameObject braceletMenu;

    [SerializeField]
    private GameObject prefabButton;

    [SerializeField]
    private GameObject parent; //Parent Panel, used for positioning

    private List<string> objFileList = new List<string>();

    private List<GameObject> buttonList = new List<GameObject>();

    private double maxNbElementPerPages = 9;

    


    /*=============================================================================
    |                               Unity Public Functions
    *===========================================================================*/
    void OnDisable()
    {
        objFileList.Clear();
    }

    /// <summary>
    /// Create the first page for the pagination, as well as counting the max number of pages
    /// </summary>
    void OnEnable()
    {
        currentPage = 1;
        //Pagination
        numberOfPages = (int)Math.Ceiling(Utility.CountNumberOfButton(Utility.jewelType.Bracelet) / maxNbElementPerPages);

        ////Create a button for each .obj in our resource folder
        Utility.CreateButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet, currentPage);
        btnBack.SetActive(false);
        btnForward.SetActive(numberOfPages > 1);

    }


   /*=============================================================================
    |                               Public Functions
    *===========================================================================*/
    /// <summary>
    /// Go back one page in the pagination
    /// </summary>
    public void OnBackClick()
    {
        currentPage--;
        ChangePage();

    }

    /// <summary>
    /// Delete all buttons before closing the panel
    /// </summary>
    public void OnCloseClick()
    {
        foreach (GameObject btn in buttonList)
        {
            Destroy(btn);
        }
        braceletMenu.SetActive(false);

    }

    /// <summary>
    /// go forward one page in the pagination
    /// </summary>
    public void OnFrontClick()
    {
        currentPage++;
        ChangePage();
    }


   /*=============================================================================
    |                               Private Functions
    *===========================================================================*/
    /// <summary>
    /// Change the current pagination and display other Buttons
    /// Change visibily of the forward and back buttons depending on the number of pages
    /// </summary>
    private void ChangePage()
    {
        foreach (GameObject btn in buttonList)
        {
            Destroy(btn);
        }
        //display new
        Utility.CreateButton(objFileList, prefabButton, buttonList, parent, Utility.jewelType.Bracelet, currentPage);

        btnForward.SetActive(currentPage < numberOfPages);//Current page is smaller than the max number of page, we cant still go forward

        btnBack.SetActive(currentPage > 1);  //Current page bigger than the min, we can come back
    }

#if DEBUG_MODE
    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    private void OnItemClickDebug(int i)
    {
        Debug.Log("On itemClick" + i);

#if UNITY_EDITOR
        string localPath = "/Resources/Bracelet/" + objFileList[i] + ".obj";
#else
            string localPath = "/Assets/Resources/Bracelet/" + objFileList[i] + ".obj";
#endif
        var globalPath = Application.dataPath + localPath;

        OBJLoader objLoader = new OBJLoader();
        //Load the file
        GameObject obj = objLoader.Load(globalPath);

        Utility.AddImportantComponent(obj); //Add the important Components

    }

    /// <summary>
    /// This method adds quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(370, 40, 150, 40), "item click bracelet 0"))
        {
            OnItemClickDebug(0);
        }
    }
#endif
}
