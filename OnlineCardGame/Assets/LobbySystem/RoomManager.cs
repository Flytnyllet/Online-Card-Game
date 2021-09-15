using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public InputField roomName;
    public InputField playerName;
    public GameObject roomListArea;
    public GameObject buttonPrefab;

    private List<RoomInfo> roomListCached = new List<RoomInfo>();


    public void Start()
    {
        playerName.text = PhotonNetwork.NickName;
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
        }
    }
    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }


    public void ChangeScene()
    {
        SceneManager.LoadScene("DeckEditor");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");



        foreach (var lobby in roomList) {
            Debug.Log(lobby.Name);
            if (lobby.RemovedFromList || !lobby.IsOpen) {
                int indexCache = 0;
                int removeid = -1;
                foreach (var lobbycache in roomListCached) {
                    if (lobbycache.Name == lobby.Name) {
                        removeid = indexCache;
                        break;
                    }
                    indexCache++;
                }
                if (removeid != -1) {
                    roomListCached.RemoveAt(removeid);
                }
                continue;
            }
            bool found = false;
            foreach (var lobbycache in roomListCached) {
                if (lobbycache.Name == lobby.Name) {
                    found = true;
                }
            }
            if (!found) {
                roomListCached.Add(lobby);
            }
            /*Debug.Log("found Lobby:" + lobby.Name);
            var newButton = Instantiate(buttonPrefab, roomListArea.transform);
            newButton.name = lobby.Name;
            newButton.GetComponentInChildren<Text>().text = lobby.Name;
            var rect = roomListArea.GetComponent<RectTransform>();
            var buttonRect = newButton.GetComponent<RectTransform>();
            var sizeDelta = rect.sizeDelta;
            sizeDelta.y += buttonRect.sizeDelta.y;
            rect.sizeDelta = sizeDelta;*/
        }

        for (int i = 0; i < roomListArea.transform.childCount; i++) {
            Destroy(roomListArea.transform.GetChild(i).gameObject);
            var rect = roomListArea.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.x);
        }

        foreach (var lobbycache in roomListCached) {
            var newButton = Instantiate(buttonPrefab, roomListArea.transform);
            newButton.name = lobbycache.Name;
            newButton.GetComponentInChildren<Text>().text = lobbycache.Name;
            var rect = roomListArea.GetComponent<RectTransform>();
            var buttonRect = newButton.GetComponent<RectTransform>();
            var sizeDelta = rect.sizeDelta;
            sizeDelta.y += buttonRect.sizeDelta.y;
            rect.sizeDelta = sizeDelta;
        }
        base.OnRoomListUpdate(roomList);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.InLobby) {
            PhotonNetwork.LeaveLobby();
        }
        Debug.Log("Joined room(" + PhotonNetwork.CurrentRoom.Name + ")");
        StartGame();
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room(" + roomName.text + ")");
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName.text, ro);
    }

    public void ChangePlayerName()
    {
        PhotonNetwork.NickName = playerName.text;
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }
}
