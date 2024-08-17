using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] CharacterBase[] _characterBases;
    public static BattleManager _instance;
    bool _EndTarn;
    Trun _nextTrun;
    [SerializeField]CharacterBase _player;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    List<Queue<CardBase>> _enemyDeck = new List<Queue<CardBase>>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    Trun _trun;
    Trun CurrentTurn
    {
        get 
        { 
            return _trun; 
        }
        set
        { 
            if(_trun != value)
            {
                _trun = value;
                ChangeTrun();
            }
        }
    }
    void Start()
    {
        CurrentTurn=Trun.Start;
        //BattelStart(_characterBases);
    }
    void Update()
    {
        
    }
    void ChangeTrun()
    {
        switch (CurrentTurn)
        {
            case Trun.Start:
                StartTest();
                //バトルスタート時の演出がいる。
                break;
            case Trun.ChoiceCard:
                DrawCard(_firstDraw);
                break;
            case Trun.UseCard:
                UseCard();
                break;
            case Trun.PlayerAttack:
                PlayerAttack();
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                break;
            case Trun.Result:
                //result処理
                break;
        }
    }
    void StartTest()
    {
        BattelStart(_characterBases);
    }
    void BattelStart(CharacterBase[] enemyArray)
    {
        SetData(enemyArray);
    }
    public void SetData(CharacterBase[] enemyArray)
    {
        if(_playerDeck!=null)_playerDeck = DeckShuffle(_player?._deck);
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyList.Add(enemy);
            _enemyDeck.Add(DeckShuffle(enemy._deck));
        }
    }
    void DrawCard(int DrawCount)
    {
        for (int i = 0; i < DrawCount; i++)
        {
            Instantiate(_playerDeck.Dequeue());
            Debug.Log(_playerDeck.Count);
            if (_playerDeck.Count >= 0)
            {
                _playerDeck = DeckShuffle(_player._deck);
            }
        }
    }
    void UseCard()
    {
        CardBase[] cards = _waitingCardListParent.GetComponentsInChildren<CardBase>();
        foreach (var playCard in cards)
        {
            playCard.CardUse(_player);
        }
    }
    IEnumerator PlayerAttack()
    {
        while (true)
        {
            switch (_player._attackPattern)
            {
                case AttackPattern.All:
                    foreach (var enemy in _enemyList)
                    {
                        _player.Attack(enemy);
                    }
                    break;
                default:
                    if (Input.GetMouseButtonDown(0))
                    {
                        _player.Attack(GetMouseClickEnemy());
                    }
                    break;
            }
            if (_enemyList == null)
            {
                Victory();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    void EnemyAttack()
    {
        foreach (var enemy in _enemyList)
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
    Queue<T> DeckShuffle<T>(List<T> DeckData)
    {
        List<T> cardsList = new List<T>();//playerのカードリスト（仮想）
        Queue<T> deckQueue = new Queue<T>();
        foreach (T Card in DeckData)
        {
            cardsList.Add(Card);
        }
        for (int i = 0; i < DeckData.Count; i++)
        {
            var j = Random.Range(0, DeckData.Count);
            var temp = cardsList[i];
            cardsList[i] = cardsList[j];
            cardsList[j] = temp;
            deckQueue.Enqueue(cardsList[i]);
            Debug.Log($"カードリスト{cardsList[i]}");
        }
        Debug.Log("end");
        return deckQueue;
    }
    CharacterBase GetMouseClickEnemy()
    {
#nullable enable
        CharacterBase? hitGameObject;
#nullable disable
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (hit.transform != null)
        {
            hitGameObject = hit.transform.gameObject.GetComponent<CharacterBase>();
            if (hitGameObject != null)
            {
                Debug.Log(hitGameObject);
                return hitGameObject;
            }
        }
        return null;
    }
}

public enum Trun
{
    None,
    Start,
    ChoiceCard,
    UseCard,
    PlayerAttack,
    EnemyAttack,
    Result
}
