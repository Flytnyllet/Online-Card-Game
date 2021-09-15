using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnText : MonoBehaviour
{
    public Text turnText;
    IEnumerator ShowTurn(string currentState)
    {
        turnText.gameObject.SetActive(true);
        turnText.text = currentState;
        yield return new WaitForSecondsRealtime(1);
        turnText.gameObject.SetActive(false);
    }
}
