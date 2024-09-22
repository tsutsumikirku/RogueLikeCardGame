using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardList")]
public class CardDataScriptablObj : ScriptableObject
{
    public List<CardBase> Cards = new List<CardBase>();
}
[CreateAssetMenu(fileName = "CardData", menuName = "CardEffect/EnptyCardData")]
public class CardData : ScriptableObject
{
    public Sprite _cardSpite;
    public string _cardName;
    public string _explanation;
    public int _price = 1;
    public int _cardPlayCount;
    public CardUseEffect _effect;
    public string _animationName;
    public IEnumerator CardUse(CharacterBase useCharacter, Action animationEndAction)
    {
        float timer = 0;
        yield return new WaitUntil(() => _effect.ChoiseTarget());
        _effect.Effect(useCharacter, _effect.target != null ? _effect.target : useCharacter, _cardPlayCount);
        var animator = useCharacter.GetComponent<Animator>();
        if (animator == null)
        {
            animationEndAction();
            yield break;
        }
        animator.Play(_animationName);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                Debug.Log("終了");
                animationEndAction();
                break;
            }
        }
    }
}
public abstract class CardUseEffect : ScriptableObject
{
    [HideInInspector] public CharacterBase target;
    public virtual bool ChoiseTarget() { return true; }
    public abstract void Effect(CharacterBase useCharacter, CharacterBase targetCharacter, int count);
}

[CreateAssetMenu(fileName = "BuffCard", menuName = "CardEffect/BuffCard")]
public class BuffCard : CardUseEffect
{
    public Buff _buff;
    public override void Effect(CharacterBase useCharacter, CharacterBase targetCharacter, int count)
    {
        Debug.Log(count);
        for (var i = 0; i < count; i++)
        {
            Debug.Log("バフの追加");
            targetCharacter._buff.Add(_buff);
        }
    }
}

[CreateAssetMenu(fileName = "OneBuffCard", menuName = "CardEffect/OneTimeBuffCard")]
public class OneTimeBuffCard : CardUseEffect
{
    public Buff _buff;
    public override void Effect(CharacterBase useCharacter, CharacterBase targetCharacter, int count)
    {
        for (var i = 0; i < count; i++)
        {
            targetCharacter._oneTimeBuff.Add(_buff);
        }
    }
}

[CreateAssetMenu(fileName = "Debuff", menuName = "CardEffect/DebuffCard")]
public class DebuffCard : CardUseEffect
{
    public Buff _debuff;
    public override bool ChoiseTarget()
    {
        Debug.Log("敵を選べ");
        target = BattleManager.Instance.GetMousePositionEnemy<CharacterBase>();
        return Input.GetMouseButton(0) && target != null;
    }
    public override void Effect(CharacterBase useCharacter, CharacterBase targetCharacter, int count)
    {
        targetCharacter._buff.Add(_debuff);
    }
}
