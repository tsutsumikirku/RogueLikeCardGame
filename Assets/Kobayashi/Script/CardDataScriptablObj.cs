using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardList")]
public class CardDataScriptablObj : ScriptableObject
{
    public List<CardBase> Cards = new List<CardBase>();
}
[CreateAssetMenu(fileName = "CardData")]
public abstract class CardData : ScriptableObject
{
    [SerializeField] public Sprite _cardSpite;
    public string _cardName;
    public string _explanation;
    public int _price = 1;
    public int _cardPlayCount;
    public string _animationName;
}

