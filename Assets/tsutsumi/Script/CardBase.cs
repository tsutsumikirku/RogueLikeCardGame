using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [SerializeField] Sprite _sprite;
    [SerializeField] Text _explanationText;
    public CardData _cardData;
    CardBase(CardData cardData)
    {
        Instantiate(gameObject);
        _explanationText.text=_cardData._explanation;
        _sprite=_cardData._cardSpite;
    }
    public void CardUse(CharacterBase useCharacter, Action animationEndAction)
    {
        StartCoroutine(_cardData.CardUse(useCharacter,  animationEndAction));
    }
    public virtual void CardUseEvent()
    {
        BattleManager.Instance._trashParent.SetParent(transform);
    }
}
public enum BuffDebuff
{
    OverRide,
    Attack,
    Buff,
    OneTimeBuff,
    AllAttack,
    AttackCount,
    Debuff,
    AllElementAttack
}
public enum DebuffSelectState
{
    None,
    SelectBefore,
    SelectAfter
}
