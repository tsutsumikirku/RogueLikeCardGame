using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffCard : IUseCard
{
    public Buff _debuff;
    [SerializeField] int _cardStats;
    CharacterBase _target;
    public bool ChoiseTarget()
    {
        Debug.Log("“G‚ð‘I‚×");
        _target = BattleManager.Instance.GetMousePositionEnemy<CharacterBase>();
        return Input.GetMouseButton(0) && _target != null;
    }
    public void Effect(CharacterBase useCharacter)
    {
        for (var i = 0; i < _cardStats; i++)
        {
            _target._buff.Add(_debuff);
        }
    }
}
