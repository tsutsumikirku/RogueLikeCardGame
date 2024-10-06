using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardManager : MonoBehaviour
{
    public static BattleCardManager instance;
    Transform _deck;
    Transform _trashZone;
    Transform _desk;
    [SerializeField] BattleCanvasChildData _childData;
    [SerializeField] Text _deckChildCount;
    List<DragAndDrop> _allDragCard = new List<DragAndDrop>();
    public Dictionary<CardBase, CardPos> _playerCards = new Dictionary<CardBase, CardPos>();
    Dictionary<CardPos, Transform> _cardParent = new Dictionary<CardPos, Transform>();
    int _playerDeckCount;
    private void Awake()
    {
        instance = this;
    }
    private void LateUpdate()
    {
        if (_deckChildCount) _deckChildCount.text = _deck.GetComponentsInChildren<DragAndDrop>().Length.ToString();
        if (BattleManager.Instance._player._deck.Count != _playerDeckCount && GameManager.Instance.State == GameManagerState.Move) 
        {
            if (BattleManager.Instance._player._deck.Count < _playerDeckCount)
            {
                var list = new List<string>();
                foreach(var card in _playerCards.Keys)
                {

                }
                foreach(var card in BattleManager.Instance._player._deck)
                {
                    
                }
            }
            else
            {

            }
            _playerDeckCount = BattleManager.Instance._player._deck.Count;
        }
    }
    public CardBase[] CardCreatSeting(List<CardBase> cards)
    {
        _deck = _childData._deck;
        _trashZone = _childData._trashParent;
        _desk = _childData._handCard;
        _cardParent.Add(CardPos.Deck, _deck.transform);
        _cardParent.Add(CardPos.TrashZone, _trashZone.transform);
        _cardParent.Add(CardPos.Desk, _desk.transform);
        _playerDeckCount = BattleManager.Instance._player._deck.Count;
        foreach (CardBase obj in cards)
        {
            Debug.Log(obj);
            Debug.Log(_deck);
            var card = Instantiate(obj, _deck.transform);
            if (!card.GetComponent<DragAndDrop>()) card.AddComponent<DragAndDrop>();
            card.transform.position = _deck.transform.position;
            _playerCards.Add(card, CardPos.Deck);
            if (card.TryGetComponent(out DragAndDrop drag))
            {
                _allDragCard.Add(drag);
                drag._isuse = false;
            }
            else Debug.LogError($"ドラッグアンドドロップを持たないカードがあります。{card}");
        }
        return _playerCards.Keys.ToArray();
    }
    public void AllCardDragMode(bool mode)
    {
        _allDragCard.ForEach(card => card._isuse = mode);
    }
    public List<CardBase> TrashZoneReset()
    {
        List<Transform> cards = new List<Transform>();
        for (var i = 0; i < _trashZone.transform.childCount; i++)
        {
            cards.Add(_trashZone.transform.GetChild(i));
            //_playerCards[card] = CardPos.Deck;
        }
        List<CardBase> cardBases = new List<CardBase>();
        cards.ForEach(card =>
        {
            card.SetParent(_deck.transform);
            cardBases.Add(card.GetComponent<CardBase>());
        });
        return cardBases;
    }
    public void ChangeCardParent(CardBase card, CardPos pos)
    {
        card.transform.SetParent(_cardParent[pos]);
    }
    public void AddCard(CardBase card, CardPos pos)
    {
        _playerCards.Add(card, pos);
    }
    public void DestroyCard(CardBase card)
    {
        //_playerCards.Remove(card);
    }
}
public enum CardPos
{
    Deck,
    Desk,
    TrashZone,
}
