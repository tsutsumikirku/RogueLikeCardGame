using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("���̒i�K�ɐi�ނ��߂̃^�[����")] int _stepTurnCount = 10;
    [SerializeField, Tooltip("�^�[�����ɉ������G�̐�")] int[] _steps = new[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3 };
    [SerializeField,Tooltip("�L�����N�^�[�x�[�X�̕t�����x�[�X�I�u�W�F�N�g")] CharacterBase _base;
    [SerializeField, Tooltip("��������̃v���n�u�������ɓ���Ă�������")] GameObject _userManual;
    int _turnCount;
    public static GameManager Instance;
    GameManagerState _state;
    GameManagerBattleStep _gameBattleStep;
    bool _boss = false;

    private void Start()
    {
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
        }
    }
    void OnSerch()
    {
       
    }
    void OnMove()
    {
        GameManagerMove _move = RandomSetMap();
        if(_move == GameManagerMove.Battle)
        {
           CharacterBase[]enemy = new CharacterBase[_steps[_turnCount]];
           for(int i =0; i < _steps[_turnCount]; i++)
            {
                enemy[i] = (Instantiate(_base));
                if(_gameBattleStep == GameManagerBattleStep.First)
                {
                    enemy[i].SetStatus(EnemyData.Instance._FirstStep[RandomEnemySet(EnemyData.Instance._FirstStep.Count)]);
                }
                else if(_gameBattleStep == GameManagerBattleStep.Second)
                {
                    enemy[i].SetStatus(EnemyData.Instance._SecondStep[RandomEnemySet(EnemyData.Instance._SecondStep.Count)]);
                }
                else if(_gameBattleStep == GameManagerBattleStep.Third)
                {
                    enemy[i].SetStatus(EnemyData.Instance._TherdStep[RandomEnemySet(EnemyData.Instance._TherdStep.Count)]);
                }
            }
            //�����Ńo�g���}�l�[�W���[���Ăяo��
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
            CharacterBase tresurebox = Instantiate(_base);
            tresurebox.SetStatus(EnemyData.Instance._TresureBox[RandomEnemySet(EnemyData.Instance._TresureBox.Count)]);
            //�����Ńo�g���}�l�[�W���[���Ăяo��
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
            CharacterBase boss = Instantiate(_base);
            boss.SetStatus(EnemyData.Instance.Boss[RandomEnemySet(EnemyData.Instance.Boss.Count)]);
            //�����Ńo�g���}�l�[�W���[���Ăяo��
        }
    }
    void OnShop()
    {
    }
    void OnDeck()
    {
    }
    void OnUserManual()
    {
    }
    void OnGameOver()
    {
        Debug.Log("�� death");
        //�Q�[���I�[�o�[�̃V�[���ɍs���񂾂����炱���ɏ���
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
}
public enum GameManagerState
{
    Serch,
    Move,
    Shop,
    Deck,
    UserManual,
    GameOver
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
