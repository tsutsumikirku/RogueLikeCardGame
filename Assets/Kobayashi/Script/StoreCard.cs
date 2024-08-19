using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCard : MonoBehaviour
{
    [HideInInspector]public CardBase CardData;
    public void OnClickCallStoreMethod()
    {
        Store.Instance.CardOnClick(CardData);
        Destroy(gameObject);
    }
}
