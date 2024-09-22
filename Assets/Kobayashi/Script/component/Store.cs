using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Store : MonoBehaviour
{
    public static Store Instance;
    StoreMode _mode;
    StoreMode _storeMode
    {
        get { return _mode; }
        set
        {
            if (_mode != value)
            {
                _mode = value;
                ChangeState();
            }
        }
    }
    [SerializeField] CardBase[] cards;
    [SerializeField] int _cardCount;
    [SerializeField] GameObject _enptyCardPrefab;
    [SerializeField] GameObject[] _storePanels;
    [SerializeField] GameObject[] _tableArray;
    bool _buyCords;
    //���L�̃V���A���C�Y�̓f�o�b�O�p�ł��B�V���A���C�Y�����폜���Ă�������
    [SerializeField] CharacterBase _player;
    [SerializeField] List<CardBase> _cards;
    //[SerializeField] Dictionary<> //�����_���̓��e�ɂ���Ďg������
    //�f�o�b�O�悤�̕ϐ�
    [SerializeField] bool _test;
    [SerializeField] CardDataScriptablObj _dataScriptablObj;
    //��{�I��EventSystem�ŌĂԁBdeck���m�F���錄�Ԃ����
    private void Awake()
    {
        Instance = this;
    }
    private void Start()//�X�^�[�g�ɂ͉��������Ȃ�
    {
        if (_test)
        {
            StoreStart();
        }
    }
    void StoreStart()
    {
        _storeMode = StoreMode.StoreMain;
        gameObject.SetActive(true);
        _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        _cards = _player?._deck;
        if (!_buyCords)
        {
            _buyCords = true;
            var count = GameObject.FindObjectsOfType<StoreCard>().Length;
            //CreatCards(RandomCard(_dataScriptablObj.Cards, _cardCount - count));
        }
    }
    void StoreEnd()
    {
        gameObject.SetActive(false);
        _storeMode = StoreMode.None;
        GameManager.Instance.BattleEnd();
    }
    void ChangeState()
    {
        switch (_storeMode)
        {
            case StoreMode.StoreSell:
                StoreModeActive("SellPanel");
                break;
            case StoreMode.StoreBuy:
                StoreModeActive("BuyPanel");
                break;
            case StoreMode.StoreMain:
                StoreModeActive("MainPanel");
                break;
            default:
                break;
        }
    }
    void StoreModeActive(string name)
    {
        foreach (var panel in _storePanels)
        {
            if (panel.name == name)
            {
                panel.SetActive(true);
            }
            else panel.SetActive(false);
        }
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
            int plus1 = 0;
            if (num2 > 0)
            {
                plus1 = 1;
                num2--;
            }
            for (int i = 0; i < num + plus1; i++)
            {
                var cardObj = Instantiate(_enptyCardPrefab, table.transform);
                cardObj.GetComponent<StoreCard>().CardData = cards[i];
                Debug.Log(cards[i]);
                Debug.Log(cardObj.GetComponent<StoreCard>().CardData);
                var enptyCard = cardObj.AddComponent<CardBase>();//�\�����ʋ���
                enptyCard = cards[i];
            }
        }
    }
    public void StateChange(string enumName)
    {
        Debug.Log(_storeMode);
        _storeMode = (StoreMode)Enum.Parse(typeof(StoreMode), enumName);
    }
    public void CardOnClick(CardBase card)
    {
        if (_storeMode == StoreMode.StoreBuy)
        {
            Debug.Log("Buy");
            _player._deck.Add(card);
            //Debug.Log(card._price);
            //_player._hp -= card._price;
        }
        else if (_storeMode == StoreMode.StoreSell)
        {
            Debug.Log("sell");
            _player._deck.Remove(card);
            //_player._hp += card._price;
        }
        else
        {
            Debug.LogError("�J�[�h�����݂��Ȃ��͂��f�X�B");
        }
    }
    void AllCardDestroy()
    {
        var cards = GameObject.FindObjectsOfType<StoreCard>();
        foreach (var card in cards)
        {
            Destroy(card);
        }
    }

    enum StoreMode
    {
        None,
        StoreMain,
        StoreBuy,
        StoreSell,
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
