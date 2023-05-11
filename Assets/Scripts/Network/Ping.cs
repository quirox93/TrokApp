using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class Ping : MonoBehaviourPunCallbacks
{
    Text pingText;
    public GameObject reconectionButton;
    // Update is called once per frame

    void Start()
    {
        pingText = this.GetComponent<Text>();
        InvokeRepeating("PrintPing", 2.0f, 5f);
    }
    
    void PrintPing()
    {
        pingText.text = PhotonNetwork.GetPing().ToString();
    }
    public override void OnConnectedToMaster()
    {
        print("Test");
    }

    public void connect()
    {
        if (PhotonNetwork.ReconnectAndRejoin())
            reconectionButton.SetActive(false);
    }
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        print("Disconected from server for reason: " + cause.ToString());
       
       reconectionButton.SetActive(true);
    }
}
