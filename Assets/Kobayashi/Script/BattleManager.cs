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
            DrawCard();
            _turnChange = false;
        }
        else if (_trun == Trun.UseCard && _turnChange)
        {
            UseCard();
            NextTrun(Trun.Attack);
        }
        else if (_trun == Trun.Attack && _turnChange)
        {
            NextTrun(Trun.EnemyAttack);
        }
        else if (_trun == Trun.EnemyAttack && _turnChange)
        {
            NextTrun(Trun.ChoiceCard);
        }
    }
    void NextTrun(Trun trun)
    {
        _turnChange = true;
        _trun = trun;
    }
    void SetData()
    {
        CharacterBase[] character = GameObject.FindObjectsOfType<CharacterBase>();
        List<CardBase> playerCards=new List<CardBase>();//player�̃J�[�h���X�g�i���z�j
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
        foreach (var Card in _player._cards)
        {
            playerCards.Add(Card);
        }
        for(int i = 0; i < _player._cards.Count; i++)
        {
            var j = Random.Range(0, _player._cards.Count);
            var temp = playerCards[i];
            playerCards[i] = playerCards[j];
            playerCards[j] = temp;
            _playerDeck.Enqueue(playerCards[i]);
        }
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

    void Defeat()
    {

    }
    void Victory()
    {

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
