using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashZone : MonoBehaviour
{
    public List<CardBase> Cards = new List<CardBase>();
    private void Update()
    {
        Cards.ForEach((card) => card.gameObject.SetActive(false));
    }
}
