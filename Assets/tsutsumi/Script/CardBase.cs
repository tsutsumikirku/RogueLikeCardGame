using System;
using System.Collections;
using UnityEngine;

public class CardBase : MonoBehaviour, IHaveCardBase
{
    public CardData cardData;
    [HideInInspector] public string _infometion;
    public string _animationName;
    [SerializeReference, SubclassSelector] public IChoiseTarget _choiseTarget;
    [SerializeReference, SubclassSelector] public IUseEffect[] _cardEffect;
    CardBase IHaveCardBase.CardBase => this;
    private void Start()
    {
        _infometion = cardData._infometion;
    }
    //人間に任せます
    //string SetInfo(string infometion)
    //{
    //    int state = 0;
    //    int color = 0;
    //    foreach (var effect in _cardEffect)
    //    {
    //        switch (effect)
    //        {
    //            case BuffCard:
    //                if (cardData._infometion.Contains("state" + state))
    //                {
    //                    Debug.Log("文字列が置き換えられました。");
    //                    _infometion = infometion.Replace(
    //                        $"state{state}", effect.GetEffectClass<BuffCard>()._stats.ToString());
    //                    state++;
    //                }
    //                if (cardData._infometion.Contains("buffColor" + color))
    //                {
    //                    Debug.Log("文字列が置き換えられました。");
    //                    cardData._infometion = infometion.Replace(
    //                        $"buffColor{color}", effect.GetEffectClass<BuffCard>()._buff.ToString());
    //                    color++;
    //                }
    //                break;
    //            case OneTimeBuffCard:
    //                if (cardData._infometion.Contains("state" + state))
    //                {
    //                    Debug.Log("文字列が置き換えられました。");
    //                    cardData._infometion = infometion.Replace(
    //                        $"state{state}", effect.GetEffectClass<OneTimeBuffCard>()._stats.ToString());
    //                    state++;
    //                }
    //                if (cardData._infometion.Contains("buffColor" + color))
    //                {
    //                    Debug.Log("文字列が置き換えられました。");
    //                    cardData._infometion = infometion.Replace(
    //                        $"buffColor{color}", effect.GetEffectClass<OneTimeBuffCard>()._buff.ToString());
    //                    color++;
    //                }
    //                break;
    //        }
    //    }
    //}
    public IEnumerator CardUse(CharacterBase useCharacter, Action animationEndAction)
    {

        CharacterBase target = null;
        bool effectMode = _choiseTarget._effect ? true : false;
        if (effectMode)//贖罪の気持ち
        {
            BattleManager.Instance._selectEffect.SelectModeStart();
            yield return new WaitUntil(() => _choiseTarget.ChoiseTarget(useCharacter, out target));
            BattleManager.Instance._selectEffect.SelectModeEnd();
        }
        else if (_choiseTarget is CallShopCard callShop)
        {
            _choiseTarget.ChoiseTarget(useCharacter, out target);
            yield return new WaitUntil(() => callShop.storeEnd);
        }
        else _choiseTarget.ChoiseTarget(useCharacter, out target);
        foreach (var effect in _cardEffect)
        {
            effect.Effect(useCharacter, target);
        }
        var animator = useCharacter.GetComponent<Animator>();
        if (animator == null)
        {
            yield return new WaitForSeconds(0.5f);
            CardUseEvent();
            animationEndAction();
            yield break;
        }
        animator.Play(_animationName);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Debug.Log("終了");
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
    public string _infometion;
    public int _price;
}
public interface IUseEffect
{
    public void Effect(CharacterBase useCharacter, CharacterBase target);
    public T GetEffectClass<T>() where T : IUseEffect;
}
public interface IChoiseTarget
{
    public bool _effect { get; }
    public bool ChoiseTarget(CharacterBase useCharacter, out CharacterBase target);
}

