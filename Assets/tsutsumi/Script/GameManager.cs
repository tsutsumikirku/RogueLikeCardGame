using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("探索画面がスタートしたときにやること")] UnityEvent _serchStart;
    [SerializeField, Tooltip("移動のときにやること")] UnityEvent _moveStart;
    [SerializeField,Tooltip("ショップ画面に行くときにやること")]UnityEvent _shopStart;
    [SerializeField, Tooltip("デッキ画面に行くときにやること")] UnityEvent _deckStart;
    [SerializeField, Tooltip("操作説明画面に行くときにやること")] UnityEvent _userManualStart;
    [SerializeField, Tooltip("次の段階に進むためのターン数")] int _stepTurnCount = 10;
    [SerializeField, Tooltip("ターン数に応じた敵の数")] int[] _steps = new[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3 };
    [SerializeField, Tooltip("敵キャラクターの位置を敵の最大値の想定された形で配置してください")] Transform [] _enemyTransform;
    [SerializeField, Tooltip("宝箱の位置を指定してください")] Transform _tresureBoxTransform;
    [SerializeField, Tooltip("ボスの位置を指定してください")] Transform _bossTransform;
    int _turnCount;
    public static GameManager Instance;
    GameManagerState _state;
    GameManagerBattleStep _gameBattleStep;
    bool _boss = false;
    bool _afterBoss = false;
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
        if(_move == GameManagerMove.Battle)
        {
           CharacterBase[]enemy = new CharacterBase[_steps[_turnCount]];
           for(int i =0; i < _steps[_turnCount]; i++)
            {
               
                switch (_gameBattleStep)
                {
                     case GameManagerBattleStep.First:
                     enemy[i] = Instantiate(EnemyData.Instance._FirstStep[RandomEnemySet(EnemyData.Instance._FirstStep.Count)]);
                     enemy[i].transform.position = _enemyTransform[i].position;
                     break;
                     case GameManagerBattleStep.Second:
                     enemy[i] = Instantiate(EnemyData.Instance._SecondStep[RandomEnemySet(EnemyData.Instance._SecondStep.Count)]);
                     enemy[i].transform.position = _enemyTransform[i].position;
                     break;
                     case GameManagerBattleStep.Third:
                     enemy[i] = Instantiate(EnemyData.Instance._TherdStep[RandomEnemySet(EnemyData.Instance._TherdStep.Count)]);
                     enemy[i].transform.position = _enemyTransform[i].position;
                     break;
                }
            }
            //ここでバトルマネージャーを呼び出す
            _turnCount++;
            if(_turnCount == _stepTurnCount)
            {
                if(_gameBattleStep > GameManagerBattleStep.Third)
                {
                    _turnCount = 0;
                    _gameBattleStep += 1;
                }
                else
                {
                    _boss = true;
                }
            }
        }
        else if(_move == GameManagerMove.TresureBox)
        {
            CharacterBase tresurebox = Instantiate(EnemyData.Instance._TresureBox[RandomEnemySet(EnemyData.Instance._TresureBox.Count)]);
            tresurebox.transform.position = _tresureBoxTransform.transform.position;
            //ここでバトルマネージャーを呼び出す
            _turnCount++;
            if (_turnCount == _stepTurnCount)
            {
                if (_gameBattleStep > GameManagerBattleStep.Third)
                {
                    _turnCount = 0;
                    _gameBattleStep += 1;
                }
                else
                {
                    _boss = true;
                }
            }
        }
        else if(_move == GameManagerMove.Boss)
        {
            _afterBoss = true;
            CharacterBase boss = Instantiate(EnemyData.Instance.Boss[RandomEnemySet(EnemyData.Instance.Boss.Count)]);
            boss.transform.position = _bossTransform.transform.position;
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
        int random = Random.Range(0, max + 1);
        return random;
    }
    public void BattleEnd()
    {
        State = GameManagerState.Serch;
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
public enum GameManagerBattleStep
{
    First,
    Second,
    Third
}
