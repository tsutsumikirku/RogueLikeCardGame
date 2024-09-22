using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoreCard : MonoBehaviour
{
    [SerializeField] Text _priceText;
    [SerializeField] Text _cardInstructionText;
    [HideInInspector] public CardBase CardData;
    Transform _canvas;
    private void Start()
    {
        _canvas = transform.root;
        var cardBase = GetComponent<CardBase>();
        //Debug.Log(cardBase._price);
        //_priceText.text = cardBase?._price.ToString();
        //_cardInstructionText.text = cardBase._dictionary;
    }
    public void CallStoreMethodOnClick()
    {
        Store.Instance.CardOnClick(CardData);
        Destroy(_cardInstructionText.gameObject);
        Destroy(gameObject);
    }
    public void PopOutText(bool popOut)
    {
        if (!popOut) StartCoroutine(Timer());
        if (popOut) _cardInstructionText.transform.SetParent(_canvas);
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        _cardInstructionText.transform.SetParent(transform);
    }
}
