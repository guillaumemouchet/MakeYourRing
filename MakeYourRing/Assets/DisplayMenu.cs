using Dummiesman;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject hand;
    public bool isHandActive = false;
    public void OnCloseClick()
    {
        hand.SetActive(false);

    }



}
