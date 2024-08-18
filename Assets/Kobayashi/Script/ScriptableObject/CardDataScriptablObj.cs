using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "MyGame/Creat CardData")]
public class CardDataScriptablObj : ScriptableObject
{
    public List<CardBase> cards=new List<CardBase>();
}
