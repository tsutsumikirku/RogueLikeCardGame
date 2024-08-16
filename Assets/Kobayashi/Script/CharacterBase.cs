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
        Debug.Log($"{buff}Ç™í«â¡Ç≥ÇÍÇΩ");
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
        Debug.Log($"çUåÇå„HP{_hp}É_ÉÅÅ[ÉW{damage + damageBuff-damageDebuff}");
        if (_hp < 0)
        {
            Debug.Log("éÄñSÇµÇΩ");
        }
        return damage;
    }
    public enum Buff
    {
        DamageBuff,
        DamageDebuff,
        All
    }
}
