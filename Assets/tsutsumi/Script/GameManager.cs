using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("探索画面がスタートしたときにやること")] UnityEvent _serchStart;
    [SerializeField, Tooltip("移動のときにやること")] UnityEvent _moveStart;
    [SerializeField, Tooltip("ショップ画面に行くときにやること")] UnityEvent _shopStart;
    [SerializeField, Tooltip("デッキ画面に行くときにやること")] UnityEvent _deckStart;
    [SerializeField, Tooltip("操作説明画面に行くときにやること")] UnityEvent _userManualStart;
    [SerializeField, Tooltip("敵キャラクターの位置を敵の最大値の想定された形で配置してください")] Transform[] _enemyTransform;
    [SerializeField, Tooltip("宝箱の位置を指定してください")] Transform _tresureBoxTransform;
    [SerializeField, Tooltip("ボスの位置を指定してください")] Transform _bossTransform;
    [SerializeField, Tooltip("一回のウェーブのターン数を指定してください")] int _waveTurnCount;
    [SerializeField] EnemyData _enemyData;
    [SerializeField] BattleManager _battleManager;
    [SerializeField,Tooltip("ボスが出現するターン")] int _bossTurn;
    [SerializeField] bool _test;
    [SerializeField,Tooltip("ターンのテキスト")] Text _turn;
    public int _waveCount;        
    public int _turnCount = 1;
    public static GameManager Instance;
    GameManagerState _state;
    bool _boss = false;
    bool _afterBoss = false;
    private void Awake()
    {
        if (FindObjectOfType<GameManager>() != null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameManagerState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChanged();
            }
        }
    }

    private void OnStateChanged()
    {
        _turn.text = _turnCount.ToString();
        switch (_state)
        {
            case GameManagerState.Serch:
                OnSerch();
                break;
            case GameManagerState.Move:
                OnMove();
                break;
            case GameManagerState.Shop:
                OnShop();
                break;
            case GameManagerState.Deck:
                OnDeck();
                break;
            case GameManagerState.UserManual:
                OnUserManual();
                break;
            case GameManagerState.GameOver:
                OnGameOver();
                break;
            case GameManagerState.GameEnd:
                OnGameEnd();
                break;
        }
    }

    void OnSerch()
    {
        _serchStart.Invoke();
        if (_afterBoss)
        {
            State = GameManagerState.GameEnd;
        }
    }

    void OnMove()
    {
        _moveStart.Invoke();
        GameManagerMove _move = RandomSetMap();

        if (_move == GameManagerMove.Battle)
        {
            Debug.Log("バトルに行きました");
            int random = Random.Range(1,_enemyTransform.Length);
            List<CharacterBase> _enemy = new List<CharacterBase>();
            for (int i = 0; i < random; i++)
            {
                _enemy.Add(Instantiate(_enemyData.Enemies[_waveCount]._character[Random.Range(0, _enemyData.Enemies[_waveCount]._character.Length)])); 
            }
            // BattleManager.Instance.BattleStart();
            _enemy.Clear();
            Debug.Log("敵データを送りました");
            _turnCount++;
            if(_turnCount >= _waveTurnCount)
            {
                _waveCount += 1;
                _turnCount = 1;
            }
            if (_test)
            {
                BattleEnd();
            }
            
        }
        else if (_move == GameManagerMove.TresureBox)
        {
            Debug.Log("宝箱に行きました");
            CharacterBase[] tresurebox = new CharacterBase[1];
            tresurebox[0] = Instantiate(_enemyData._TresureBox[RandomEnemySet(_enemyData._TresureBox.Count)]);
            tresurebox[0].transform.position = _tresureBoxTransform.position;
            //BattleManager.Instance.SetData(tresurebox);
            _turnCount++;
            if (_turnCount >= _waveTurnCount)
            {
                _waveCount += 1;
                _turnCount = 1;
            }
            if (_test)
            {
                BattleEnd();
            }
        }
        else if (_move == GameManagerMove.Boss)
        {
            _afterBoss = true;
            CharacterBase[] boss = new CharacterBase[1];
            boss[0] = Instantiate(_enemyData.Boss[RandomEnemySet(_enemyData.Boss.Count)]);
            boss[0].transform.position = _bossTransform.position;
            if (_test)
            {
                BattleEnd();
            }
            // BattleManager.Instance.SetData(boss);
            //ここでバトルマネージャーを呼び出す
        }
    }

    void OnShop()
    {
        _shopStart.Invoke();
    }

    void OnDeck()
    {
        _deckStart.Invoke();
    }

    void OnUserManual()
    {
        _userManualStart.Invoke();
    }

    void OnGameOver()
    {
        Debug.Log("死 death");
        //ゲームオーバーのシーンに行くんだったらここに書く
    }

    void OnGameEnd()
    {
        //ラスボスを倒したあとの処理をここに書く
    }

    GameManagerMove RandomSetMap()
    {
        if (!_boss)
        {
            int random = Random.Range(0, 2);
            return (GameManagerMove)random;
        }
        else
        {
            return GameManagerMove.Boss;
        }
    }

    int RandomEnemySet(int max)
    {
        return Random.Range(0, max);
    }

    public void BattleEnd()
    {
        Debug.Log("バトルが終わりサーチモードになりました");
        State = GameManagerState.Serch;
    }
    public void GameOver()
    {
        Debug.Log("ゲームオーバー");
        State = GameManagerState.GameOver;
    }

    //ここから下ボタンのための関数許して
    public void MoveButton()
    {
        State = GameManagerState.Move;
    }

    public void ShopButton()
    {
        State = GameManagerState.Shop;
    }

    public void DeckButton()
    {
        State = GameManagerState.Deck;
    }

    public void UserManual()
    {
        State = GameManagerState.UserManual;
    }
}

public enum GameManagerState
{
    Serch,
    Move,
    Shop,
    Deck,
    UserManual,
    GameOver,
    GameEnd
}

public enum GameManagerMove
{
    Battle,
    TresureBox,
    Boss
}
