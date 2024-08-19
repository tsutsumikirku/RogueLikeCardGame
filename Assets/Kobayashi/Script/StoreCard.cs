using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCard : MonoBehaviour
{
    [SerializeField]Text _priceText;
    [SerializeField]Text _cardInstructionText;
    [HideInInspector]public CardBase CardData;
    Transform _canvas;
    private void Start()
    {
        _canvas = transform.root;
        var cardBase = GetComponent<CardBase>();
        _priceText.text = cardBase._price.ToString();
        _cardInstructionText.text = cardBase._dictionary;
    }
    public void CallStoreMethodOnClick()
    {
        Store.Instance.CardOnClick(CardData);
        Destroy(_cardInstructionText.gameObject);
        Destroy(gameObject);
    }
    public void PopOutText(bool popOut)
    {
        if(popOut)_cardInstructionText.transform.SetParent(transform);
        if (!popOut) _cardInstructionText.transform.SetParent(_canvas);
    }
}
