using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuffCard : IUseEffect
{
    public Buff _buff;
    [SerializeField] int _stats;
    public void Effect(CharacterBase useCharacter,CharacterBase target)
    {
        for (var i = 0; i < _stats; i++)
        {
            Debug.Log("�o�t�̒ǉ�");
            useCharacter._buff.Add(_buff);
        }
    }
}
public class doubleAttack : IUseEffect
{
    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        throw new System.NotImplementedException();
    }
}
