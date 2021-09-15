using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInEditor : MonoBehaviour
{
    [SerializeField]
    Image cardImage;
    [SerializeField]
    Image cardBorder;
    [SerializeField]
    Text cardType;
    [SerializeField]
    Text cardName;
    [SerializeField]
    Text cardDescription;
    public Sprite[] boarderImages;
    public Sprite[] costSprites;
    public Image costImage;
    public Text costText;
    public Text subTypeText;
    Button button;

    Card currentCard;

    public void Start()
    {
 
    }

    void AddCard()
    {
        if (DeckEditor.Instance.yourDeck.Count < 40) {
            DeckEditor.Instance.yourDeck.Add(currentCard);
            DeckEditor.Instance.UpdateYourDeck();
        }
    }
    void RemoveCard()
    {
        DeckEditor.Instance.yourDeck.Remove(currentCard);
        DeckEditor.Instance.UpdateYourDeck();
    }

    public void InitCard(Card card, bool add = true)
    {
        button = GetComponent<Button>();
        cardBorder.sprite = boarderImages[card.GetCardBorderIndex()];
        cardImage.sprite = card.image;
        cardName.text = card.cardName;
        cardType.text = card.GetCardTypeName();
        cardDescription.text = card.cardText;
        costText.text = (card.cardType == CardType.EquipmentCard ? card.cost : card.manaCost).ToString();
        subTypeText.text = card.GetCardSubTypeName();
        costImage.sprite = (card.cardType == CardType.EquipmentCard ? costSprites[0] : costSprites[1]);
        currentCard = card;
        if (add)
            button.onClick.AddListener(AddCard);
        else
            button.onClick.AddListener(RemoveCard);
    }
    public void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
