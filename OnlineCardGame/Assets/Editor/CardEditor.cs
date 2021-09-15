
 using UnityEditor;
 using UnityEngine;
 
[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{


    Card card;
    void OnEnable()
    {
        card = (Card)target;

    }

    public override void OnInspectorGUI()
    {
        card = (Card)target;
        card.cardName = EditorGUILayout.TextField("Card Name",card.cardName);
        EditorGUILayout.LabelField("Card Text");
        card.cardText = EditorGUILayout.TextArea(card.cardText, GUILayout.MaxHeight(80), GUILayout.MaxWidth(600));
        card.image = (Sprite)EditorGUILayout.ObjectField("Card Image", card.image, typeof(Sprite),true);

        card.cardType = (CardType)EditorGUILayout.EnumPopup("Card Type", card.cardType);

        switch (card.cardType)
        {
            case CardType.EquipmentCard:
                card.cost = EditorGUILayout.IntField("Cost",card.cost);
                card.boostHp = EditorGUILayout.IntField("Boost hp", card.boostHp);
                card.boostMana = EditorGUILayout.IntField("Boost Mana", card.boostMana);
                card.boostAttack = EditorGUILayout.IntField("Boost Attack", card.boostAttack);
                card.boostDefence = EditorGUILayout.IntField("Boost Defence", card.boostDefence);
                card.equipmentType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", card.equipmentType);
                break;

            case CardType.AbilityCard:
                card.manaCost = EditorGUILayout.IntField("Mana Cost", card.manaCost);
                card.abilityType = (AbilityType)EditorGUILayout.EnumPopup("Ability Type", card.abilityType);
                switch (card.abilityType)
                {
                    case AbilityType.Offensive:
                        card.damageValue = EditorGUILayout.IntField("Damage Value", card.damageValue);
                        card.ignoreArmorValue = EditorGUILayout.IntField("Ignore Armor Value", card.ignoreArmorValue);
                        break;
                    case AbilityType.Defensive:
                        card.armorValue = EditorGUILayout.IntField("Armor Value", card.armorValue);
                        card.roundDuration = EditorGUILayout.IntField("Round Duration", card.roundDuration);
                        card.taunt = EditorGUILayout.Toggle("Taunt", card.taunt);
                        break;
                    case AbilityType.Utility:
                        card.restoreHealthValue = EditorGUILayout.IntField("Restore Health Value", card.restoreHealthValue);
                        //card.restoreManaValue = EditorGUILayout.IntField("Restore Mana Value", card.restoreManaValue);
                        card.drawCardAmount = EditorGUILayout.IntField("Draw Card Amount", card.drawCardAmount);
                        break;
                }

                break;

        }
        EditorUtility.SetDirty(target);

    }
}