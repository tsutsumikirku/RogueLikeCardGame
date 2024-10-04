using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEnemyCard : IUseEffect
{
    [SerializeField] CharacterBase _enemy;
    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        BattleManager.Instance.AddNewEnemy(_enemy);
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if (this is T effect)
        {
            return effect;
        }
        return default;
    }
}
