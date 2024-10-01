using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [HideInInspector] public AttackPattern _attackPattern = AttackPattern.Single;
    [HideInInspector] public int _attackCount = 1;
    public List<CardBase> _deck;
    public string _name;
    public float _hp;
    public float _attackpower;
    public Dictionary<Buff, float> _buff = new Dictionary<Buff, float>() { {Buff.Red,0},{Buff.Green,0},{ Buff.Blue, 0 },
                                            {Buff.NowRed, 0},{Buff.NowGreen,0},{Buff.NowBlue,0},{Buff.OllEnemyAttack,0},{Buff.OllElementAttack,0},
                                            {Buff.Debuff,0}};
    public Buff _characterBuff;//キャラクターの属性値メインプレイヤーの属性はNoneの想定です
    public void Attack(CharacterBase enemy)
    {
        float buffUp = 0;
        float debuffUp = 0;
        if (_buff[Buff.OllEnemyAttack] >= 1)
        {
            buffUp = _buff[Buff.Red] + _buff[Buff.Green] + _buff[Buff.Blue] + _buff[Buff.NowRed] + _buff[Buff.NowGreen] + _buff[Buff.NowBlue];
        }
        else if (_buff[Buff.OllElementAttack] <= 1)
        {
            switch (enemy._characterBuff)
            {
                case Buff.Red:
                    break;
                case Buff.Green:
                    break;
                case Buff.Blue:
                    break;
            }
        }
        _buff[Buff.NowRed] = 0;
        _buff[Buff.NowGreen] = 0;
        _buff[Buff.NowBlue] = 0;
        _buff[Buff.OllElementAttack] = 0;
        _buff[Buff.OllEnemyAttack] = 0;
    }
    public void AddBuffDictionary(Buff key, float value)
    {

    }
    public void BuffReset()
    {
        _buff[Buff.Red] = 0;
        _buff[Buff.Green] = 0;
        _buff[Buff.Blue] = 0;
    }
}
public enum AttackPattern
{
    Single,
    All
}
public enum Buff
{
    None,
    Red,
    Green,
    Blue,
    NowRed,
    NowGreen,
    NowBlue,
    OllEnemyAttack,
    OllElementAttack,
    Debuff
}