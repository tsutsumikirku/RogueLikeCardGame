using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActiveFalseParent : MonoBehaviour, ICardDirective
{
    public List<CardBase> Cards = new List<CardBase>();
    public Vector2 GetPos()
    {
        return transform.position;
    }

    public void InParent(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OutParent(GameObject obj)
    {
        obj.SetActive(true);
    }
}
