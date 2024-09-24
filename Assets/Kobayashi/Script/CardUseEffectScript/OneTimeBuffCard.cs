using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeBuffCard : IUseCard
{
    public Buff _buff;
    [SerializeField] int _stats;
    public bool ChoiseTarget()
    {
        return true;
    }
    public void Effect(CharacterBase useCharacter)
    {
        for (var i = 0; i < _stats; i++)
        {
            useCharacter._oneTimeBuff.Add(_buff);
        }
    }
}