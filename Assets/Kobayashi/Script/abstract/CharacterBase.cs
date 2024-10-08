using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [HideInInspector] public AttackPattern _attackPattern = AttackPattern.Single;
    [HideInInspector] public int _attackCount = 1;
    [SerializeField, Tooltip("プレイヤーの場合true、敵の場合はfalse")] bool _isPlayer;
    public List<CardBase> _deck;
    public string _name;
    public float _hp;
    public float _attackpower;
    public Dictionary<Buff, float> _buff = new Dictionary<Buff, float>() { {Buff.Red,0},{Buff.Green,0},{ Buff.Blue, 0 },
                                            {Buff.NowRed, 0},{Buff.NowGreen,0},{Buff.NowBlue,0},{Buff.OllEnemyAttack,0},{Buff.OllElementAttack,0},
                                            {Buff.Debuff,0}};
    public Buff _characterBuff;//キャラクターの属性値メインプレイヤーの属性はNoneの想定です 
    public Buff _characterNowBuff;
    private CharacterBase _attackEnemy;
    CharaBaseState _state;
    Action _idleMethod;

    private void Start()
    {
        switch (_characterBuff)
        {
            case Buff.Red:
            _characterNowBuff = Buff.NowRed;
                break;
            case Buff.Green:
            _characterNowBuff = Buff.NowGreen;
                break;
            case Buff.Blue:
            _characterNowBuff = Buff.NowBlue;
                break;
        }
    }
    public void Attack(Action idleMethod)
    {
        _idleMethod = idleMethod;
        if (_isPlayer && _buff[Buff.OllEnemyAttack] < 1)
        {
            State = CharaBaseState.Select;
        }
        else
        {
            State = CharaBaseState.Attack;
        }
    }
    public void AddBuffDictionary(Buff key, float value)
    {
        _buff[key] += value;
    }
    public void BuffReset()
    {
        _buff[Buff.Red] = 0;
        _buff[Buff.Green] = 0;
        _buff[Buff.Blue] = 0;
    }
    public CharaBaseState State
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
    void OnStateChanged()
    {
        switch (_state)
        {
            case CharaBaseState.Idle:
                OnIdle();
                break;
            case CharaBaseState.Select:
                OnSelect();
                break;
            case CharaBaseState.Attack:
                OnAttack();
                break;       
        }
    }
    void OnIdle()
    {
        _idleMethod();
        _attackEnemy = null;
        _buff[Buff.NowRed] = 0;
        _buff[Buff.NowGreen] = 0;
        _buff[Buff.NowBlue] = 0;
        _buff[Buff.OllElementAttack] = 0;
        _buff[Buff.OllEnemyAttack] = 0;
    }
    void OnSelect()
    {
        SelectEffect.Instance.SelectModeStart();
    }
    void OnAttack()
    {
        if (_isPlayer)
        {
            SelectEffect.Instance.SelectModeEnd();
            if (_buff[Buff.OllEnemyAttack] >= 1)
            {
                CharacterBase[] atkEnemy = Array.ConvertAll(GameObject.FindGameObjectsWithTag("Enemy"),x => x.GetComponent<CharacterBase>());
                for (int i = 0; i < atkEnemy.Length; i++)
                {
                    if (_buff[Buff.OllElementAttack] >= 1)
                    {
                        atkEnemy[i]._hp -= (1 - _buff[Buff.Debuff]) * (_attackpower + _buff[Buff.Red] + _buff[Buff.Green] + _buff[Buff.Blue]) * (_buff[Buff.NowRed] + _buff[Buff.NowGreen] + _buff[Buff.NowBlue] + 1);
                    }
                    else
                    {
                        atkEnemy[i]._hp -= (1 - _buff[Buff.Debuff]) * (_attackpower + _buff[atkEnemy[i]._characterBuff]) * (_buff[atkEnemy[i]._characterNowBuff] + 1);
                    }
                    if (atkEnemy[i]._hp <= 0)
                    {
                        Destroy(atkEnemy[i].gameObject);
                    }
                }
            }
            else
            {
                if (_buff[Buff.OllElementAttack] >= 1)
                {
                    _attackEnemy._hp -= (1 - _buff[Buff.Debuff]) * (_attackpower + _buff[Buff.Red] + _buff[Buff.Green] + _buff[Buff.Blue]) * (_buff[Buff.NowRed] + _buff[Buff.NowGreen] + _buff[Buff.NowBlue] + 1);
                }
                else
                {
                    _attackEnemy._hp -= (1 - _buff[Buff.Debuff]) * (_attackpower + _buff[_attackEnemy._characterBuff]) * (_buff[_attackEnemy._characterNowBuff] + 1);
                }
                if(_attackEnemy._hp <= 0)
                {
                    Destroy(_attackEnemy.gameObject);
                }
            }
            State = CharaBaseState.Idle;
        }
        else
        {
            _attackEnemy = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
            _attackEnemy._hp -= _attackpower * (1 - _buff[Buff.Debuff]);
            if(_attackEnemy._hp <= 0)
            {
                Destroy(_attackEnemy.gameObject);
            }
            State = CharaBaseState.Idle;
        }
        
    }
    public void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && _state == CharaBaseState.Select)
        {
            RayCast();
        }
    }
    void RayCast()
    {
        Camera camera = Camera.main;
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.tag == "Enemy")
        {
            _attackEnemy = hit.collider.GetComponent<CharacterBase>();
            State = CharaBaseState.Attack;
        }
        else
        {
            Debug.Log("No enemy hits on Raycast : レイキャストに敵がヒットしていません");
        }
    }
    public void SelectButton()
    {
        State = CharaBaseState.Attack;
    }
}
public enum AttackPattern
{
    Single,
    All
}
public enum CharaBaseState
{
    Idle,
    Select,
    Attack
}
public enum Buff
{
    None,
    Red,
    Green,
    Blue,
    NowRed,
    NowGreen,
    NowBlue,
    OllEnemyAttack,
    OllElementAttack,
    Debuff
}