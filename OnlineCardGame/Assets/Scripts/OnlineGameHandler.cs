using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public enum State
{
    Draw,
    Spend,
    Ability,
    Battle,
    Resolve
}

public class OnlineGameHandler : MonoBehaviourPunCallbacks
{
    private int money;
    public int Money { get { return money; } set { moneyText.text = value.ToString(); money = value; } }

    public Camera mainCamera;
    public GameObject returnToLobbyButton;
    public GameObject masterHand;
    public GameObject guestHand;
    public GameObject board;
    public GameObject masterHeroes;
    public GameObject guestHeroes;
    public Arrow masterArrow;
    public Arrow guestArrow;
    public ArrowLocal localArrowOne;
    public ArrowLocal localArrowTwo;
    public Text phaseText;
    public Text moneyText;
    [HideInInspector]
    public List<Card> masterDeck;
    [HideInInspector]
    public List<Card> guestDeck;
    public Sprite[] boarderImages;
    public TurnAnime turnAnime;
    public Transform historyPanel;
    public GameObject historyTurnPref;

    [HideInInspector]
    public Queue<Turn> cardHistory = new Queue<Turn>();

    public bool isMasterTurn;
    public bool masterSkipped;
    public bool guestSkipped;
    public bool masterBattleFinished;
    public bool guestBattleFinished;

    public Queue<Turn> turnList = new Queue<Turn>();

    public State currentState;


    public static OnlineGameHandler Instance { get; private set; }


    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void Start()
    {
        LoadDeck();
        if (!PhotonNetwork.IsMasterClient)
        {
            mainCamera.transform.Rotate(new Vector3(0, 0, 180));
            guestArrow = PhotonNetwork.Instantiate("Arrow", Vector3.forward * -9, Quaternion.identity).GetComponent<Arrow>();
            ShuffleDeck(ref guestDeck);
        }
        else
        {
            masterArrow = PhotonNetwork.Instantiate("Arrow", Vector3.forward * -9, Quaternion.identity).GetComponent<Arrow>();
            ShuffleDeck(ref masterDeck);
        }
        currentState = State.Draw;
        DrawCard();

        Money = 8;

        isMasterTurn = true;
    }

