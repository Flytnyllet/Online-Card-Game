using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnAnime : MonoBehaviour
{
    public Text text;
    public GameObject panel;

    public string myTurnText;
    public string opponentTurnText;

    public string winText;
    public string loseText;

    public void StartAnime()
    {
        StopAllCoroutines();
        StartCoroutine(ShowTurnText());

    }
    IEnumerator ShowTurnText()
    {
        text.text = (OnlineGameHandler.Instance.isMasterTurn == PhotonNetwork.IsMasterClient ? myTurnText: opponentTurnText);
        panel.SetActive(true);

        yield return new WaitForSecondsRealtime(2);
        panel.SetActive(false);
    }

    public void StartAnimeWin(bool masterWon)
    {
        text.text = (masterWon == PhotonNetwork.IsMasterClient ? winText : loseText);
        panel.SetActive(true);
    }
}
