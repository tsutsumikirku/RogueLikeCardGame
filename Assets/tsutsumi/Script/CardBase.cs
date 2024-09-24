using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [SerializeField] Sprite _sprite;
    [SerializeField] Text _nameText;
    [SerializeField] Text _explanationText;
    [SerializeField] CardData _cardData;
    [SerializeReference, SubclassSelector] IUseCard _cardEffect;
    [SerializeReference, SubclassSelector] IUseCard[] _cardEffects;
    public CardData CardData
    {
        get => _cardData;
        set
        {
            _cardData = value;
            _sprite = value._cardSpite;
            _explanationText.text = value._explanation;
            _explanationText.text = value._explanation;
        }
    }
    public IEnumerator CardUse(CharacterBase useCharacter, Action animationEndAction)
    {
        float timer = 0;
        yield return new WaitUntil(() => _cardEffect.ChoiseTarget());
        _cardEffect.Effect(useCharacter);
        var animator = useCharacter.GetComponent<Animator>();
        CardUseEvent();
        if (animator == null)
        {
            animationEndAction();
            yield break;
        }
        animator.Play(CardData._animationName);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                Debug.Log("èIóπ");
                animationEndAction();
                break;
            }
        }
    }
    public virtual void CardUseEvent()
    {
        transform.SetParent(BattleManager.Instance._trashParent);
    }
}
interface IUseCard
{
    public void Effect(CharacterBase useCharacter);
    public bool ChoiseTarget();
}

