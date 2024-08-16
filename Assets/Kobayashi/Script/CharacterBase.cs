using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public string _name;
    public float _hp;

    public List<CardBase> _cards;
    public List<Buff> _buff;
    abstract public void CardExecution(CardBase card, GameObject attackObject);
    virtual public void SetStatus(Data enemydata)
    {
        _name = enemydata._name;
        _hp = enemydata._maxHp;
        _cards = enemydata._cardData;
    }
    public void SetBuff(Buff buff, int count)
    {
        _buff.Add(buff);
        Debug.Log($"{buff}が追加された");
        if (count > 1)
        {
            SetBuff(buff, count - 1);
        }
    }
    public float Damage(float damage)
    {
        int damageBuff=0;
        int damageDebuff = 0;
        foreach (var buff in _buff)
        {
            switch (buff)
            {
                case Buff.DamageBuff:
                    damageBuff++;
                    break;
                case Buff.DamageDebuff:
                    damageDebuff++;
                    break;
                case Buff.All:

                    break;
            }
        }
        _hp -= damage + damageBuff-damageDebuff;
        Debug.Log($"攻撃後HP{_hp}ダメージ{damage + damageBuff-damageDebuff}");
        if (_hp < 0)
        {
            Debug.Log("死亡した");
        }
        return damage;
    }
   
}
public enum Buff
{
    DamageBuff,
    DamageDebuff,
    All
}
