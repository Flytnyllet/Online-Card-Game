using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollFix : MonoBehaviour
{
    RectTransform rectTransform;
    public void setSize(int cardAmount)
    {
        rectTransform = GetComponent<RectTransform>();
        int count = Mathf.CeilToInt(cardAmount / 5.0f);
        Debug.Log(cardAmount);
        Debug.Log(Mathf.CeilToInt(cardAmount / 5.0f));
        int height = count * 348 + 20;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }
}
