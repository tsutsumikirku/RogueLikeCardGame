using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitCardParent;
    public static BattleManager _instance;
    bool _turnChange;
    Trun _trun;
    CharacterBase _player;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    Queue<CardBase> _playerDeck = new Queue<CardBase> ();
    void Start()
    {

    }
    void Update()
    {
        if (_trun == Trun.Start)
        {
            SetData();
            NextTrun(Trun.ChoiceCard);
        }
        else if (_trun == Trun.ChoiceCard && _turnChange)
        {
            _turnChange = false;
            DrawCard();
        }
        else if (_trun == Trun.UseCard && _turnChange)
        {
            UseCard();
            NextTrun(Trun.Attack);
        }
        else if (_trun == Trun.Attack && _turnChange)
        {
            Attack();
            _turnChange = false;
        }
        else if (_trun == Trun.EnemyAttack && _turnChange)
        {
            EnemyAttack();
            NextTrun(Trun.ChoiceCard);
        }
    }
    void NextTrun(Trun trun)
    {
        _turnChange = true;
        _trun = trun;
    }
    public void SetData()
    {
        CharacterBase[] character = GameObject.FindObjectsOfType<CharacterBase>();
        foreach(var chara in character)
        {
            if (chara.gameObject.tag == "Player")
            {
                _player = chara;
            }
            else
            {
                _enemyList.Add(chara);
            }
        }
        DeckShuffle();
    }
    void DrawCard()
    {
        for(int i = 0; i < _firstDraw; i++)
        {
            Instantiate(_playerDeck.Dequeue());
        }
    }
    void UseCard()
    {
        CardBase[] cards = _waitCardParent.GetComponentsInChildren<CardBase>();
        foreach(var playCard in cards)
        {
            playCard.CardUse();
        }
    }
    void Attack()
    {
        if (_enemyList == null)
        {
            Victory();
        }
    }
    void EnemyAttack()
    {
        foreach(var enemy in _enemyList)
        {
            enemy.Attack(_player);
        }
        if (_player._hp <= 0)
        {
            Defeat();
        }
    }
    void Defeat()
    {
        Debug.Log("defeat");
    }
    void Victory()
    {
        Debug.Log("victory");
    }
    void DeckShuffle()
    {
        List<CardBase> playerCards = new List<CardBase>();//playerのカードリスト（仮想）
        foreach (var Card in _player._deck)
        {
            playerCards.Add(Card);
        }
        for (int i = 0; i < _player._deck.Count; i++)
        {
            var j = Random.Range(0, _player._deck.Count);
            var temp = playerCards[i];
            playerCards[i] = playerCards[j];
            playerCards[j] = temp;
            _playerDeck.Enqueue(playerCards[i]);
        }
    }
}
public enum Trun
{
    Start,
    ChoiceCard,
    UseCard,
    Attack,
    EnemyAttack
}
