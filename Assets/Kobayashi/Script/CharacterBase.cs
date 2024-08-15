using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public string _name;
    public float _hp;
    public List<CardBase> _cards;
    public List<Buff> _buff;
    abstract public void CardExecution(CardBase card,GameObject actionGameObject);
    virtual public void SetStatus(Data enemydata)
    {
        _name = enemydata._name;
        _hp = enemydata._maxHp;
        _cards = enemydata._cardData;
    }
    public void SetBuff(Buff buff,int count)
    {
        _buff.Add(buff);
        if (count > 0)
        {
            SetBuff(buff, count - 1);
        }
    }
    public float Damage()
    {
        foreach(var buff in _buff)
        {
            switch(buff){
                case Buff.DamageBuff:
                    
                    break;
                case Buff.All:

                    break;
            }
        }
        float damage=0;
        return damage;
    }
    public enum Buff
    {
        DamageBuff,
        All
    }
}
