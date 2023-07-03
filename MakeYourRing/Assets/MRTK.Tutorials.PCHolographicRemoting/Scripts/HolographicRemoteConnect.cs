using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.OpenXR.Remoting;
using System.Net;


/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |      Author:  Guillaume Mouchet - ISC3il-b
 |      Notes: This code is inspired and modified from this tutorial by Microsoft https://learn.microsoft.com/en-us/training/modules/pc-holographic-remoting-tutorials/1-introduction
 *===========================================================================*/

/**
 * Some test where made to see if multiplayer was possible so with 2 Hololens 2
 * One app, started 2 times doesn't work -> same port, differant IP
 * One app, started 2 times doesn't work -> different IP and port
 * One app, started once doesn't work -> 2 IP and 2 Ports where given
 * 
 * Every time the first connexion works but when we start the second, we can't see anymore on the first Hololens but on the second yes
 * 
 * */
public class HolographicRemoteConnect : MonoBehaviour
{

    [SerializeField]
    private string IP_1;

    private bool connected = false;

    [SerializeField, Tooltip("The configuration information for the remote connection.")]

    /**
    * Parameters where given in the example, changing them doens't help with the fps
    * Changing the codex doesn't change the FPS
    * Changing the MaxBitrate doestn't change the FPS
    * */
    private RemotingConnectConfiguration remotingConfiguration_1 = new RemotingConnectConfiguration() { RemotePort = 8265, MaxBitrateKbps = 20000, VideoCodec = RemotingVideoCodec.H265, EnableAudio = false };


    /// <summary>
    /// Connect the application to the hololens using AppRemoting
    /// </summary>
    public void Connect()
    {
        connected = true;
        remotingConfiguration_1.RemoteHostName = IP_1;

        AppRemoting.StartConnectingToPlayer(remotingConfiguration_1); //Changing the remoting type doesn't change the FPS

    }

    /// <summary>
    /// Add a button to connect to the provided IP
    /// </summary>
    private void OnGUI()
    {
        IP_1 = GUI.TextField(new Rect(10, 10, 200, 30), IP_1, 25);

        string button = (connected ? "Disconnect" : "Connect");

        if (GUI.Button(new Rect(220, 10, 100, 30), button))
        {
            if (connected)
            {
                AppRemoting.Disconnect();
                connected = false;
            }
            else
                Connect();
            Debug.Log(button);

        }

    }
}