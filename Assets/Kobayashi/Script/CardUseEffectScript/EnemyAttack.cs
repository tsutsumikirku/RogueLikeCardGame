using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : IUseEffect
{

    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        useCharacter.Attack();
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if(this is T effect)
        {
            //return(T)this;Ç»Ç∫Ç±ÇÍÇ≈ÉGÉâÅ[ìfÇ≠ÇÃÇ©ÇÌÇ©ÇÁÇ»Ç¢
            return effect;
        }
        return default;
    }
}
