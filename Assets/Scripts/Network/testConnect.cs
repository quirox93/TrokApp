using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class testConnect : MonoBehaviourPunCallbacks
{
      public Transform connectText;
      public Button crearSala;
      public Text log;

    private void Start()
    {
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
        connectText.GetChild(0).gameObject.SetActive(true);
        connectText.GetChild(1).gameObject.SetActive(false);
        connectText.GetChild(2).gameObject.SetActive(false);
        crearSala.interactable = false;
    }

 
    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        connectText.GetChild(0).gameObject.SetActive(false);
        connectText.GetChild(1).gameObject.SetActive(true);
        connectText.GetChild(2).gameObject.SetActive(false);
        crearSala.interactable = true;
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        //print("Disconected from server for reason: " + cause.ToString());
        //log.text = "Disconected from server for reason: " + cause.ToString();
        connectText.GetChild(0).gameObject.SetActive(false);
        connectText.GetChild(1).gameObject.SetActive(false);
        connectText.GetChild(2).gameObject.SetActive(true);
        crearSala.interactable = false;
    }

}

