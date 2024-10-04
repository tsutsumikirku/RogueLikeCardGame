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
    private CharacterBase _attackEnemy;
    CharaBaseState _state;

    public void Attack(CharacterBase enemy)
    {
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
        _buff[Buff.NowRed] = 0;
        _buff[Buff.NowGreen] = 0;
        _buff[Buff.NowBlue] = 0;
        _buff[Buff.OllElementAttack] = 0;
        _buff[Buff.OllEnemyAttack] = 0;
    }
    void OnSelect()
    {
        
    }
    void OnAttack()
    {
        if (_buff[Buff.OllEnemyAttack] >= 1)
        {
            EnemyBase[] atkEnemy = GameObject.FindObjectsOfType<EnemyBase>();
            for(int i = 0; i < atkEnemy.Length; i++)
            {
                
            }
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
        Camera camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 10f);
        if (hit.collider != null && hit.collider.tag == "enemy")
        {
            _attackEnemy = hit.collider.GetComponent<CharacterBase>();
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