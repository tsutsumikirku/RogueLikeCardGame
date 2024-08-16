using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData",menuName = "MyGame/Creat EnemyData")]
public class EnemyData : ScriptableObject
{
    public List<Data> _enemyList=new List<Data>();
}
[System.Serializable]
public class Data
{
    public string _name;
    public int _maxHp;
    public List<CardBase> _cardData = new List<CardBase>();
}