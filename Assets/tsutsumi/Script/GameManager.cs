using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Content;
using UnityEditor.SearchService;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("�T����ʂ��X�^�[�g�����Ƃ��ɂ�邱��")] UnityEvent _serchStart;
    [SerializeField, Tooltip("�ړ��̂Ƃ��ɂ�邱��")] UnityEvent _moveStart;
    [SerializeField, Tooltip("�V���b�v��ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _shopStart;
    [SerializeField, Tooltip("�f�b�L��ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _deckStart;
    [SerializeField, Tooltip("���������ʂɍs���Ƃ��ɂ�邱��")] UnityEvent _userManualStart;
    [SerializeField, Tooltip("�G�L�����N�^�[�̈ʒu��G�̍ő�l�̑z�肳�ꂽ�`�Ŕz�u���Ă�������")] Transform[] _enemyTransform;
    [SerializeField, Tooltip("�󔠂̈ʒu���w�肵�Ă�������")] Transform _tresureBoxTransform;
    [SerializeField, Tooltip("�{�X�̈ʒu���w�肵�Ă�������")] Transform _bossTransform;
    [SerializeField, Tooltip("���̃E�F�[�u�̃^�[�������w�肵�Ă�������")] int _waveTurnCount;
    [SerializeField] EnemyData _enemyData;
    [SerializeField] CardBaseArray _cards;
    [SerializeField] string _animationName;
    [SerializeField] string _gameOverSceneName;
    [SerializeField] string _gameClearSceneName;
    [SerializeField] Text _text;
    [SerializeField]Animator _animation;
    CharacterBase _player;
    public int _turnCount = 1;
    public int _phaseCount = 1;
    public static GameManager Instance;
    GameManagerState _state;
    bool _boss = false;
    bool _afterBoss = false;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _animation = GetComponent<Animator>();
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
        if (!_player)
        {
            _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        }
        _moveStart.Invoke();
        GameManagerMove _move = RandomSetMap();
        if(_phaseCount - 1 < _enemyData._enemies.Count)
        {
            if (_move == GameManagerMove.Battle)
            {
                Debug.Log("�o�g���ɍs���܂���");
                int random = Random.Range(1, _enemyTransform.Length);
                CharacterBase[] enemy = new CharacterBase[random];
                for (int i = 0; i < random; i++)
                {
                    enemy[i] = Instantiate(_enemyData._enemies[_phaseCount - 1]._character[Random.Range(0, _enemyData._enemies[_phaseCount - 1]._character.Length)]);
                }
                //StartCoroutine(AnimationExtension(_animationName, enemy));
                BattleManager.Instance.BattleStart(_player, enemy, _cards.Cards);
                Debug.Log("�G�f�[�^�𑗂�܂���");

            }
            else if (_move == GameManagerMove.TresureBox)
            {
                CharacterBase[] tresurebox = new CharacterBase[1];
                tresurebox[0] = Instantiate(_enemyData._tresureBox[RandomEnemySet(_enemyData._tresureBox.Count)]);
                tresurebox[0].transform.position = _tresureBoxTransform.position;
                //StartCoroutine(AnimationExtension(_animationName, tresurebox));
                BattleManager.Instance.BattleStart(_player, tresurebox, _cards.Cards);
                Debug.Log("�󔠂ɍs���܂���");
            }
        }
        else
        {
            CharacterBase[] boss = new CharacterBase[1];
            boss[0] = Instantiate(_enemyData._tresureBox[RandomEnemySet(_enemyData._tresureBox.Count)]);
            //StartCoroutine(AnimationExtension(_animationName, boss));
            BattleManager.Instance.BattleStart(_player, boss, _cards.Cards);
            Debug.Log("�{�X�ɍs���܂���");
        }
       
    }

    //IEnumerator AnimationExtension(string animationName, CharacterBase[] enemydata)
    //{
    //    if (_animation.GetCurrentAnimatorStateInfo(0).IsName(animationName))
    //    {
    //        yield return null;
    //    }
    //    BattleManager.Instance.BattleStart(_player, enemydata, _cards.Cards);
    //}

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
        SceneManager.LoadScene(_gameOverSceneName);
    }

    void OnGameEnd()
    {
        SceneManager.LoadScene(_gameClearSceneName);
        //���X�{�X��|�������Ƃ̏����������ɏ���
    }

    GameManagerMove RandomSetMap()
    {
            int random = Random.Range(0, 2);
            return (GameManagerMove)random;
    }

    int RandomEnemySet(int max)
    {
        return Random.Range(0, max);
    }

    public void BattleEnd()
    {
        Debug.Log("�o�g�����I���T�[�`���[�h�ɂȂ�܂���");
        _turnCount++;
        if (_turnCount > _waveTurnCount)
        {
            _phaseCount += 1;
            _turnCount = 1;
        }
        State = GameManagerState.Serch;
        _text.text = _turnCount.ToString();
    }
    public void ShopEnd()
    {
       // if (_state != GameManagerMove.) ;
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
