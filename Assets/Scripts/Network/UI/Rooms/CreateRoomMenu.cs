using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;

    private RoomCanvases _roomCanvases;
  

    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }

    public void OnClick_createRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.PlayerTtl = 120000;
        options.EmptyRoomTtl = 120000;
        if (string.IsNullOrEmpty( _roomName.text ))
        {
           PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.LocalPlayer.NickName,options,TypedLobby.Default);
        }
        else
            PhotonNetwork.JoinOrCreateRoom(_roomName.text,options,TypedLobby.Default);

    }


    public override void OnCreatedRoom()
    {
        _roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed.", this);
    }

}
