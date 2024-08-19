using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] GameObject _nextStepButton;
    [SerializeField] CharacterBase[] _characterBasesTest;//�e�X�g�p
    [SerializeField] CharacterBase _player;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    List<Queue<CardBase>> _enemyDeck = new List<Queue<CardBase>>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    [SerializeField] bool Testmode;
    Trun _trun;
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
    private void Awake()
    {
        if (FindObjectOfType<BattleManager>() != null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (Testmode)
        {
            SetData(_characterBasesTest);
        }
    }
    void ChangeTrun()
    {
        switch (CurrentTurn)
        {
            case Trun.Start:
                StartEffect();
                //�o�g���X�^�[�g���̉��o������B
                break;
            case Trun.Draw:
                int num = GameObject.FindObjectsOfType<CardBase>().Length;
                int drawCount = _firstDraw - num;
                DrawCard(drawCount);
                break;
            case Trun.ChoiseUseCard:
                ChoiseUseCard();
                break;
            case Trun.UseCard:
                UseCard();
                break;
            case Trun.PlayerAttack:
                //PlayerAttack();
                Debug.Log("�G��I��");
                ;
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                break;
            case Trun.EndTrun:
                EndTrun();
                break;
            case Trun.Result:
                //result����
                Result();
                break;
        }
    }
    void Update()
    {
        if (CurrentTurn == Trun.PlayerAttack)
        {
            PlayerAttack();
        }
    }
    void PlayerAttack()
    {
        switch (_player._attackPattern)
        {
            case AttackPattern.All:
                foreach (var enemy in _enemyList)
                {
                    _player.Attack(enemy);
                    if (enemy._hp <= 0)
                    {
                        _enemyList.Remove(enemy);
                        Debug.Log(enemy + "��|����");
                    }
                }
                if (_enemyList.Count != 0)
                {
                    NextTrun(Trun.EnemyAttack, 3);
                }
                else
                {
                    Victory();
                }
                break;
            default:
                if (Input.GetMouseButtonDown(0))
                {
                    var enemy = GetMouseClickEnemy();
                    if (enemy != null)
                    {
                        _player.Attack(enemy);
                        if (enemy._hp <= 0)
                        {
                            _enemyList.Remove(enemy);
                            Debug.Log(enemy + "��|����");
                        }
                        if (_enemyList.Count != 0)
                        {
                            NextTrun(Trun.EnemyAttack, 3);
                        }
                        else
                        {
                            Victory();
                        }
                    }
                }
                break;
        }
    }
    public void SetData(CharacterBase[] enemyArray)
    {
        Debug.Log("�Q�[���X�^�[�g");
        this.gameObject.SetActive(true);
        if (_playerDeck != null) _playerDeck = DeckShuffle(_player?._deck);
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyList?.Add(enemy);
            _enemyDeck?.Add(DeckShuffle(enemy?._deck));
        }
        NextTrun(Trun.Start, 1);
    }
    void StartEffect()
    {
        Debug.Log("�^�[���J�n");
        List<CharacterBase> enemyList = new List<CharacterBase>(_enemyList);
        foreach (CharacterBase enemy in enemyList)
        {
            if (enemy._hp <= 0)
            {
                _enemyList.Remove(enemy);
            }
        }
        if (_enemyList.Count <= 0)
        {
            NextTrun(Trun.Result, 1);
        }
        else
        {
            NextTrun(Trun.Draw, 1);
        }
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
        Debug.Log("�J�[�h�h���[");
        NextTrun(Trun.ChoiseUseCard, 1);
    }
    void ChoiseUseCard()
    {
        NextTrun(Trun.UseCard, 3);
    }
    void UseCard()
    {
        CardBase[] cards = _waitingCardListParent.GetComponentsInChildren<CardBase>();
        List<CardBase> debuffCard = new List<CardBase>();
        foreach (var playCard in cards)
        {
            if (playCard._isBuff == BuffDebuff.Debuff)
            {
                debuffCard.Add(playCard);
                //playCard.CardUse(_player,)
            }
            else
            {
                playCard.CardUse(_player, null);
                Debug.Log("�J�[�h�g�p");
            }
        }
        StartCoroutine(UseDebuffCrad(debuffCard, _player));//�R���[�`�����Ń^�[�����Ǘ�����
    }
    void EnemyAttack()
    {
        Debug.Log("�G�̍U��");
        for (int i = 0; i < _enemyList.Count; i++)
        {
            CharacterBase attackObj=_player;
            foreach(var enemy in _enemyList[i]._debuff)
            {
                //�����f�o�t�̖��O�����܂莟�揑�������܂�
                if (1 == 0)
                {
                    var random =Random.Range(0, _enemyList.Count);
                    attackObj = _enemyList[random];
                }
            }
            _enemyDeck[i].Dequeue().CardUse(_enemyList[i], attackObj);
        }
        if (_player._hp <= 0)
        {
            Defeat();
        }
        NextTrun(Trun.EndTrun, 3);
    }
    void Victory()
    {
        Debug.Log("victory");
        NextTrun(Trun.Result, 2);
    }
    void Defeat()
    {
        Debug.Log("defeat");
        NextTrun(Trun.Result, 2);
    }
    void EndTrun()
    {
        Debug.Log("�^�[���I��");
        NextTrun(Trun.Start, 0);
    }
    void Result()
    {
        if (!Testmode)
        {
            GameManager.Instance.BattleEnd();
        }
        gameObject.SetActive(false);

    }
    public void AddNewEnemy(CharacterBase enemy)
    {
        _enemyList.Add(enemy);
    }
    Queue<T> DeckShuffle<T>(List<T> DeckData)
    {
        List<T> cardsList = new List<T>();//player�̃J�[�h���X�g�i���z�j
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
    IEnumerator UseDebuffCrad(List<CardBase> debufCardList, CharacterBase useCharacter)
    {
        int count = 0;
        Debug.Log("�f�o�t�Ώۂ�I��ł�������");
        while (count < debufCardList.Count)
        {
            CharacterBase enemy = null;
            if (Input.GetMouseButton(0))
            {
                enemy = GetMouseClickEnemy();
                if (enemy != null)
                {
                    Debug.Log($"{enemy}�Ƀf�o�t��������");
                    count++;
                    debufCardList[count].CardUse(useCharacter, enemy);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (_nextStepButton == null)
        {
            NextTrun(Trun.PlayerAttack, 0);
        }
    }
    void NextTrun(Trun trunName, float waiteTimer)
    {
        //StopCoroutine(NextTrunCoroutine(trunName, waiteTimer));
        StartCoroutine(NextTrunCoroutine(trunName, waiteTimer));
    }
    IEnumerator NextTrunCoroutine(Trun trunName, float waiteTimer)
    {
        Debug.Log($"TrunChange{trunName}");
        yield return new WaitForSeconds(waiteTimer);
        if (CurrentTurn != Trun.Result)
        {
            CurrentTurn = trunName;
        }
    }
    public void NextTrun(string trunName)//eventTrigger�p
    {
        Debug.Log($"TrunChange{trunName}");
        CurrentTurn = (Trun)Enum.Parse(typeof(Trun), trunName);
    }
}

public enum Trun
{
    None,
    Start,
    Draw,
    ChoiseUseCard,
    UseCard,
    PlayerAttack,
    EnemyAttack,
    EndTrun,
    Result
}