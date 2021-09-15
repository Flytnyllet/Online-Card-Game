using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HistoryTurn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Sprite[] spriteIcons;

    private Card card = null;
    private HeroStats myHero = null;
    private HeroStats enemyHero = null;
    
    public void InitializeTurn(Turn turn)
    {
        this.card = turn.card;
        this.myHero = turn.myHero;
        this.enemyHero = turn.enemyHero;

        switch (card.cardType)
        {
            case CardType.EquipmentCard:
                GetComponent<Image>().color = new Color(1, 1, 0, 1);
                icon.sprite = spriteIcons[0];
                break;
            case CardType.AbilityCard:
                GetComponent<Image>().color = new Color(0, 0.1f, 1, 1);
                icon.sprite = spriteIcons[1];
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card == null)
            return;


        Transform hand = (myHero.isMaster ? OnlineGameHandler.Instance.masterHand.transform : OnlineGameHandler.Instance.guestHand.transform);
        ArrowLocal myArrow = OnlineGameHandler.Instance.localArrowOne;


        switch (card.cardType)
        {
            case CardType.EquipmentCard:
                myArrow.arrowBetween(new Vector2(hand.position.x, hand.position.y), new Vector2(myHero.transform.position.x, myHero.transform.position.y));
                CardInfo.Instance.SetVisible(true);
                CardInfo.Instance.ShowUsedCard(card);
                break;
            case CardType.AbilityCard:
                myArrow.arrowBetween(new Vector2(hand.position.x, hand.position.y), new Vector2(myHero.transform.position.x, myHero.transform.position.y));
                if (card.abilityType == AbilityType.Offensive)
                {
                    OnlineGameHandler.Instance.localArrowTwo.arrowBetween(new Vector2(myHero.transform.position.x, myHero.transform.position.y), new Vector2(enemyHero.transform.position.x, enemyHero.transform.position.y));
                }
                CardInfo.Instance.SetVisible(true);
                
                CardInfo.Instance.ShowUsedCard(card, !(PhotonNetwork.IsMasterClient == myHero.isMaster));
                break;
            default:
                break;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ArrowLocal myArrow = OnlineGameHandler.Instance.localArrowOne;
        myArrow.toggleArrow(false);
        OnlineGameHandler.Instance.localArrowTwo.toggleArrow(false);
        CardInfo.Instance.SetVisible(false);
    }
}
