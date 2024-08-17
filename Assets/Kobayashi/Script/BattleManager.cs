using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    public static BattleManager _instance;
    bool _EndTarn;
    Trun _trun;
    Trun _nextTrun;
    CharacterBase _player;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    void Start()
    {

    }
    void Update()
    {
        switch (_trun)
        {
            case Trun.Start:
                SetData();
                _trun = Trun.None;
                break;
            case Trun.ChoiceCard:
                DrawCard(_firstDraw);
                _trun = Trun.None;
                break;
            case Trun.UseCard:
                UseCard();
                _trun = Trun.None;
                break;
            case Trun.PlayerAttack:
                PlayerAttack();
                _trun = Trun.None;
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                _trun = Trun.None;
                break;
            default:
                if (_EndTarn)
                {
                    NextTrun(_nextTrun);
                }
                break;
        }
    }
    void NextTrun(Trun trun)
    {
        _EndTarn = false;
        _trun = trun;
    }
    public void SetData()
    {
        CharacterBase[] character = GameObject.FindObjectsOfType<CharacterBase>();
        foreach (var chara in character)
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
        _playerDeck = DeckShuffle(_player._deck);
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
            playCard.CardUse();
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
        foreach (var Card in DeckData)
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
        }
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
    EnemyAttack
}
