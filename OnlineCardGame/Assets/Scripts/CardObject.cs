using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct Turn
{
    public Card card;
    public HeroStats myHero;
    public HeroStats enemyHero;
}

public class CardObject : PhotonView, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Card cardData;
    public SpriteRenderer cardImage;
    public SpriteRenderer cardBoarder;
    public Camera mainCamera;
    public GameObject highlight;
    public GameObject highlightParent;
    public GameObject backside;

    public TextMesh cardName;
    public TextMesh cardType;
    public TextMesh cardText;

    public LayerMask heroMask;

    private Arrow mineArrow;
    private Vector3 startPosition;
    CardLayout2D parentObject;
    public void OnBeginDrag(PointerEventData eventData)
    {
        CardInfo.Instance.SetVisible(false);
        if (parentObject.isMaster != PhotonNetwork.IsMasterClient)
            return;

        startPosition = transform.position;
        startPosition.z = startPosition.z - 5f;
        transform.rotation = parentObject.oldRotation;
        transform.position = startPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        CardInfo.Instance.SetVisible(false);
        if (parentObject.isMaster != PhotonNetwork.IsMasterClient)
            return;
        // Vector3.up makes it move in the world x/z plane.
        Plane plane = new Plane(transform.forward, transform.position);
        Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
        Vector3 oldPos = transform.position;
        float distamce;
        if (plane.Raycast(ray, out distamce))
        {
            var dir = ray.direction;
            Vector3 result = (ray.origin + dir * distamce);
            result.z = oldPos.z;
            transform.position = result;
        }
        if ((Mathf.Abs(transform.position.y - startPosition.y) > 2f || Mathf.Abs(transform.position.x - parentObject.transform.position.x) > 5f) && OnlineGameHandler.Instance.isMasterTurn == parentObject.isMaster)
        {
            cardImage.gameObject.SetActive(false);
            cardBoarder.gameObject.SetActive(false);
            highlightParent.SetActive(false);

            mineArrow.arrowBetween(new Vector2(parentObject.transform.position.x, parentObject.transform.position.y), new Vector2(transform.position.x, transform.position.y));
        }
        else
        {
            cardImage.gameObject.SetActive(true);
            cardBoarder.gameObject.SetActive(true);
            highlightParent.SetActive(true);
            mineArrow.RPC("toggleArrow", RpcTarget.All, false);
            if (Mathf.Abs(transform.position.x - startPosition.x) > parentObject.distanceBetween)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (transform.position.x - startPosition.x > 0)
                        startPosition.x += parentObject.distanceBetween;
                    else
                        startPosition.x -= parentObject.distanceBetween;
                }
                else
                {
                    if (transform.position.x - startPosition.x > 0)
                        startPosition.x += parentObject.distanceBetween;
                    else
                        startPosition.x -= parentObject.distanceBetween;
                }
                parentObject.SortCard();
                parentObject.FixCardLayout(transform);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mineArrow.RPC("toggleArrow", RpcTarget.All, false);

        if (parentObject.isMaster != PhotonNetwork.IsMasterClient)
            return;
        parentObject.FixCardLayout();
        cardImage.gameObject.SetActive(true);
        cardBoarder.gameObject.SetActive(true);
        highlightParent.SetActive(true);

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 200, heroMask);

        if (hit.collider != null)
        {
            UseCard(hit.collider.GetComponent<HeroStats>());
        }
    }

    public void UseCard(HeroStats hero)
    {

        if (hero.isMaster == parentObject.isMaster &&
            OnlineGameHandler.Instance.isMasterTurn == parentObject.isMaster)
        {
            switch (cardData.cardType)
            {

                case CardType.EquipmentCard:
                    if (OnlineGameHandler.Instance.currentState == State.Spend && cardData.cost <= OnlineGameHandler.Instance.Money)
                    {
                        OnlineGameHandler.Instance.Money -= cardData.cost;
                        hero.RPC("Equip", RpcTarget.All, cardData.name);
                        OnlineGameHandler.Instance.photonView.RPC("PushIntoHistory", RpcTarget.All, cardData.name, hero.ViewID, -1);
                        RPC("RunAnimation", RpcTarget.All, new Vector2(parentObject.transform.position.x, parentObject.transform.position.y), new Vector2(hero.transform.position.x, hero.transform.position.y), true);
                        RPC("RemoveCard", RpcTarget.All);
                        OnlineGameHandler.Instance.photonView.RPC("ChangeTurn", RpcTarget.All);
                    }
                    break;
                case CardType.AbilityCard:
                    if (OnlineGameHandler.Instance.currentState == State.Ability && cardData.manaCost <= hero.CurrentMana)
                    {
                        switch (cardData.abilityType)
                        {
                            case AbilityType.Offensive:
                                StartCoroutine(ChooseEnemy(hero));

                                break;
                            case AbilityType.Defensive:
                                OnlineGameHandler.Instance.photonView.RPC("AddTurn", RpcTarget.All, cardData.name, hero.ViewID, -1);
                                OnlineGameHandler.Instance.photonView.RPC("PushIntoHistory", RpcTarget.All, cardData.name, hero.ViewID, -1);
                                hero.RPC("setMana",RpcTarget.All,(hero.CurrentMana - cardData.manaCost));
                                RPC("RemoveCard", RpcTarget.All);
                                OnlineGameHandler.Instance.photonView.RPC("ChangeTurn", RpcTarget.All);
                                break;
                            case AbilityType.Utility:
                                OnlineGameHandler.Instance.photonView.RPC("AddTurn", RpcTarget.All, cardData.name, hero.ViewID, -1);
                                OnlineGameHandler.Instance.photonView.RPC("PushIntoHistory", RpcTarget.All, cardData.name, hero.ViewID, -1);
                                hero.RPC("setMana", RpcTarget.All, (hero.CurrentMana - cardData.manaCost));
                                RPC("RemoveCard", RpcTarget.All);
                                OnlineGameHandler.Instance.photonView.RPC("ChangeTurn", RpcTarget.All);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }

        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentObject.isMaster != PhotonNetwork.IsMasterClient)
            return;
        RPC("SetHighlight", RpcTarget.All, true);
        CardInfo.Instance.SetVisible(true);
        CardInfo.Instance.ShowUsedCard(cardData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardInfo.Instance.SetVisible(false);
        if (parentObject.isMaster != PhotonNetwork.IsMasterClient)
            return;
        RPC("SetHighlight", RpcTarget.All, false);
    }

    [PunRPC]
    public void SetHighlight(bool val)
    {
        highlight.SetActive(val);
    }

    public void OnTransformParentChanged()
    {
        parentObject = transform.parent?.GetComponent<CardLayout2D>();
    }

    public void Start()
    {
        parentObject = transform.parent.GetComponent<CardLayout2D>();

        if (parentObject.isMaster == PhotonNetwork.IsMasterClient)
            backside.SetActive(false);

        mainCamera = Camera.main;
        cardText.text = cardData.cardText;
        cardName.text = cardData.cardName;
        cardImage.sprite = cardData.image;
        cardBoarder.sprite = OnlineGameHandler.Instance.boarderImages[cardData.GetCardBorderIndex()];
        cardType.text = cardData.GetCardTypeName();
        if (parentObject.isMaster)
        {
            mineArrow = OnlineGameHandler.Instance.masterArrow;
        }
        else
        {
            mineArrow = OnlineGameHandler.Instance.guestArrow;
        }

    }

    [PunRPC]
    public void SetParent(bool isMasterClient)
    {
        if (isMasterClient)
        {
            transform.parent = OnlineGameHandler.Instance.masterHand.transform;
            OnlineGameHandler.Instance.masterHand.GetComponent<CardLayout2D>().FixCardLayout();
        }
        else
        {
            transform.parent = OnlineGameHandler.Instance.guestHand.transform;
            OnlineGameHandler.Instance.guestHand.GetComponent<CardLayout2D>().FixCardLayout();

        }
    }

    [PunRPC]
    public void InitializeCard(string filename)
    {
        cardData = Resources.Load<Card>("CardsScriptableObjects/" + filename);

        parentObject = transform.parent.GetComponent<CardLayout2D>();

        if (parentObject.isMaster == PhotonNetwork.IsMasterClient)
            backside.SetActive(false);

        mainCamera = Camera.main;
        cardText.text = cardData.cardText;
        cardName.text = cardData.cardName;
        cardImage.sprite = cardData.image;
        cardBoarder.sprite = OnlineGameHandler.Instance.boarderImages[cardData.GetCardBorderIndex()];
        cardType.text = cardData.GetCardTypeName();

    }
    [PunRPC]
    public void RemoveCard()
    {
        var temp = parentObject;
        transform.parent = null;
        temp.FixCardLayout();
        if (IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void RunAnimation(Vector2 startPosition, Vector2 endPosition, bool flip)
    {

        AnimeCard.Instance.InitializeCard(cardData.name);
        AnimeCard.Instance.RunAnimation(startPosition, endPosition, flip);
    }

    public IEnumerator ChooseEnemy(HeroStats hero)
    {
        while (true)
        {
            Vector3 mouse = Input.mousePosition;
            mineArrow.arrowBetween(new Vector2(hero.transform.position.x, hero.transform.position.y), Camera.main.ScreenToWorldPoint(mouse));
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 200, heroMask);
                if (hit.collider != null)
                {
                    OnlineGameHandler.Instance.photonView.RPC("AddTurn", RpcTarget.All, cardData.name, hero.ViewID, hit.collider.GetComponent<HeroStats>().ViewID);
                    OnlineGameHandler.Instance.photonView.RPC("PushIntoHistory", RpcTarget.All, cardData.name, hero.ViewID, hit.collider.GetComponent<HeroStats>().ViewID);
                    hero.RPC("setMana", RpcTarget.All, (hero.CurrentMana - cardData.manaCost));
                    mineArrow.RPC("toggleArrow", RpcTarget.All, false);
                    RPC("RemoveCard", RpcTarget.All);
                    OnlineGameHandler.Instance.photonView.RPC("ChangeTurn", RpcTarget.All);
                    break;
                }
            }
            yield return null;
        }

    }
}
