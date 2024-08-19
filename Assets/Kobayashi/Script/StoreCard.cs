using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCard : CardBase
{
    public void CallStoreBuy()
    {
        Store.Instance.Buy(GetComponent<StoreCard>());
        Destroy(gameObject);
    }
}
