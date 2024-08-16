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
    public CharacterBase character;
    public List<CardBase> _cardData = new List<CardBase>();
    public SpriteRenderer spriteRenderer;
}