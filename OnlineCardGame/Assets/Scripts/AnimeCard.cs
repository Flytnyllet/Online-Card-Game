using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeCard : MonoBehaviour
{
    [HideInInspector]
    public Card cardData;
    public SpriteRenderer cardImage;
    public SpriteRenderer cardBoarder;
    public GameObject highlight;
    public GameObject highlightParent;
    public GameObject backside;

    public TextMesh cardName;
    public TextMesh cardType;
    public TextMesh cardText;
    public Animator anime;

    public TextMesh costText;
    public TextMesh subTypeText;
    public SpriteRenderer costSprite;
    public Sprite[] costImages;

    public float speed = 10.0f;

    public static AnimeCard Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("WTF IS HAPPENING");
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            var tempRotation = transform.parent.rotation;
            tempRotation.eulerAngles = new Vector3(0, 0, 180);
            transform.parent.rotation = tempRotation;
        }
    }

    public void InitializeCard(string filename)
    {
        anime.Play("Idle");
        cardData = Resources.Load<Card>("CardsScriptableObjects/" + filename);
        backside.SetActive(true);
        cardText.text = cardData.cardText;
        cardName.text = cardData.cardName;
        cardImage.sprite = cardData.image;
        costText.text = (cardData.cardType == CardType.EquipmentCard ? cardData.cost : cardData.manaCost).ToString();
        subTypeText.text = cardData.GetCardSubTypeName();
        costSprite.sprite = (cardData.cardType == CardType.EquipmentCard ? costImages[0] : costImages[1]);
        cardBoarder.sprite = OnlineGameHandler.Instance.boarderImages[cardData.GetCardBorderIndex()];
        cardType.text = cardData.GetCardTypeName();

    }

    IEnumerator MoveTo(Vector2 startPoint, Vector2 targetPoint, bool flip)
    {
        Vector3 posTemp = transform.position;
        posTemp.x = startPoint.x;
        posTemp.y = startPoint.y;
        transform.position = posTemp;
        while (true)
        {

            if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            {
                if(flip)
                    CardInfo.Instance.StartCoroutine("ShowUseCard", cardData.name);
                anime.Play(flip ? "Flip" : "NoFlip");
                break;
            }

            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPoint, step);


            yield return null;
        }
    }

    public void RunAnimation(Vector2 startPosition, Vector2 endPosition, bool flip)
    {
        Debug.Log(startPosition);
        Debug.Log(endPosition);
        StartCoroutine(MoveTo(startPosition, endPosition, flip));
    }
}
