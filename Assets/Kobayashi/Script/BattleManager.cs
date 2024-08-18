using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] CharacterBase[] _characterBases;
    [SerializeField] CharacterBase _player;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    List<Queue<CardBase>> _enemyDeck = new List<Queue<CardBase>>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    Trun _trun=Trun.None;
    Trun CurrentTurn
    {
        get
        {
            return _trun;
        }
        set
        {
            if (_trun != value)
            {
                _trun = value;
                ChangeTrun();
            }
        }
    }
    void ChangeTrun()
    {
        switch (CurrentTurn)
        {
            case Trun.Start:
                StartTest();
                CardBase._isPlayer = false;
                //バトルスタート時の演出がいる。
                break;
            case Trun.Draw:
                int num = GameObject.FindObjectsOfType<CardBase>().Length;
                int drawCount=_firstDraw - num;
                DrawCard(drawCount);
                break;
            case Trun.UseCard:
                UseCard();
                CardBase._isPlayer = true;
                break;
            case Trun.PlayerAttack:
                CardBase._isPlayer = false;
                //PlayerAttack();
                Debug.Log("敵を選べ");
                ;
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                break;
            case Trun.EndTrun:
                EndTrun();
                break;
            case Trun.Result:
                //result処理
                break;
        }
    }
    void Start()
    {
        CurrentTurn = Trun.Start;
        //BattelStart(_characterBases);
    }
    void Update()
    {
        if (_trun==Trun.PlayerAttack)
        {
            switch (_player._attackPattern)
            {
                case AttackPattern.All:
                    foreach (var enemy in _enemyList)
                    {
                        _player.Attack(enemy);
                        StartCoroutine(NextTrun(Trun.EnemyAttack, 3));
                    }
                    break;
                default:
                    if (Input.GetMouseButtonDown(0))
                    {
                        var enemy = GetMouseClickEnemy();
                        if (enemy != null)
                        {
                            _player.Attack(enemy);
                            StartCoroutine(NextTrun(Trun.EnemyAttack,3));
                        }
                    }
                    break;
            }
            if (_enemyList == null)
            {
                Victory();
            }
        }
    }
    void StartTest()
    {
        BattelStart(_characterBases);
        Debug.Log("ゲームスタート");
    }
    void BattelStart(CharacterBase[] enemyArray)
    {
        if (_playerDeck != null) _playerDeck = DeckShuffle(_player?._deck);
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyList?.Add(enemy);
            _enemyDeck?.Add(DeckShuffle(enemy?._deck));
        }
        StartCoroutine(NextTrun(Trun.Draw, 1));
    }
    void DrawCard(int DrawCount)
    {
        for (int i = 0; i < DrawCount; i++)
        {
            Instantiate(_playerDeck.Dequeue());
            if (_playerDeck.Count >= 0)
            {
                _playerDeck = DeckShuffle(_player._deck);
            }
        }
        Debug.Log("カードドロー");
        StartCoroutine(NextTrun(Trun.UseCard, 1));
    }
    void UseCard()
    {
        CardBase[] cards = _waitingCardListParent.GetComponentsInChildren<CardBase>();
        foreach (var playCard in cards)
        {
            playCard.CardUse(_player);
        }
        Debug.Log("カード使用");
        StartCoroutine(NextTrun(Trun.PlayerAttack, 3));
    }
    void EnemyAttack()
    {
        Debug.Log("敵の攻撃");
        foreach (var enemyDeck in _enemyDeck)
        {
            enemyDeck.Dequeue().CardUse(_player);
        }
        if (_player._hp <= 0)
        {
            Defeat();
        }
        StartCoroutine(NextTrun(Trun.EndTrun, 3));
    }
    void EndTrun()
    {
        Debug.Log("ターン終了");
        StartCoroutine(NextTrun(Trun.Start, 3));
    }
    void Victory()
    {
        Debug.Log("victory");
        CurrentTurn=Trun.Result;
    }
    void Defeat()
    {
        Debug.Log("defeat");
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
        }
        foreach (T Card in cardsList)
        {
            deckQueue.Enqueue(Card);
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
        Debug.Log("raycast=null");
        return null;
    }
    IEnumerator NextTrun(Trun trunName,float waiteTimer)//あんまり使わないかも
    {
        yield return new WaitForSeconds(waiteTimer);
        CurrentTurn = trunName;
        Debug.Log($"TrunChange{trunName}");
    }
    void NextTrun(Trun trunName)//アニメーションやイベントトリガーなどで呼ぶよう
    {
        CurrentTurn = trunName;
        Debug.Log($"TrunChange{trunName}");
    }
}

public enum Trun
{
    None,
    Start,
    Draw,
    UseCard,
    PlayerAttack,
    EnemyAttack,
    EndTrun,
    Result
}
