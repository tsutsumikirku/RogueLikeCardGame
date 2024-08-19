using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "MyGame/Creat CardData")]
public class CardDataScriptablObj : ScriptableObject
{
    public List<CardBase> Cards=new List<CardBase>();
}
