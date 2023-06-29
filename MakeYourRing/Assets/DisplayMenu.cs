using Dummiesman;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;


/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

public class DisplayMenu : MonoBehaviour
{

    /// <summary>
    /// Hide the hand and seperate the Jewel to still be able to work on it
    /// </summary>
    public void OnCloseClick()
    {
        List<GameObject> listChildren = new List<GameObject>();
        GameObject.Find("position").GetChildGameObjects(listChildren);

        foreach(GameObject child in listChildren)
        {
            child.transform.SetParent(null, true);
        }

        GameObject.Find("Hand").SetActive(false);

    }



}
