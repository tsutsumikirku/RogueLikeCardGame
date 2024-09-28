using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Store : MonoBehaviour
{
    public static Store Instance;
    [SerializeField] GameObject _tablePrefab;
    [SerializeField] int _prizeCardCount;
    [SerializeField] public int _maxChildElement;
    [SerializeField] StorePrefabData _storeCanvasPrefab; //ストアprefab
    StorePrefabData _storeCanvas;//ストアprefabのinstans情報
    [SerializeField] public CardBaseArray _cardArray; //報酬のtable
    [SerializeField] List<CardBase> _prizeCards = new List<CardBase>();//報酬の量//デバック用にシリアライズしてます。
    bool _buyCards;
    //カードがDestroyされたときの挙動用
    //List<Transform> _buyCardsTransform = new List<Transform>();
    //List<Transform> _buyTablesTransform = new List<Transform>();
    //List<Transform> _sellCardsTransform = new List<Transform>();
    //List<Transform> _sellTablesTransform = new List<Transform>();
    //下記のシリアライズはデバッグ用です。シリアライズだけ削除してください
    CharacterBase _player;
    [SerializeField] List<CardBase> _cards;
    //[SerializeField] Dictionary<> //ランダムの内容によって使うかも
    //デバッグようの変数
    [SerializeField] bool _test;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()//スタートには何も書かない
    {
        if (_test)
        {
            SetPrizeCard(_cardArray);
            StoreStart();
        }
    }
    private void SetPrizeCard(CardBaseArray cardBaseArray)
    {
        _prizeCards.Clear();
        _cardArray = cardBaseArray;
        var cardList = ShuffleList(_cardArray.Cards.ToList());
        Debug.Log($"{cardList.Count} {_prizeCards}");
        for (var i = 0; i < _prizeCardCount; i++)
        {
            Debug.Log(cardList[i]);
            _prizeCards.Add(cardList[i]);
        }
    }
    public void StoreStart()
    {
        gameObject.SetActive(true);
        _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        _storeCanvas = Instantiate(_storeCanvasPrefab);
        _storeCanvas._BuyPanel.gameObject.SetActive(false);
        _storeCanvas._SellPanel.gameObject.SetActive(false);
        CreatCards(_prizeCards, _storeCanvas._BuyTableParent, _prizeCardCount, true);
        CreatCards(_player._deck, _storeCanvas._SellTableParent, _player._deck.Count, false);
    }
    void CreatCards(List<CardBase> cards, Transform parent, int count, bool buyMode)
    {
        if (_maxChildElement <= 0)
        {
            Debug.LogError("_maxChildElementがゼロです。");
            return;
        }
        //if (buyMode)
        //{
        //    _buyCardsTransform.Clear();
        //    _buyTablesTransform.Clear();
        //}
        //else
        //{
        //    _sellCardsTransform.Clear();
        //    _sellTablesTransform.Clear();
        //}
        float num = (float)cards.Count / _maxChildElement;
        int createCardCount = 0;
        for (var i = 0; i < num; i++)
        {
            var table = Instantiate(_tablePrefab, parent);
            //if (buyMode) _buyTablesTransform.Add(table.transform);
            //else _sellTablesTransform.Add(table.transform);
            for (int j = 0; j < _maxChildElement && createCardCount < count; j++)
            {
                Debug.Log(cards[createCardCount]);
                var obj = cards[createCardCount];
                var card = Instantiate(cards[createCardCount].gameObject, table.transform);
                if (buyMode)
                {
                    //_buyCardsTransform.Add(card.transform);
                    SetEventTrigger(card, () =>
                    {
                        BuyCardOnClick(obj);
                        Destroy(card);
                        //CardParentResetStart();
                    });
                }
                else
                {
                    //_sellCardsTransform.Add(card.transform);
                    SetEventTrigger(card, () =>
                    {
                        if (_player._deck.Count > Setting.Instans._minDeckRange)
                        {
                            SellCardOnClick(obj);
                            Destroy(card);
                            //CardParentResetStart();
                        }
                        else Debug.Log("それ以上は売れません");
                    });
                }
                createCardCount++;
            }
        }
    }
    //void CardParentResetStart()
    //{
    //    StartCoroutine(CardParentReset(_sellCardsTransform));

    //}
    //IEnumerator CardParentReset(List<Transform> _cardTransform)
    //{
    //    yield return null;
    //    Debug.Log("開始");
    //    foreach (var transform in _cardTransform)
    //    {
    //        for (var i = 0; i < _sellTablesTransform.Count; i++)
    //        {
    //            if (_sellTablesTransform[i].childCount < _maxChildElement)
    //            {
    //                Debug.Log("Set");
    //                transform.SetParent(_sellTablesTransform[i]);
    //                break;
    //            }
    //        }
    //    }
    //}
    public void CreatPlayerDeckCard(Transform parent)
    {
        CreatCards(_player._deck, parent, _player._deck.Count, false);
    }
    void SetEventTrigger(GameObject card, Action action)
    {
        EventTrigger trigger = card.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = card.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { action(); });
        trigger.triggers.Add(entry);
    }
    public void BuyCardOnClick(CardBase card)
    {
        Debug.Log("Buy");
        _player._deck.Add(card);
        Debug.Log(card.cardData._price);
        _player._hp -= card.cardData._price;
    }
    public void SellCardOnClick(CardBase card)
    {
        Debug.Log("sell");
        _player._deck.Remove(card);
        _player._hp += card.cardData._price;
    }
    public List<T> ShuffleList<T>(List<T> DeckData)//バトルマネージャーにも同じメソッドがある　木目
    {
        List<T> cardsList = new List<T>(DeckData);//playerのカードリスト(コピー)
        for (int i = 0; i < DeckData.Count; i++)
        {
            var j = Random.Range(0, DeckData.Count);
            var temp = cardsList[i];
            cardsList[i] = cardsList[j];
            cardsList[j] = temp;
        }
        return cardsList;
    }
}
