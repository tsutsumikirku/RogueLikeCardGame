using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IUseCard
{
    public bool ChoiseTarget()
    {
        return true;
    }

    public void Effect(CharacterBase useCharacter)
    {
        useCharacter.Attack(BattleManager.Instance._player);
    }
}
