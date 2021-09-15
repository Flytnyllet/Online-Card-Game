using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField]
    Image borderImage;
    [SerializeField]
    Image cardImage;
    [SerializeField]
    Text cardName;
    [SerializeField]
    Text cardType;
    [SerializeField]
    Text cardDescription;
    [SerializeField]
    GameObject cardHolder;
    [SerializeField]
    Text costText;
    [SerializeField]
    Text subTypeText;
    [SerializeField]
    Sprite[] costSprites;
    [SerializeField]
    Image costImage;
    [SerializeField]
    Image cardBack;

    bool isRunning;
    public static CardInfo Instance { get; private set; }


    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    IEnumerator ShowUseCard(string cardFileName)
    {
        var cardToShow = Resources.Load<Card>("CardsScriptableObjects/" + cardFileName);

        isRunning = true;
        cardHolder.SetActive(true);
        borderImage.sprite = OnlineGameHandler.Instance.boarderImages[cardToShow.GetCardBorderIndex()];
        cardImage.sprite = cardToShow.image;
        cardName.text = cardToShow.cardName;
        cardType.text = cardToShow.GetCardTypeName();
        cardDescription.text = cardToShow.cardText;

        costText.text = (cardToShow.cardType == CardType.EquipmentCard ? cardToShow.cost : cardToShow.manaCost).ToString();
        subTypeText.text = cardToShow.GetCardSubTypeName();
        costImage.sprite = (cardToShow.cardType == CardType.EquipmentCard ? costSprites[0] : costSprites[1]);

        yield return new WaitForSecondsRealtime(2.5f);
        cardHolder.SetActive(false);
        isRunning = false;
    }

    public void ShowUsedCard(Card cardToShow, bool showBack = false)
    {
        if (!isRunning)
        {
            if (showBack)
            {
                cardBack.gameObject.SetActive(true);
            }
            else
            {
                cardBack.gameObject.SetActive(false);
            }
            borderImage.sprite = OnlineGameHandler.Instance.boarderImages[cardToShow.GetCardBorderIndex()];
            cardImage.sprite = cardToShow.image;
            cardName.text = cardToShow.cardName;
            cardType.text = cardToShow.GetCardTypeName();
            cardDescription.text = cardToShow.cardText;
            costText.text = (cardToShow.cardType == CardType.EquipmentCard ? cardToShow.cost : cardToShow.manaCost).ToString();
            subTypeText.text = cardToShow.GetCardSubTypeName();
            costImage.sprite = (cardToShow.cardType == CardType.EquipmentCard ? costSprites[0] : costSprites[1]);
        }
    }

    public void SetVisible(bool value)
    {

        if (!isRunning)
        {
            cardHolder.SetActive(value);
            cardBack.gameObject.SetActive(value);
        }
    }
}
