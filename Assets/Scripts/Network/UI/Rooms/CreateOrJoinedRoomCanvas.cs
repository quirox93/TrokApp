using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinedRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateRoomMenu _createRoomMenu;

    [SerializeField]
    private RoomListingMenu _roomListingMenu;
    private RoomCanvases _roomCanvases;
    

    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
        _createRoomMenu.FirstInitialize(canvases);
        _roomListingMenu.FirstInitialize(canvases);
    }

    public void OnButtonQuit()
    {
        Application.Quit();
    }
}
