using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public static Store Instance;
    [SerializeField] CardBase[] cards;
    [SerializeField] GameObject _storePanel;
    [SerializeField] int _cardCount;
    [SerializeField] GameObject _enptyCardPrefab;
    [SerializeField] GameObject[] _tableArray;
    //下記のシリアライズはデバッグ用です。シリアライズだけ削除してください
    [SerializeField] CharacterBase _player;
    [SerializeField] List<CardBase> _cards;
    //[SerializeField] Dictionary<> //ランダムの内容によって使うかも
    //デバッグようの変数
    [SerializeField] bool _test;
    [SerializeField] CardDataScriptablObj _dataScriptablObj;
    //基本的にEventSystemで呼ぶ。deckを確認する隙間を作る
    private void Awake()
    {
        Instance = this;
    }
    private void Start()//スタートには何も書かない
    {
        if (_test)
        {
            StoreActive();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    void StoreActive()
    {
        gameObject.SetActive(true);
        _storePanel.SetActive(true);
        _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        _cards = _player?._deck;
        CreatCards(RandomCard(_dataScriptablObj.Cards, 6));
    }
    CardBase[] RandomCard(List<CardBase> cards, int count)
    {
        CardBase[] result = new CardBase[count];
        for (var i = 0; i < count; i++)
        {
            int num = Random.Range(0, cards.Count);
            result[i] = cards[num];
        }
        return result;
    }
    void CreatCards(CardBase[] cards)
    {
        var num = _cardCount / _tableArray.Length;
        var num2 = _cardCount % _tableArray.Length;
        foreach (var table in _tableArray)
        {
            int plus1=0;
            if (num2 > 0)
            {
                plus1 = 1;
                num2--;
            }
            for (int i = 0; i < num+plus1; i++)
            {
                var cardObj=Instantiate(_enptyCardPrefab, table.transform);
                var enptyCard = cardObj.GetComponent<CardBase>();
                enptyCard = cards[i];
            }
        }
    }
    public void Buy(CardBase card)
    {
        Debug.Log("Buy");
        _player._deck.Add(card);
        _player._hp -= card._price;
    }
    HashSet<CardBase> RandomCardHashSet(List<CardBase> cards, int count)
    {
        HashSet<CardBase> result = new HashSet<CardBase>();
        for (var i = 0; i < count; i++)
        {
            int num = Random.Range(0, cards.Count);
            result.Add(cards[num]);
        }
        return result;
    }
}
