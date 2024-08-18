using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour
{
    public BuffDebuff _isBuff;
    [SerializeField] Buff _buff;
    [SerializeField] int _attackCount = 1;
    [SerializeField] string _dictionary = "説明なし";
    public int _price = 1;
    public BuffDebuff CardUse(CharacterBase attack , CharacterBase beattacked)
    {
        Debug.Log($"カードを{attack}へ使用");
        CardUsing(attack,beattacked);
        CardUseEvent();
        return _isBuff;
    }
    public void CardUsing(CharacterBase attack,CharacterBase beattacked)
    {
        switch (_isBuff)
        {
            case BuffDebuff.OverRide:
            CardOverRide();
            break;
            case BuffDebuff.Attack:
            attack.Attack(beattacked);
            break;
            case BuffDebuff.Buff:
            attack._buff.Add(_buff);
            break;
            case BuffDebuff.OneTimeBuff:
            attack._oneTimeBuff.Add(_buff);
            break;
            case BuffDebuff.AllAttack:
            attack._attackPattern = AttackPattern.All;
            break;
            case BuffDebuff.AttackCount:
            attack._attackCount = _attackCount;
            break;
        }
    }
    public virtual void CardOverRide()
    {
        Debug.Log("オーバーライドされていません");
    }
    public virtual void CardUseEvent()
    {
        //Destroy(gameObject);
    }
}
public enum BuffDebuff
{
    OverRide,
    Attack,
    Buff,
    OneTimeBuff,
    AllAttack,
    AttackCount,
    Debuff
}
public enum DebuffSelectState
{
    None,
    SelectBefore,
    SelectAfter
}
