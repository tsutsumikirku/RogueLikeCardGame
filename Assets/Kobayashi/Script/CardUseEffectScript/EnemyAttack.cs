using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IUseEffect
{

    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        useCharacter.Attack(target);
    }
}
