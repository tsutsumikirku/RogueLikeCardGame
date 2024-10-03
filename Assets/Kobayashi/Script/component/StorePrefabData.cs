using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePrefabData : MonoBehaviour
{
    public Image _mainePanel;
    public Image _BuyPanel;
    public Transform _BuyTableParent;
    public Image _SellPanel;
    public Transform _SellTableParent;
    public void StoreEnd()
    {
        Store.Instance.StoreEnd();
        Destroy(gameObject);
    }
    public void CreatPlayerCards()
    {
        for (int i = 0; i < _SellTableParent.childCount; i++)
        {
            Destroy(_SellTableParent.GetChild(i).gameObject);
        }
        Store.Instance.CreatPlayerDeckCard(_SellTableParent);
    }
}
