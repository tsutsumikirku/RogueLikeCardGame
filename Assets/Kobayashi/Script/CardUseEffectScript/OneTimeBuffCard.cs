using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeBuffCard : IUseEffect
{
    public Buff _buff;
    [SerializeField] int _stats;
    public void Effect(CharacterBase useCharacter,CharacterBase target)
    {
        for (var i = 0; i < _stats; i++)
        {
            target._oneTimeBuff.Add(_buff);
        }
    }
}