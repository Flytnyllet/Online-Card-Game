using UnityEngine;

public enum CardType
{
    EquipmentCard,
    AbilityCard
}
public enum EquipmentType
{
    Weapon,
    Armor,
    Trinket
}

public enum AbilityType
{
    Offensive, // Minska Hp, minska defence, detta ska beräknas ut efter spelarens attack value och motsåndarens defence value
    Defensive, // Öka defence, taunt
    Utility // Dra ability kort, restore hp, restore mana, (unika effekter)????, 
}
[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject
{
    public string cardName;
    public string cardText;
    public Sprite image;
    public CardType cardType;
    public int cost;
    public int boostHp;
    public int boostMana;
    public int boostAttack;
    public int boostDefence;
    public EquipmentType equipmentType;
    public int manaCost;
    public AbilityType abilityType;
    // if offensive 
    public int damageValue;
    public int ignoreArmorValue;
    //if defensive
    public int armorValue;
    public int roundDuration;
    public bool taunt;
    //if utility
    public int restoreHealthValue;
    //public int restoreManaValue;
    public int drawCardAmount;

    public string GetCardTypeName()
    {
        if (cardType == CardType.EquipmentCard)
        {
            return "Equipment";
        }
        else if (cardType == CardType.AbilityCard)
        {
            return "Ability";
        }
        else
        {
            return "";
        }
    }


    public int GetCardBorderIndex() 
    {
        if (cardType == CardType.EquipmentCard)
        {
            return 0;
        }
        else if (cardType == CardType.AbilityCard)
        {
            return 1;
        }
        else
        {
            return 10000;
        }
    }

    public string GetCardSubTypeName() 
    
    {
        if (cardType == CardType.EquipmentCard)
        {
            if (equipmentType == EquipmentType.Armor)
            {
                return "Armor";
            }
            else if (equipmentType == EquipmentType.Trinket)
            {
                return "Trinket";
            }
            else
            {
                return "Weapon";
            }
        }
        else if (cardType == CardType.AbilityCard)
        {
            if (abilityType == AbilityType.Defensive)
            {
                return "Defensive";
            }
            else if (abilityType == AbilityType.Offensive)
            {
                return "Offensive";
            }
            else
            {
                return "Utility";
            }
        }
        else
        {
            return "";
        }

    }

}
