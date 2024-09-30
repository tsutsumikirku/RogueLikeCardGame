using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeBuffCard : IUseEffect
{
    public Buff _buff;
    public int _stats;
    public void Effect(CharacterBase useCharacter,CharacterBase target)
    {
        for (var i = 0; i < _stats; i++)
        {
            target._oneTimeBuff.Add(_buff);
        }
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if(this is T effect)
        {
            return effect;
        }
        return default;
    }
}