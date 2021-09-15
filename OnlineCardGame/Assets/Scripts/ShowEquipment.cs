using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowEquipment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public HeroStats hero;
    public EquipmentType type;

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                if(hero.currentWeapon == null) { return; }
                CardInfo.Instance.SetVisible(true);
                CardInfo.Instance.ShowUsedCard(hero.currentWeapon);
                break;
            case EquipmentType.Armor:
                if (hero.currentArmor == null) { return; }
                CardInfo.Instance.SetVisible(true);
                CardInfo.Instance.ShowUsedCard(hero.currentArmor);
                break;
            case EquipmentType.Trinket:
                if (hero.currentTrinket == null) { return; }
                CardInfo.Instance.SetVisible(true);
                CardInfo.Instance.ShowUsedCard(hero.currentTrinket);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardInfo.Instance.SetVisible(false);
    }
}
