using System;
using System.Collections;
using UnityEngine;

public class CardBase : MonoBehaviour, IHaveCardData
{
    public CardData cardData;
    public string _animationName;
    [SerializeReference, SubclassSelector] IChoiseTarget _choiseTarget;
    [SerializeReference, SubclassSelector] IUseEffect[] _cardEffect;
    CardData IHaveCardData.CardData => cardData;

    public IEnumerator CardUse(CharacterBase useCharacter, Action animationEndAction)
    {
        CharacterBase target = null;
        BattleManager.Instance._selectEffect.SelectModeStart();
        yield return new WaitUntil(() => _choiseTarget.ChoiseTarget(useCharacter, out target));
        BattleManager.Instance._selectEffect.SelectModeEnd();
        foreach (var effect in _cardEffect)
        {
            effect.Effect(useCharacter, target);
        }
        var animator = useCharacter.GetComponent<Animator>();
        if (animator == null)
        {
            CardUseEvent();
            animationEndAction();
            yield break;
        }
        animator.Play(_animationName);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Debug.Log("èIóπ");
        animationEndAction();
        CardUseEvent();
    }
    public virtual void CardUseEvent()
    {
        if (BattleCardManager.instance._playerCards.ContainsKey(this))
        {
            BattleCardManager.instance.ChangeCardParent(this, CardPos.TrashZone);
        }
    }
}
[Serializable]
public struct CardData
{
    public string _cardName;
    public string _information;
    public int _price;
}
interface IUseEffect
{
    public void Effect(CharacterBase useCharacter, CharacterBase target);
}
interface IChoiseTarget
{
    public bool ChoiseTarget(CharacterBase useCharacter, out CharacterBase target);
}

