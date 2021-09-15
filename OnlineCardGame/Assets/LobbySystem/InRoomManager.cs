using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviourPunCallbacks
{
    public GameObject playerListArea;
    public GameObject buttonPrefab;
    public GameObject startButton;

    public void Start()
    {
        Debug.Log(PhotonNetwork.InRoom);
        if (PhotonNetwork.InRoom)
        {
            UpdatePlayerList(PhotonNetwork.CurrentRoom.Players);
        }
    }
    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
        base.OnLeftRoom();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
        //PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel(1);
        base.OnJoinedLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList(PhotonNetwork.CurrentRoom.Players);
        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList(PhotonNetwork.CurrentRoom.Players);
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public void UpdatePlayerList(Dictionary<int,Player> playerList)
    {
        for (int i = 0; i < playerListArea.transform.childCount; i++) {
            Destroy(playerListArea.transform.GetChild(i).gameObject);
            var rect = playerListArea.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.x);
        }


        foreach (var lobby in playerList) {
            Debug.Log("found player:" + lobby.Value.NickName);
            var newButton = Instantiate(buttonPrefab, playerListArea.transform);
            newButton.name = lobby.Value.NickName;
            newButton.GetComponentInChildren<Text>().text = lobby.Value.NickName;
            var rect = playerListArea.GetComponent<RectTransform>();
            var buttonRect = newButton.GetComponent<RectTransform>();
            var sizeDelta = rect.sizeDelta;
            sizeDelta.y += buttonRect.sizeDelta.y;
            rect.sizeDelta = sizeDelta;
        }

        if (PhotonNetwork.IsMasterClient) {
            startButton.SetActive(true);
        }else {
            startButton.SetActive(false);
        }
    }


    public void LeaveRoom()
    {
        Debug.Log("Leaving room");
        PhotonNetwork.LeaveRoom();
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(3);
    }
}
