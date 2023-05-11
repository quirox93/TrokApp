using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCanvases : MonoBehaviour
{
    [SerializeField]
    private CreateOrJoinedRoomCanvas _createOrJoinedRoomCanvas;
    public CreateOrJoinedRoomCanvas CreateOrJoinedRoomCanvas { get { return _createOrJoinedRoomCanvas;}}

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas { get { return _currentRoomCanvas;}}

    private void Awake()
    {
        FirstInitialize();
    }

    private void FirstInitialize()
    {
        CreateOrJoinedRoomCanvas.FirstInitialize(this);
        CurrentRoomCanvas.FirstInitialize(this);
    }
}