    public void LoadDeck()
    {
        if (File.Exists(Application.dataPath + "/deck.txt"))
        {
            string json = File.ReadAllText(Application.dataPath + "/deck.txt");
            var deck = JsonUtility.FromJson<Deck>(json);
            foreach (var fileName in deck.cardFileName)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    var cardData = Resources.Load<Card>("CardsScriptableObjects/" + fileName);
                    masterDeck.Add(cardData);
                }
                else
                {
                    var cardData = Resources.Load<Card>("CardsScriptableObjects/" + fileName);
                    guestDeck.Add(cardData);
                }
            }
        }

    }

    public void DrawCard()
    {
        int cardOnHand = (PhotonNetwork.IsMasterClient ? masterHand.transform.childCount : guestHand.transform.childCount);

        phaseText.text = currentState.ToString();
        for (int i = cardOnHand; i < 7; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if(masterDeck.Count <= 0)
                {
                    LoadDeck();
                    foreach (var cardInHand in masterHand.GetComponentsInChildren<CardObject>())
                    {
                        masterDeck.Remove(cardInHand.cardData);
                    }
                    ShuffleDeck(ref masterDeck);
                }
                var card = PhotonNetwork.Instantiate("Card", Vector3.zero, masterHand.transform.rotation);
                Debug.Log(masterDeck[0].name);
                card.GetPhotonView().RPC("SetParent", RpcTarget.All, true);
                card.GetPhotonView().RPC("InitializeCard", RpcTarget.All, masterDeck[0].name);
                masterDeck.RemoveAt(0);
            }
            else
            {
                if (guestDeck.Count <= 0)
                {
                    LoadDeck();
                    foreach (var cardInHand in guestHand.GetComponentsInChildren<CardObject>())
                    {
                        guestDeck.Remove(cardInHand.cardData);
                    }
                    ShuffleDeck(ref guestDeck);
                }
                var card = PhotonNetwork.Instantiate("Card", Vector3.zero, guestHand.transform.rotation);
                Debug.Log(guestDeck[0].name);
                card.GetPhotonView().RPC("SetParent", RpcTarget.All, false);
                card.GetPhotonView().RPC("InitializeCard", RpcTarget.All, guestDeck[0].name);
                guestDeck.RemoveAt(0);
            }
        }

        SpendPhase();
        return;
    }

    public void ShuffleDeck(ref List<Card> deckToShuffle)
    {
        deckToShuffle = deckToShuffle.OrderBy((number) => Random.Range(0, 1000)).ToList();
    }

    [PunRPC]
    public void ChangeTurn()
    {
        isMasterTurn = !isMasterTurn;
        if (isMasterTurn)
        {
            masterSkipped = false;
        }
        else
        {
            guestSkipped = false;
        }
        turnAnime.StartAnime();
        UpdateHistoryUI();
    }

    public void SpendPhase()
    {
        currentState = State.Spend;
        phaseText.text = currentState.ToString();
        masterSkipped = false;
        guestSkipped = false;

    }

    public void AbilityPhase()
    {
        currentState = State.Ability;
        phaseText.text = currentState.ToString();
        masterSkipped = false;
        guestSkipped = false;
    }

    public void BattlePhase()
    {
        currentState = State.Battle;
        phaseText.text = currentState.ToString();
        StartCoroutine(Battle());
    }

    public void ResolvePhase()
    {
        currentState = State.Resolve;
        phaseText.text = currentState.ToString();
        guestBattleFinished = false;
        masterBattleFinished = false;
        Money += 5;
        foreach (var hero in masterHeroes.GetComponentsInChildren<HeroStats>())
        {
            foreach (var card in hero.equippedDefense)
            {
                hero.maxDefence -= card.armorValue;
                hero.CurrentDefence -= card.armorValue;
            }
            hero.equippedDefense.Clear();
            hero.CurrentMana += hero.baseMana;
        }
        foreach (var hero in guestHeroes.GetComponentsInChildren<HeroStats>())
        {
            foreach (var card in hero.equippedDefense)
            {
                hero.maxDefence -= card.armorValue;
                hero.CurrentDefence -= card.armorValue;
            }
            hero.equippedDefense.Clear();
            hero.CurrentMana += hero.baseMana;
        }
        DrawCard();
    }
    [PunRPC]
    public void HasSkipped(bool isMaster)
    {
        if (isMaster)
        {
            masterSkipped = true;
            isMasterTurn = false;
        }
        else
        {
            guestSkipped = true;
            isMasterTurn = true;
        }
        turnAnime.StartAnime();

        if (masterSkipped && guestSkipped && currentState != State.Battle && currentState != State.Resolve)
        {
            NextState(currentState);
        }
    }
    public void HasSkippedLocal()
    {
        if (isMasterTurn == PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("HasSkipped", RpcTarget.All, PhotonNetwork.IsMasterClient);
        }
    }

    public void NextState(State myEnum)
    {
        switch (myEnum)
        {
            case State.Spend:
                AbilityPhase();
                break;
            case State.Ability:
                BattlePhase();
                break;
            case State.Battle:
                ResolvePhase();
                break;
            default:
                return;
        }
    }

    public IEnumerator Battle()
    {
        HeroStats masterTaunt = null;
        HeroStats guestTaunt = null;

        while (true)
        {
            if (turnList.Count <= 0)
                break;
            
            var ability = turnList.Dequeue();
            if (ability.myHero.dead == true)
            {
                continue;
            }

            var hand = (ability.myHero.isMaster ? masterHand : guestHand);

            RunAnimation(new Vector2(hand.transform.position.x, hand.transform.position.y), new Vector2(ability.myHero.transform.position.x, ability.myHero.transform.position.y), true, ability.card.name);


            switch (ability.card.abilityType)
            {
                case AbilityType.Offensive:
                    HeroStats enemyHeroTemp;
                    if (ability.myHero.isMaster && guestTaunt != null)
                    {
                        enemyHeroTemp = guestTaunt;
                    }
                    else if (!ability.myHero.isMaster && masterTaunt != null)
                    {
                        enemyHeroTemp = masterTaunt;
                    }
                    else
                    {
                        enemyHeroTemp = ability.enemyHero;
                    }

                    if (enemyHeroTemp.dead)
                        continue;

                    ability.myHero.RunAnimation(new Vector2(ability.myHero.transform.position.x, ability.myHero.transform.position.y), new Vector2(enemyHeroTemp.transform.position.x, enemyHeroTemp.transform.position.y));
                    yield return new WaitForSecondsRealtime(3);

                    int armor = Mathf.Max(enemyHeroTemp.CurrentDefence - ability.card.ignoreArmorValue, 0);
                    enemyHeroTemp.CurrentHp -= Mathf.Max(ability.myHero.CurrentAttack - armor + ability.card.damageValue, 0);
                    break;
                case AbilityType.Defensive:
                    yield return new WaitForSecondsRealtime(1);
                    ability.myHero.maxDefence += ability.card.armorValue;
                    ability.myHero.CurrentDefence += ability.card.armorValue;
                    ability.myHero.equippedDefense.Add(ability.card);
                    if (ability.card.taunt)
                    {
                        if (ability.myHero.isMaster)
                        {
                            masterTaunt = ability.myHero;
                        }
                        else
                        {
                            guestTaunt = ability.myHero;
                        }
                    }
                    yield return new WaitForSecondsRealtime(2);
                    break;
                case AbilityType.Utility:
                    yield return new WaitForSecondsRealtime(1);
                    ability.myHero.CurrentHp += ability.card.restoreHealthValue;
                    yield return new WaitForSecondsRealtime(2);
                    break;
                default:
                    break;

            }

            int deadHeroesMaster = 0;
            int deadHeroesGuest = 0;

            foreach (var hero in masterHeroes.GetComponentsInChildren<HeroStats>())
            {
                if (hero.dead) { deadHeroesMaster++; }
            }
            foreach (var hero in guestHeroes.GetComponentsInChildren<HeroStats>())
            {
                if (hero.dead) { deadHeroesGuest++; }
            }
            
            if(deadHeroesMaster == masterHeroes.transform.childCount)
            {
                GameWon(false);
                break;
            }
            if (deadHeroesGuest == guestHeroes.transform.childCount)
            {
                GameWon(true);
                break;
            }
        }

        photonView.RPC("BattleFinished", RpcTarget.All, PhotonNetwork.IsMasterClient);
    }
    [PunRPC]
    public void AddTurn(string cardName, int heroID, int enemyID = -1)
    {
        //Cool animations and stuff goes here
        var cardData = Resources.Load<Card>("CardsScriptableObjects/" + cardName);
        HeroStats myHero = PhotonNetwork.GetPhotonView(heroID).GetComponent<HeroStats>();
        if (enemyID != -1)
        {
            HeroStats enemyHero = PhotonNetwork.GetPhotonView(enemyID).GetComponent<HeroStats>();
            turnList.Enqueue(new Turn { card = cardData, myHero = myHero, enemyHero = enemyHero });
        }
        else
        {
            turnList.Enqueue(new Turn { card = cardData, myHero = myHero, enemyHero = null });
        }
    }

    [PunRPC]
    public void BattleFinished(bool isMaster)
    {
        if (isMaster)
        {
            masterBattleFinished = true;
        }
        else
        {
            guestBattleFinished = true;
        }

        if (masterBattleFinished && guestBattleFinished)
        {
            NextState(currentState);
        }
    }

    public void RunAnimation(Vector2 startPosition, Vector2 endPosition, bool flip, string cardName)
    {

        AnimeCard.Instance.InitializeCard(cardName);
        AnimeCard.Instance.RunAnimation(startPosition, endPosition, flip);
    }
    
    public void GameWon(bool isMaster)
    {
        turnAnime.winText = "You Won!";
        turnAnime.loseText = "You Lose!";

        returnToLobbyButton.SetActive(true);
        
        turnAnime.StartAnimeWin(isMaster);
    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    public void PushIntoHistory(string cardName, int myHeroID, int enemyHeroID)
    {

        //Cool animations and stuff goes here
        var cardData = Resources.Load<Card>("CardsScriptableObjects/" + cardName);
        HeroStats myHero = PhotonNetwork.GetPhotonView(myHeroID).GetComponent<HeroStats>();
        if (enemyHeroID != -1)
        {
            HeroStats enemyHero = PhotonNetwork.GetPhotonView(enemyHeroID).GetComponent<HeroStats>();
            cardHistory.Enqueue(new Turn { card = cardData, myHero = myHero, enemyHero = enemyHero });
        }
        else
        {
            cardHistory.Enqueue(new Turn { card = cardData, myHero = myHero, enemyHero = null });
        }


        if (cardHistory.Count > 8)
        {
            cardHistory.Dequeue();
        }

    }

    public void UpdateHistoryUI()
    {
        
        foreach (var child in historyPanel.GetComponentsInChildren<HistoryTurn>())
        {
            Destroy(child.gameObject);

        }
        int id = cardHistory.Count;
        foreach (var turn in cardHistory)
        {
            var temp = Instantiate(historyTurnPref, historyPanel);
            temp.GetComponent<HistoryTurn>().InitializeTurn(turn);
            temp.transform.SetSiblingIndex(id);
            id--;
        }
        
    }
}
