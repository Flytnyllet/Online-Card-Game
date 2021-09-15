using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum HeroClass
{
    Warrior, Ranger, Mage
}

public class HeroStats : PhotonView, IPointerEnterHandler, IPointerExitHandler
{
    public Transform equipmentHolder;

    public TextMesh hpText;
    public TextMesh manaText;
    public TextMesh defenceText;
    public TextMesh attackText;

    public int baseHp;
    public int baseMana;
    public int baseAttack;
    public int baseDefence;

    public float animationSpeed;

    private int currentHp;
    private int currentMana;
    private int currentDefence;
    private int currentAttack;

    private Coroutine hpCor;
    private Coroutine manaCor;
    private Coroutine attackCor;
    private Coroutine defCor;

    public int maxHp;
    public int maxMana;
    public int maxDefence;
    public int maxAttack;

    public List<Card> equippedDefense = new List<Card>();

    [HideInInspector]
    public bool dead = false;

    [HideInInspector]
    public int CurrentHp {
        get { return currentHp; }
        set {
            if (value <= 0)
            {
                dead = true;
            }
            if (value >= maxHp)
                currentHp = maxHp;
            else
                currentHp = value;
            if (hpCor != null)
            {
                StopCoroutine(hpCor);
            }
            hpCor = StartCoroutine(ChangeText(hpText, currentHp, maxHp));
        }
    }
    [HideInInspector]
    public int CurrentMana {
        get { return currentMana; }
        set {
            if (value >= maxMana)
                currentMana = maxMana;
            else
                currentMana = value;
            if (manaCor != null)
            {
                StopCoroutine(manaCor);
            }
            manaCor = StartCoroutine(ChangeText(manaText, currentMana, maxMana));
        }
    }
    [HideInInspector]
    public int CurrentAttack {
        get { return currentAttack; }
        set {
            if (value >= maxAttack)
                currentAttack = maxAttack;
            else
                currentAttack = value;
            if (attackCor != null)
            {
                StopCoroutine(attackCor);
            }
            attackCor = StartCoroutine(ChangeText(attackText, currentAttack, maxAttack));
        }
    }
    [HideInInspector]
    public int CurrentDefence {
        get { return currentDefence; }
        set {
            if (value >= maxDefence)
                currentDefence = maxDefence;
            else
                currentDefence = value;
            if (defCor != null)
            {
                StopCoroutine(defCor);
            }
            defCor = StartCoroutine(ChangeText(defenceText, currentDefence, maxDefence));
        }
    }

    [HideInInspector]
    public List<Card> currentActiveCards = new List<Card>();

    public bool isMaster;

    public HeroClass classtype;

    public Card currentArmor, currentWeapon, currentTrinket;

    private void Start()
    {
        int temp = 1;

        maxHp = baseHp;
        maxMana = baseMana;
        maxAttack = baseAttack;
        maxDefence = baseDefence;

        CurrentHp = baseHp;
        CurrentMana = baseMana;
        CurrentAttack = baseAttack;
        CurrentDefence = baseDefence;

        if (!PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(new Vector3(0, 0, 180));
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private IEnumerator ChangeText(TextMesh text, int value, int maxValue)
    {
        while (int.Parse(text.text.Split('/')[0]) != value)
        {
            if (int.Parse(text.text.Split('/')[0]) > value)
            {
                text.color = Color.red;
                text.text = (int.Parse(text.text.Split('/')[0]) - 1).ToString() + "/" + maxValue.ToString();
            }
            else
            {
                text.color = Color.green;
                text.text = (int.Parse(text.text.Split('/')[0]) + 1).ToString() + "/" + maxValue.ToString();
            }
            yield return new WaitForSeconds(0.15f);
        }
        text.color = Color.black;
        if (CurrentHp <= 0)
        {
            HeroDied();
        }
    }

    [PunRPC]
    public void Equip(string cardName)
    {

        var c = Resources.Load<Card>("CardsScriptableObjects/" + cardName);

        switch (c.equipmentType)
        {
            case EquipmentType.Armor:
                if (currentArmor != null)
                    Unequip(currentArmor.name);
                currentArmor = c;
                break;
            case EquipmentType.Weapon:
                if (currentWeapon != null)
                    Unequip(currentWeapon.name);
                currentWeapon = c;
                break;
            case EquipmentType.Trinket:
                if (currentTrinket != null)
                    Unequip(currentTrinket.name);
                currentTrinket = c;
                break;
        }

        maxHp += c.boostHp;
        maxMana += c.boostMana;
        maxAttack += c.boostAttack;
        maxDefence += c.boostDefence;

        CurrentHp += c.boostHp;
        CurrentMana += c.boostMana;
        CurrentAttack += c.boostAttack;
        CurrentDefence += c.boostDefence;
    }

    public void Unequip(string cardName)
    {
        var c = Resources.Load<Card>("CardsScriptableObjects/" + cardName);

        if (c == null)
            return;

        maxHp -= c.boostHp;
        maxMana -= c.boostMana;
        maxAttack -= c.boostAttack;
        maxDefence -= c.boostDefence;

        switch (c.equipmentType)
        {
            case EquipmentType.Armor:
                currentArmor = null;
                break;
            case EquipmentType.Weapon:
                currentWeapon = null;
                break;
            case EquipmentType.Trinket:
                currentTrinket = null;
                break;
        }
    }

    [PunRPC]
    public void UseDefence(string cardName)
    {
        var c = Resources.Load<Card>("CardsScriptableObjects/" + cardName);
        currentActiveCards.Add(c);
        CurrentDefence += c.armorValue;

    }


    IEnumerator MoveTo(Vector2 startPoint, Vector2 targetPoint)
    {
        float x = (isMaster ? -3 : 3);
        Vector2 targetPointNew = targetPoint + new Vector2(x, 0);
        Vector3 posTemp = transform.position;
        posTemp.x = startPoint.x;
        posTemp.y = startPoint.y;
        transform.position = posTemp;
        yield return new WaitForSeconds(1f);

        while (true)
        {

            if (Vector2.Distance(transform.position, targetPointNew) < 0.1f)
            {
                break;
            }

            float step = animationSpeed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, targetPointNew, step);

            yield return null;
        }

        SlashAnime.Instance.startAnime(targetPoint);
        yield return new WaitForSeconds(0.5f);

        while (true)
        {

            if (Vector2.Distance(transform.position, startPoint) < 0.1f)
            {
                break;
            }

            float step = animationSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, startPoint, step);

            yield return null;
        }

        Vector3 temp = transform.position;
        temp.z = posTemp.z;
        transform.position = temp;
    }

    [PunRPC]
    public void setMana(int value)
    {
        CurrentMana = value;
    }

    public void RunAnimation(Vector2 startPosition, Vector2 endPosition)
    {
        StartCoroutine(MoveTo(startPosition, endPosition));
    }

    public void HeroDied()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var collider = GetComponent<BoxCollider2D>();

        spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 0.4f);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        collider.enabled = false;
        this.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        equipmentHolder.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        equipmentHolder.gameObject.SetActive(false);
        CardInfo.Instance.SetVisible(false);
    }
}
