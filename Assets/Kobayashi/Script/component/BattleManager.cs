using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] BattleCanvasChildData _battleCanvasChildData;
    Transform _waitingCardListParent;
    [HideInInspector]public Transform _trashParent;
    Transform _handCard;
    Transform _resultCanvas;
    [SerializeField] GameObject _resultPanel;
    [SerializeField] GameObject _stateEndButton;
    [SerializeField] Vector2 _stateEndButtonAnchor;
    [SerializeField] public SelectEffect _selectEffect;
    [HideInInspector] public CharacterBase _player;
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    List<CharacterBase> _playerAttackTarget = new List<CharacterBase>();
    public Dictionary<CharacterBase, Queue<CardBase>> _enemyDictionary = new Dictionary<CharacterBase, Queue<CardBase>>();
    BattleCardManager _cardManager;
    //�J�[�h�\�͗p�t���O
    [HideInInspector] public bool _doubleAttack;
    //��V�I�����������ǂ����̃t���O
    [HideInInspector] public bool _getReward;
    CardBase[] _praiseCardTable;//��V�̃e�[�u��
    Trun _trun;//�X�e�[�g�Ǘ�     CurrentTurn���炢����܂��B���ڂ������
    //���Ɏg���Ă���̂��킩��Ȃ����琮����
    [SerializeField] ButtleTimeLineManager _timeLine;
    //�f�o�b�O�p     �����l�̃e�X�g�Ɏg���Ă��邽�߃}�[�W�̎�������
    [SerializeField] CharacterBase[] _characterBasesTest;
    [SerializeField] CardBaseArray _CardDataScriptablObj;
    [SerializeField] bool Testmode;

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
                _cardManager.AllCardDragMode(true);
                int num = _handCard.childCount;
                int drawCount = _firstDraw - num;
                Debug.Log("��D��" + drawCount + "�����܂���");
                StartCoroutine(DrawCard(drawCount));
                break;
            case Trun.ChoiseUseCard:
                ChoiseUseCard();
                //�{�^���Ŏ���state��
                break;
            case Trun.UseCard:
                _cardManager.AllCardDragMode(false);
                UseCard();
                //�{�^���Ŏ���state��
                break;
            case Trun.PlayerAttackTargetSelection:
                Debug.Log("�G��I��");
                //�{�^���Ŏ���state��
                StartCoroutine(PlayerAttackTargetSelection());
                break;
            case Trun.PlayerAttack:
                StartCoroutine(PlayerAttack(_playerAttackTarget));
                break;
            case Trun.EnemyAttack:
                StartCoroutine(EnemyAttack());
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
            BattleStart(GameObject.FindWithTag("Player").GetComponent<CharacterBase>(), _characterBasesTest, _CardDataScriptablObj.Cards);
        }
    }
    /// <summary>
    /// �o�g���J�n���ɌĂԊ֐�
    /// </summary>
    /// <param name="player">����L����</param>
    /// <param name="enemyArray">�G�̔z��</param>
    /// <param name="prizeCards">��V�̃e�[�u��</param>
    public void BattleStart(CharacterBase player, CharacterBase[] enemyArray, CardBase[] prizeCards)
    {
        var canvas=Instantiate(_battleCanvasChildData);
        _trashParent = canvas._trashParent;
        _waitingCardListParent = canvas._waitingCardListParent;
        _handCard = canvas._handCard;
        _cardManager = canvas._cardManager;
        _resultCanvas = canvas.transform;
        //player�̃J�[�h�x�[�X�擾
        _player = player.gameObject.GetComponent<CharacterBase>();
        gameObject.SetActive(true);//�ꉞ
        _praiseCardTable = prizeCards;//��V�̐ݒ�
        _getReward = false;
        _doubleAttack=false;
        //�G�l�~�[�����Ȃ��ꍇ�����ɕ�V��ʂɐi�݃��\�b�h���I������
        if (enemyArray == null)
        {
            CurrentTurn = Trun.Result;
            return;
        }
        Debug.Log("�Q�[���X�^�[�g");
        var instansCardData = _cardManager.CardCreatSeting(_player._deck);
        _playerDeck = new Queue<CardBase>(ShuffleList(instansCardData.ToList()));
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyDictionary.Add(enemy, new Queue<CardBase>(ShuffleList(enemy._deck)));
        }
        NextTrun(Trun.Start, 1);
    }
    void StartEffect()
    {
        Debug.Log("�^�[���J�n");
        List<CharacterBase> enemyList = new List<CharacterBase>(_enemyDictionary.Keys);
        foreach (CharacterBase enemy in enemyList)
        {
            if (enemy._hp <= 0)
            {
                _enemyDictionary.Remove(enemy);
            }
        }
        if (_enemyDictionary.Count <= 0)
        {
            NextTrun(Trun.Result, 1);
        }
        else
        {
            NextTrun(Trun.Draw, 1);
        }
    }
    IEnumerator DrawCard(int DrawCount)
    {
        for (int i = 0; i < DrawCount; i++)
        {
            if (_playerDeck.Count <= 0)
            {
                Debug.Log("�̂ĎD���f�b�L�ɖ߂��܂���");
                var trashZone = _cardManager.TrashZoneReset();
                _playerDeck = new Queue<CardBase>(ShuffleList(trashZone));
            }
            yield return null;
            if (_playerDeck.TryDequeue(out CardBase card))
            {
                _cardManager.ChangeCardParent(card, CardPos.Desk);
            }
        }
        NextTrun(Trun.ChoiseUseCard, 1);
    }
    void ChoiseUseCard()
    {
        Debug.Log("�J�[�h�I��");
        CreatTrunChangeButton(() => NextTrun(Trun.UseCard, 0));
    }
    void CreatTrunChangeButton(Action nextTrunState)
    {
        GameObject obj = Instantiate(_stateEndButton, _stateEndButtonAnchor, Quaternion.identity, _resultCanvas);
        obj.TryGetComponent(out Button button);
        if (button == null)
        {
            obj.AddComponent<Button>();
        }
        button.onClick.AddListener(() => nextTrunState());
        button.onClick.AddListener(() => Destroy(button.gameObject));
    }
    void UseCard()
    {
        List<Action> actions = new List<Action>();
        int count = 0;
        Debug.Log("�J�[�h�g�p");
        Array.ForEach(_waitingCardListParent.GetComponentsInChildren<CardBase>(),
            card => actions.Add(() => StartCoroutine(card.CardUse(_player, () =>
            {
                count++;
                actions[count]();
            }))));
        actions.Add(() => NextTrun(Trun.PlayerAttackTargetSelection, .5f));
        actions[count]();
    }
    IEnumerator CallBack(Action action, Action EndAction, Func<bool> endIf)
    {
        action();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(endIf);
        EndAction();
    }
    IEnumerator CallBack(Action action, Action EndAction, float timar)
    {
        action();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(timar);
        EndAction();
    }
    IEnumerator PlayerAttackTargetSelection()
    {
        _selectEffect.SelectModeStart();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Input.GetMouseButton(0))
            {
                var enemy = GetMousePositionObject<CharacterBase>();
                if (enemy != null)
                {
                    _playerAttackTarget.Add(enemy);
                    _selectEffect.SelectModeEnd();
                    break;
                }
            }
        }

        NextTrun(Trun.PlayerAttack, 1);
    }
    IEnumerator PlayerAttack(List<CharacterBase> enemys)
    {
        foreach (var enemy in enemys)
        {
            Debug.Log("player�̍U��");
            _player.Attack(enemy);
            if (_player.TryGetComponent(out Animator animator))
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            }
            if (enemy._hp <= 0)
            {
                _enemyDictionary.Remove(enemy);
                Debug.Log(enemy + "��|����");
                if (_enemyDictionary.Count == 0)
                {
                    Victory();
                    yield break;
                }
            }
        }
        _playerAttackTarget.Clear();
        if (_doubleAttack) NextTrun(Trun.EndTrun, 1);
        NextTrun(Trun.EnemyAttack, 0);
    }
    IEnumerator EnemyAttack()
    {
        List<Action> actions = new List<Action>();//�s���ҋ@�p���X�g
        int count = 0;
        var endActions = false;
        Debug.Log("�G�̍U��");
        foreach (var enemy in _enemyDictionary.Keys)
        {
            CharacterBase attackObj = _player;
            CardBase nextcard;
            _enemyDictionary[enemy].TryDequeue(out nextcard);
            if (nextcard == null)
            {
                _enemyDictionary[enemy] = new Queue<CardBase>(ShuffleList(enemy._deck));
                _enemyDictionary[enemy].TryDequeue(out nextcard);
            }
            actions.Add(() => StartCoroutine(nextcard.CardUse(enemy, () =>
            {
                count++;
                if (actions.Count > count) actions[count]();
                else
                {
                    Debug.Log("�R���[�`���I��");
                    endActions = true;
                }
            })));
        }
        actions[0]();
        yield return new WaitUntil(() => endActions);
        if (_player._hp <= 0)
        {
            Defeat();
            yield break;
        }
        NextTrun(Trun.EndTrun, 0);
    }
    void EndTrun()
    {
        Debug.Log("���̃^�[��");
        NextTrun(Trun.Start, 0);
    }
    void Victory()
    {
        Debug.Log("victory");
        NextTrun(Trun.Result, 0);
    }
    void Defeat()
    {
        Debug.Log("defeat");
    }
    void Result(int count)//
    {
        Debug.Log("risult");
        var cards = ShuffleList(_praiseCardTable.ToList());//, count);
        var result = Instantiate(_resultPanel, _resultCanvas.transform);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("�J�[�h�ǉ�");
            var obj = Instantiate(cards[i], result.transform.GetChild(0));
            // EventTrigger �R���|�[�l���g���擾�܂��͒ǉ�
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.gameObject.AddComponent<EventTrigger>();
            }

            // PointerClick �C�x���g�p�̃G���g�����쐬
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            // �C�x���g���Ɏ��s����R�[���o�b�N��ǉ�
            entry.callback.AddListener((data) => { OnClick(cards[i]);
                GameManager.Instance.BattleEnd();
            });

            // Entry �� EventTrigger �ɒǉ�
            trigger.triggers.Add(entry);
        }
    }
    void OnClick(CardBase obj)//result�ł̕�V�I��p�̃��\�b�h
    {
        Debug.Log("click���ꂽ " + obj.name);
        //�A�j���[�V�������΁A�v���C���[�̃J�[�h���X�g�ɓo�^�Aresult��ʂ����\��
        if (_getReward)
        {
            _player._deck.Add(obj);
        }
    }
    void BattleEnd()
    {
        GameManager.Instance.BattleEnd();
        //canvas�̍폜
        gameObject.SetActive(false);
    }
    public void AddNewEnemy(CharacterBase enemy)
    {
        _enemyDictionary.Add(enemy, new Queue<CardBase>(ShuffleList(enemy._deck.ToList())));
    }
    public List<T> ShuffleList<T>(List<T> DeckData)
    {
        List<T> cardsList = DeckData;//player�̃J�[�h���X�g(�R�s�[)
        for (int i = 0; i < DeckData.Count; i++)
        {
            var j = Random.Range(0, DeckData.Count);
            var temp = cardsList[i];
            cardsList[i] = cardsList[j];
            cardsList[j] = temp;
        }
        return cardsList;
    }
    public IEnumerator GetClickMousePositionObj<T>()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                var obj = GetMousePositionObject<T>();
                if (obj != null)
                {
                    yield return obj;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// �}�E�X�|�C���^�̈ʒu����w�肵���R���|�[�l���g���擾����B
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T �������� null</returns>
    [return: MaybeNull]
    public T GetMousePositionObject<T>()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
#nullable enable
        if (hit.transform != null && hit.transform.TryGetComponent(out T? hitGameObjectComponent))
        {
            return hitGameObjectComponent;
        }
#nullable disable
        return default;
    }
    void NextTrun(Trun trunName, float waiteTimer)
    {
        StartCoroutine(NextTrunCoroutine(trunName, waiteTimer));
    }
    IEnumerator NextTrunCoroutine(Trun trunName, float waiteTimer)
    {
        bool endCroutine = false;
        switch (trunName)
        {
            case Trun.PlayerAttack:
                StartCoroutine(CallBack(() => ChangeTrunEffect("�����̂̍U��", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            case Trun.EnemyAttack:
                StartCoroutine(CallBack(() => ChangeTrunEffect("�G�̍U��", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            case Trun.EndTrun:
                StartCoroutine(CallBack(() => ChangeTrunEffect("���̃^�[��", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            default:
                endCroutine = true;
                break;
        }
        yield return new WaitUntil(() => endCroutine);
        yield return new WaitForSeconds(waiteTimer);
        CurrentTurn = trunName;
    }
    public void NextTrun(string trunName)//eventTrigger�p
    {
        CurrentTurn = (Trun)Enum.Parse(typeof(Trun), trunName);
        switch (CurrentTurn)
        {
            case Trun.PlayerAttack:
                break;
            case Trun.EnemyAttack:
                ChangeTrunEffect("�G�̍U��", null);
                break;
        }
    }
    void ChangeTrunEffect(string text, UnityEngine.Playables.PlayableAsset clip)
    {
        if (_timeLine == null) return;
        _timeLine.Text.text = text;
        if (clip == null)
        {
            _timeLine.Director.Play();
            return;
        }
        _timeLine.Director.Play(clip);
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