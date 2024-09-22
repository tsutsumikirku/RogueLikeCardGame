using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("�T����ʂ��X�^�[�g�����Ƃ��ɂ�邱��")] UnityEvent _serchStart;
    [SerializeField, Tooltip("�ړ��̂Ƃ��ɂ�邱��")] UnityEvent _moveStart;
    [SerializeField, Tooltip("�V���b�v��ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _shopStart;
    [SerializeField, Tooltip("�f�b�L��ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _deckStart;
    [SerializeField, Tooltip("���������ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _userManualStart;
    [SerializeField, Tooltip("���̒i�K�ɐi�ނ��߂̃^�[����")] int _stepTurnCount = 10;
    [SerializeField, Tooltip("�^�[�����ɉ������G�̐�")] int[] _steps = new[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3 };
    [SerializeField, Tooltip("�G�L�����N�^�[�̈ʒu��G�̍ő�l�̑z�肳�ꂽ�`�Ŕz�u���Ă�������")] Transform[] _enemyTransform;
    [SerializeField, Tooltip("�󔠂̈ʒu���w�肵�Ă�������")] Transform _tresureBoxTransform;
    [SerializeField, Tooltip("�{�X�̈ʒu���w�肵�Ă�������")] Transform _bossTransform;
    [SerializeField] EnemyData _enemyData;
    [SerializeField]BattleManager _battleManager;
    int _turnCount;
    public static GameManager Instance;
    GameManagerState _state;
    GameManagerBattleStep _gameBattleStep;
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
            Debug.Log("�o�g���ɍs���܂���");
            CharacterBase[] enemy = new CharacterBase[_steps[_turnCount]];

            for (int i = 0; i < _steps[_turnCount]; i++)
            {
                switch (_gameBattleStep)
                {
                    case GameManagerBattleStep.First:
                        enemy[i] = Instantiate(_enemyData._FirstStep[RandomEnemySet(_enemyData._FirstStep.Count)]);
                        break;
                    case GameManagerBattleStep.Second:
                        enemy[i] = Instantiate(_enemyData._SecondStep[RandomEnemySet(_enemyData._SecondStep.Count)]);
                        break;
                    case GameManagerBattleStep.Third:
                        enemy[i] = Instantiate(_enemyData._TherdStep[RandomEnemySet(_enemyData._TherdStep.Count)]);
                        break;
                }
                enemy[i].transform.position = _enemyTransform[i].position;
            }

            BattleManager.Instance.SetData(enemy);
            Debug.Log("�G�f�[�^�𑗂�܂���");
            _turnCount++;

            if (_turnCount >= _stepTurnCount)
            {
                if (_gameBattleStep < GameManagerBattleStep.Third)
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
        else if (_move == GameManagerMove.TresureBox)
        {
            Debug.Log("�󔠂ɍs���܂���");
            CharacterBase[] tresurebox = new CharacterBase[1];
            tresurebox[0] = Instantiate(_enemyData._TresureBox[RandomEnemySet(_enemyData._TresureBox.Count)]);
            tresurebox[0].transform.position = _tresureBoxTransform.position;

            BattleManager.Instance.SetData(tresurebox);
            _turnCount++;

            if (_turnCount >= _stepTurnCount)
            {
                if (_gameBattleStep < GameManagerBattleStep.Third)
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
        else if (_move == GameManagerMove.Boss)
        {
            _afterBoss = true;
            CharacterBase[] boss = new CharacterBase[1];
            boss[0] = Instantiate(_enemyData.Boss[RandomEnemySet(_enemyData.Boss.Count)]);
            boss[0].transform.position = _bossTransform.position;

            BattleManager.Instance.SetData(boss);
            //�����Ńo�g���}�l�[�W���[���Ăяo��
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
        Debug.Log("�� death");
        //�Q�[���I�[�o�[�̃V�[���ɍs���񂾂����炱���ɏ���
    }

    void OnGameEnd()
    {
        //���X�{�X��|�������Ƃ̏����������ɏ���
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
        Debug.Log("�o�g�����I���T�[�`���[�h�ɂȂ�܂���");
        State = GameManagerState.Serch;
    }

    //�������牺�{�^���̂��߂̊֐�������
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
