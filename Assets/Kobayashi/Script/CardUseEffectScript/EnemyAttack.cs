using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : IUseEffect
{

    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        useCharacter.Attack(() => target=null);
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if(this is T effect)
        {
            //return(T)this;‚È‚º‚±‚ê‚ÅƒGƒ‰[“f‚­‚Ì‚©‚í‚©‚ç‚È‚¢
            return effect;
        }
        return default;
    }
}
