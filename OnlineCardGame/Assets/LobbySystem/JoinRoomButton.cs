using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(gameObject.name);
    }
}
