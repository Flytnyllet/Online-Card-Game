using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckEditor : MonoBehaviour
{
    [HideInInspector]
    public List<Card> cards;
    public CardInEditor cardInEditor;
    public Transform cardPanel;
    public Transform yourDeckPanel;
    private int currentSort;
    private int currentSortType;
    public ScrollFix scrollFix;
    public Text amountText;

    public Deck deck = new Deck();

    public List<Card> yourDeck = new List<Card>();

    public enum Sort {
        nameASC,
        nameDESC,
        costASC,
        costDESC
    };

    public enum SortTypeEnum
    {
        all,
        equipment,
        ability
    };

    public static DeckEditor Instance { get; private set; }


    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void Start()
    {
        LoadDeck();
        currentSort = 0;
        currentSortType = 0;
        SortType(currentSortType);
        UpdatePanel(currentSort);
    }


    public void Search(string q)
    {
        SortType(currentSortType);
        if (q != "") {
            cards = cards.Where(t => t.cardName.ToLower().Contains(q.ToLower())).ToList();
        }
        UpdatePanel(currentSort);
    }

    public void  LoadDeck()
    {
        if (File.Exists(Application.dataPath + "/deck.txt"))
        {
            string json = File.ReadAllText(Application.dataPath + "/deck.txt");
            deck = JsonUtility.FromJson<Deck>(json);
            foreach (var fileName in deck.cardFileName)
            {
                var cardData = Resources.Load<Card>("CardsScriptableObjects/" + fileName);
                var temp = Instantiate(cardInEditor, yourDeckPanel);
                temp.InitCard(cardData, false);
                yourDeck.Add(cardData);
            }
        }

        amountText.text = "Amount: " + yourDeck.Count;
    }

    public void SortType(int sortType)
    {
        cards.Clear();
        var temp = Resources.LoadAll("CardsScriptableObjects", typeof(Card));
        foreach (var t in temp)
        {
            cards.Add((Card)t);
        }

        switch (sortType)
        {
            case (int)SortTypeEnum.equipment:
                cards = cards.Where(x => x.cardType == CardType.EquipmentCard).ToList();
                break;
            case (int)SortTypeEnum.ability:
                cards = cards.Where(x => x.cardType == CardType.AbilityCard).ToList();
                break;
        }
        UpdatePanel(currentSort);
        currentSortType = sortType;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("RoomScene");
    }

    public void UpdatePanel(int sort)
    {
        foreach (Transform card in cardPanel)
        {
            Destroy(card.gameObject);
        }

        var cardsTemp = cards;

        switch (sort)
        {
            case (int)Sort.nameASC:
                cardsTemp = cardsTemp.OrderBy(x => x.cardName).ToList();
                break;
            case (int)Sort.nameDESC:
                cardsTemp = cardsTemp.OrderByDescending(x => x.cardName).ToList();
                break;
            case (int)Sort.costASC:
                cardsTemp = cardsTemp.OrderBy(x => x.cost).ToList();
                break;
            case (int)Sort.costDESC:
                cardsTemp = cardsTemp.OrderByDescending(x => x.cost).ToList();
                break;
        }
        foreach (Card card in cardsTemp)
        {
            var temp = Instantiate(cardInEditor, cardPanel);
            temp.InitCard(card);
        }
        scrollFix.setSize(cardsTemp.Count());
        currentSort = sort;
    }


    public void UpdateYourDeck()
    {
        foreach (Transform card in yourDeckPanel)
        {
            Destroy(card.gameObject);
        }

        var cardsTemp = yourDeck;

        foreach (Card card in cardsTemp)
        {
            var temp = Instantiate(cardInEditor, yourDeckPanel);
            temp.InitCard(card,false);
        }

        amountText.text = "Amount: " + yourDeck.Count;

        deck.cardFileName = yourDeck.Select(x => x.name).ToList();
        string json = JsonUtility.ToJson(deck);
        File.WriteAllText(Application.dataPath+ "/deck.txt",json);
    }
}
[Serializable]
public class Deck
{
    public List<string> cardFileName;
}