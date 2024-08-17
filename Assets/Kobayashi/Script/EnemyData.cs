using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData",menuName = "MyGame/Creat EnemyData")]
public class EnemyData : ScriptableObject
{
    public static EnemyData Instance;
    public List<Data> _FirstStep =new List<Data>();
    public List<Data> _SecondStep = new List<Data>();
    public List<Data> _TherdStep = new List<Data>();
    public List<Data> _TresureBox = new List<Data>();
}
[System.Serializable]
public class Data
{
    public string _name;
    public int _maxHp;
    public List<CardBase> _cardData = new List<CardBase>();
}