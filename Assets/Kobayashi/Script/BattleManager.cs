using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] CharacterBase[] _characterBasesTest;//�e�X�g�p
    [SerializeField] CharacterBase _player;
    [SerializeField] GameObject _stateEndButton;
    [SerializeField] Vector2 _stateEndButtonAnchor;
    [SerializeField] Canvas _resultCanvas;
    List<CharacterBase> _enemyList = new List<CharacterBase>();
    List<Queue<CardBase>> _enemyDeck = new List<Queue<CardBase>>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    List<CardBase> _trashZone=new List<CardBase>();
    Trun _trun;
    Dictionary<Action, Func<bool>> _cardPlayQueue=new Dictionary<Action, Func<bool>>();
    [SerializeField]GameObject _resultPanel;
    [SerializeField] bool Testmode;
    [SerializeField] GameObject _emptyCard;
    [SerializeField] CardBase[] _praiseCardList;
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
                //�{�^���Ŏ���state��
            case Trun.UseCard:
                UseCard();
                break;
                //�{�^���Ŏ���state��
            case Trun.PlayerAttackTargetSelection:
                Debug.Log("�G��I��");
                ;// PlayerAttackTargetSelection();
                break;
                //�{�^���Ŏ���state��
            case Trun.PlayerAttack:
                ;//PlayerAttack
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                break;
            case Trun.EndTrun:
                EndTrun();
                break;
            case Trun.Result:
                //result����
                Result(3);
                break;
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
    void Update()
    {
        if (CurrentTurn == Trun.PlayerAttackTargetSelection)
        {
            PlayerAttackTargetSelection();
        }
    }
    void PlayerAttackTargetSelection()
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
                    NextTrun(Trun.PlayerAttack, 3);
                }
                else
                {
                    Victory();
                }
                break;
            default:
                if (Input.GetMouseButtonDown(0))
                {
                    var enemy = GetMousePositionEnemy<CharacterBase>();
                    if (enemy != null)
                    {
                        NextTrun(Trun.PlayerAttack, 1);
                        PlayerAttack(enemy);
                    }
                }
                break;
        }
    }
    public void SetData(CharacterBase[] enemyArray)
    {
        Debug.Log("�Q�[���X�^�[�g");
        var playerObj = GameObject.FindWithTag("Player");
        _player = playerObj.GetComponent<CharacterBase>();
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
            if (_playerDeck.Count <= 0)
            {
                _playerDeck = DeckShuffle(_trashZone);
            }
        }
        Debug.Log("�J�[�h�h���[");
        NextTrun(Trun.ChoiseUseCard, 1);
    }
    void ChoiseUseCard()
    {
        Debug.Log("�J�[�h�I��");
        GameObject obj = Instantiate(_stateEndButton, _stateEndButtonAnchor, Quaternion.identity,_resultCanvas.transform);
        obj.TryGetComponent(out Button button);
        if (button == null)
        {
            obj.AddComponent<Button>();
        }
        button.onClick.AddListener(()=>NextTrun(Trun.UseCard,0));
    }
    void UseCard()
    {
        CardBase[] cards = _waitingCardListParent.GetComponentsInChildren<CardBase>();
        List<CardBase> debuffCard = new List<CardBase>();
        foreach (var playCard in cards)
        {
            _trashZone.Add(playCard);
            if (playCard._isBuff == BuffDebuff.Debuff)
            {
                debuffCard.Add(playCard);
            }
            else
            {
                playCard.CardUse(_player, null);
                Debug.Log("�J�[�h�g�p");
            }
        }
        StartCoroutine(UseDebuffCrad(debuffCard, _player));//�R���[�`�����ŃX�e�[�g���Ǘ�����
        //StartCoroutine(CallBack(_cardPlayQueue));
    }
    IEnumerator CallBack(Dictionary<Action,Func<bool>> taskList)//�J�[�h�̑ҋ@
    {
        foreach(var task in taskList)
        {
            Debug.Log("�^�X�N�J�n");
            task.Key.Invoke();
            yield return null;
            yield return new WaitUntil(task.Value);
            Debug.Log("�^�X�N�I��");
        }
    }
    void PlayerAttack(CharacterBase enemy)
    {
        _player.Attack(enemy);
        if (enemy._hp <= 0)
        {
            _enemyList.Remove(enemy);
            Debug.Log(enemy + "��|����");
            if (_enemyList.Count == 0)
            {
                Victory();
            }
            else
            {
                for(int i = 0; i < _player._buff.Count; i++)
                {
                    //�o�t�T��
                }
            }
            NextTrun(CurrentTurn++, 3);
        }
    }
    void EnemyAttack()
    {
        Debug.Log("�G�̍U��");
        for (int i = 0; i < _enemyList.Count; i++)
        {
            CharacterBase attackObj = _player;
            foreach (var enemy in _enemyList[i]._debuff)
            {
                //�����f�o�t�̖��O�����܂莟�揑�������܂�
                if (1 == 0)
                {
                    var random = Random.Range(0, _enemyList.Count);
                    attackObj = _enemyList[random];
                }
            }
            _enemyDeck[i].Dequeue().CardUse(_enemyList[i], attackObj);
            if (_enemyDeck[i].Count == 0)
            {
                _enemyDeck[i] = DeckShuffle(_enemyList[i]._deck);
            }
        }
        if (_player._hp <= 0)
        {
            Defeat();
        }
        NextTrun(Trun.EndTrun, 3);
    }
    void EndTrun()
    {
        Debug.Log("�^�[���I��");
        NextTrun(Trun.Start, 2);
    }
    void Victory()
    {
        Debug.Log("victory");
        NextTrun(Trun.Result, 2);
    }
    void Defeat()
    {
        Debug.Log("defeat");
    }
    void Result(int count)//
    {
        Debug.Log("risult");
        var cards=RandomCard(_praiseCardList, count);
        var result = Instantiate(_resultPanel,_resultCanvas.transform);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("�J�[�h�ǉ�");
            var obj = Instantiate(_emptyCard,result.transform.GetChild(0));
            // EventTrigger �R���|�[�l���g���擾�܂��͒ǉ�
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.AddComponent<EventTrigger>();
            }

            // PointerClick �C�x���g�p�̃G���g�����쐬
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            // �C�x���g���Ɏ��s����R�[���o�b�N��ǉ�
            entry.callback.AddListener((data) => { OnClick(); });

            // Entry �� EventTrigger �ɒǉ�
            trigger.triggers.Add(entry);
            Debug.Log("Evenntotuika");
        }
    }
    void OnClick()//result�ł̕�V�I��p�̃��\�b�h
    {
        Debug.Log("click���ꂽ "+this.gameObject.name);
        //�A�j���[�V�������΁A�v���C���[�̃J�[�h���X�g�ɓo�^�Aresult��ʂ����
    }
    void BattleEnd()
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
    [return: MaybeNull]
    public T GetMousePositionEnemy<T>()
    {

        T hitGameObject;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (hit.transform != null)
        {
            hitGameObject = hit.transform.gameObject.GetComponent<T>();
            if (hitGameObject != null)
            {
                Debug.Log(hitGameObject);
                return hitGameObject;
            }
        }
        Debug.Log("raycast=null");
        return default;
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
                enemy = GetMousePositionEnemy<CharacterBase>();
                if (enemy != null)
                {
                    Debug.Log($"{enemy}�Ƀf�o�t��������");
                    count++;
                    debufCardList[count].CardUse(useCharacter, enemy);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (_stateEndButton == null)
        {
            NextTrun(Trun.PlayerAttack, 0);
        }
    }
    T[] RandomCard<T>(T[] randomizeArray,int count)
    {
        if (randomizeArray.Length < count)
        {
            return randomizeArray;
        }
        else
        {
            T[] randomCopy = new T[randomizeArray.Length];
            randomCopy = randomizeArray;
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                var random = Random.Range(0, randomizeArray.Length - i);
                result[i] = randomCopy[random];
                randomCopy[random] = randomCopy[randomCopy.Length-1 - i];
            }
            return result;
        }
    }
    public void SetCardUse(Action action,Func<bool> endConditions)
    {
        _cardPlayQueue.Add(action, endConditions);
    }
    void NextTrun(Trun trunName, float waiteTimer)
    {
        //StopCoroutine(NextTrunCoroutine(trunName, waiteTimer));
        StartCoroutine(NextTrunCoroutine(trunName, waiteTimer));
    }
    IEnumerator NextTrunCoroutine(Trun trunName, float waiteTimer)
    {
        yield return new WaitForSeconds(waiteTimer);
        if (CurrentTurn != Trun.Result)
        {
            CurrentTurn = trunName;
        }
    }
    public void NextTrun(string trunName)//eventTrigger�p
    {
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
    PlayerAttackTargetSelection,
    PlayerAttack,
    EnemyAttack,
    EndTrun,
    Result
}