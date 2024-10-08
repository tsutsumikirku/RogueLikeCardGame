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
            //return(T)this;なぜこれでエラー吐くのかわからない
            return effect;
        }
        return default;
    }
}
