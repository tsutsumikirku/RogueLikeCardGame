using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] BattleCanvasChildData _battleCanvasPrefabChildData;
    BattleCanvasChildData _canvas;
    [SerializeField] GameObject _resultTable;
    [SerializeField] GameObject _stateEndButton;
    [SerializeField] public SelectEffect _selectEffect;
    [HideInInspector] public CharacterBase _player;
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    List<CharacterBase> _playerAttackTarget = new List<CharacterBase>();
    public Dictionary<CharacterBase, Queue<CardBase>> _enemyDictionary = new Dictionary<CharacterBase, Queue<CardBase>>();
    //カード能力用フラグ
    [HideInInspector] public bool _doubleAttack;
    //報酬選択をしたかどうかのフラグ
    [HideInInspector] public bool _getReward;
    CardBase[] _praiseCardTable;//報酬のテーブル
    Trun _trun;//ステート管理     CurrentTurnからいじれます。直接いじるな
    //プレハブのデータを持つ部分
    BattleCardManager _cardManager;
    ButtleTimeLineManager _timeLine;
    Transform _waitingCardListParent;
    [HideInInspector] public Transform _trashParent;
    Transform _handCard;
    Transform _resultCanvas;
    List<RectTransform> _enemyAnchor;
    float _maxEnemy;
    //デバッグ用     初期値のテストに使っているためマージの時消して
    [SerializeField] CharacterBase[] _characterBasesTest;
    [SerializeField] CardBaseArray _CardDataScriptablObj;
    [SerializeField] bool _testmode;
    [SerializeField] int _enemyCount;

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
                //バトルスタート時の演出がいる。
                break;
            case Trun.Draw:
                _cardManager.AllCardDragMode(true);
                int num = _handCard.childCount;
                int drawCount = _firstDraw - num;
                Debug.Log("手札を" + drawCount + "引きました");
                StartCoroutine(DrawCard(drawCount));
                break;
            case Trun.ChoiseUseCard:
                ChoiseUseCard();
                //ボタンで次のstateに
                break;
            case Trun.UseCard:
                _cardManager.AllCardDragMode(false);
                CardUse();
                //ボタンで次のstateに
                break;
            case Trun.PlayerAttackTargetSelection://このstateは現在使われておりません
                Debug.Log("敵を選べ");
                //ボタンで次のstateに
                StartCoroutine(PlayerAttackTargetSelection());
                break;
            case Trun.PlayerAttack:
                PlayerAttack();//_playerAttackTarget));
                break;
            case Trun.EnemyAttack:
                StartCoroutine(EnemyAttack());
                break;
            case Trun.EndTrun:
                EndTrun();
                break;
            case Trun.Result:
                //result処理
                Result(3);
                break;
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    private void Start()
    {
        if (_testmode)
        {
            BattleStart(GameObject.FindWithTag("Player").GetComponent<CharacterBase>(), _characterBasesTest, _CardDataScriptablObj.Cards);
        }
    }
    private void Update()
    {
        _enemyCount = _enemyDictionary.Count;
    }
    /// <summary>
    /// バトル開始時に呼ぶ関数
    /// </summary>
    /// <param name="player">操作キャラ</param>
    /// <param name="enemyArray">敵の配列</param>
    /// <param name="prizeCards">報酬のテーブル</param>
    public void BattleStart(CharacterBase player, CharacterBase[] enemyArray, CardBase[] prizeCards)
    {
        //battleUIのcloneと初期設定
        _canvas = Instantiate(_battleCanvasPrefabChildData);
        SetData(_canvas);
        //playerのカードベース取得
        _player = player.gameObject.GetComponent<CharacterBase>();
        gameObject.SetActive(true);//一応
        _praiseCardTable = prizeCards;//報酬の設定
        _getReward = false;
        _doubleAttack = false;
        //エネミーがいない場合即座に報酬画面に進みメソッドを終了する
        if (enemyArray == null)
        {
            CurrentTurn = Trun.Result;
            return;
        }
        Debug.Log("ゲームスタート");
        var instansCardData = _cardManager.CardCreatSeting(_player._deck);
        _playerDeck = new Queue<CardBase>(ShuffleList(instansCardData.ToList()));
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyDictionary.Add(enemy, new Queue<CardBase>(ShuffleList(enemy._deck)));
        }
        NextTrun(Trun.Start, 1);
        void SetData(BattleCanvasChildData canvas)
        {
            _timeLine = canvas._timeLineManager;
            _trashParent = canvas._trashParent;
            _waitingCardListParent = canvas._waitingCardListParent;
            _handCard = canvas._handCard;
            _cardManager = canvas._cardManager;
            _resultCanvas = canvas.transform;
            _enemyAnchor = new List<RectTransform>(canvas._enemysAnchor);
            _maxEnemy = canvas._enemysAnchor.Count;
        }
    }
    void StartEffect()
    {
        Debug.Log("ターン開始");
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
                Debug.Log("捨て札をデッキに戻しました");
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
        Debug.Log("カード選択");
        CreatTrunChangeButton(() => NextTrun(Trun.UseCard, 0));
    }
    void CreatTrunChangeButton(Action nextTrunState)
    {
        GameObject obj = Instantiate(_stateEndButton, _canvas._buttonAnchar.position, Quaternion.identity, _resultCanvas);
        obj.TryGetComponent(out Button button);
        if (button == null)
        {
            obj.AddComponent<Button>();
        }
        button.onClick.AddListener(() => nextTrunState());
        button.onClick.AddListener(() => Destroy(button.gameObject));
    }
    void CardUse()
    {
        List<Action> actions = new List<Action>();
        int count = 0;
        Debug.Log("カード使用");
        Array.ForEach(_waitingCardListParent.GetComponentsInChildren<CardBase>(),
            card => actions.Add(() => StartCoroutine(card.CardUse(_player, () =>
            {
                count++;
                actions[count]();
            }))));
        actions.Add(() => NextTrun(Trun.PlayerAttack, .5f));
        actions[count]();
    }
    IEnumerator CallBack(Action action, Action endAction, Func<bool> endIf)
    {
        action();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(endIf);
        endAction();
    }
    IEnumerator CallBack(Action action, Action EndAction, float timar)
    {
        action();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(timar);
        EndAction();
    }
    IEnumerator PlayerAttackTargetSelection()//現在使っていない関数
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
    void PlayerAttack()//List<CharacterBase> enemys)
    {
        Debug.Log("playerの攻撃");
        _player.Attack(() =>
        {
            foreach (var enemy in new List<CharacterBase>(_enemyDictionary.Keys))
            {
                if (enemy._hp <= 0)
                {
                    _enemyDictionary.Remove(enemy);
                    Debug.Log(enemy + "を倒した");
                }
                if (_enemyDictionary.Count == 0)
                {
                    Victory();
                    break;
                }
            }
            if (_enemyDictionary.Count != 0) NextTrun(Trun.EnemyAttack, 0);
            _playerAttackTarget.Clear();
        });
        //if (_doubleAttack) NextTrun(Trun.EndTrun, 1);
    }
    IEnumerator EnemyAttack()
    {
        List<Action> actions = new List<Action>();//行動待機用リスト
        int count = 0;
        var endActions = false;
        //foreach内で配列の要素が増える可能性があるためコピーをとる
        var enemyList = new List<CharacterBase>(_enemyDictionary.Keys);
        Debug.Log("敵の攻撃");
        foreach (var enemy in enemyList)
        {
            CardBase useCard;
            _enemyDictionary[enemy].TryDequeue(out useCard);
            if (useCard == null)
            {
                _enemyDictionary[enemy] = new Queue<CardBase>(ShuffleList(enemy._deck));
                _enemyDictionary[enemy].TryDequeue(out useCard);
            }
            actions.Add(() => StartCoroutine(useCard.CardUse(enemy, () =>
            {
                count++;
                Debug.Log(useCard.name);
                if (actions.Count > count) actions[count]();
                else
                {
                    Debug.Log("コルーチン終了");
                    endActions = true;
                }
            })));
        }
        actions[0]();
        yield return new WaitUntil(() => endActions);
        if (_player._hp <= 0)
        {
            Invoke(nameof(Defeat), 1);
            yield break;
        }
        NextTrun(Trun.EndTrun, 0);
    }
    void EndTrun()
    {
        Debug.Log("次のターン");
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
        ResultScript.LoadResultScene(false);
        Invoke(nameof(BattleEnd), 1);
    }
    void Result(int count)//
    {
        _player.BuffReset();
        Debug.Log("risult");
        var cards = ShuffleList(_praiseCardTable.ToList());//, count);
        Debug.Log($"{_resultTable} {_resultCanvas.transform}");
        var result = Instantiate(_resultTable, _resultCanvas.transform);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("カード追加");
            var obj = Instantiate(cards[i], result.transform);
            // EventTrigger コンポーネントを取得または追加
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.gameObject.AddComponent<EventTrigger>();
            }

            // PointerClick イベント用のエントリを作成
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            // イベント時に実行するコールバックを追加
            entry.callback.AddListener((data) =>
            {
                OnClick(cards[i]);
                if (!_testmode) GameManager.Instance.BattleEnd();
                Destroy(_canvas.gameObject);
            });

            // Entry を EventTrigger に追加
            trigger.triggers.Add(entry);
        }
    }
    void OnClick(CardBase obj)//resultでの報酬選択用のメソッド
    {
        //アニメーション発火、プレイヤーのカードリストに登録、result画面を閉じる予定
        if (!_getReward)
        {
            Debug.Log("clickされた " + obj.name);
            _player._deck.Add(obj);
            _getReward = true;
        }
    }
    void BattleEnd()
    {
        _player.BuffReset();
        Destroy(_canvas.gameObject);
        gameObject.SetActive(false);
        if (!_testmode) GameManager.Instance.BattleEnd();
    }
    public void AddNewEnemy(CharacterBase enemy)
    {
        if (_enemyDictionary.Count < _maxEnemy)
        {
            Debug.Log("絵ねmy－を追加しました");
            Vector2 anchorPoint = Camera.main.ScreenToWorldPoint(_enemyAnchor[_enemyDictionary.Count].transform.position);
            Debug.Log(anchorPoint);
            var enemyObj = Instantiate(enemy, anchorPoint, Quaternion.identity);
            _enemyDictionary.Add(enemyObj, new Queue<CardBase>(ShuffleList(enemyObj._deck.ToList())));
        }
        else Debug.Log("エネミーを追加していません");
    }
    public List<T> ShuffleList<T>(List<T> DeckData)
    {
        List<T> cardsList = DeckData;//playerのカードリスト(コピー)
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
    /// マウスポインタの位置から指定したコンポーネントを取得する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T もしくは null</returns>
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
        Debug.Log(_timeLine);
        bool endCroutine = false;
        switch (trunName)
        {
            case Trun.PlayerAttackTargetSelection:
                StartCoroutine(CallBack(() => ChangeTrunEffect("敵を選べ", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            case Trun.PlayerAttack:
                StartCoroutine(CallBack(() => ChangeTrunEffect("味方のの攻撃", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            case Trun.EnemyAttack:
                StartCoroutine(CallBack(() => ChangeTrunEffect("敵の攻撃", null),
                    () => endCroutine = true,
                    () => _timeLine.Director.duration <= _timeLine.Director.time || _timeLine.Director.time == 0));
                break;
            case Trun.EndTrun:
                StartCoroutine(CallBack(() => ChangeTrunEffect("次のターン", null),
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
    public void NextTrun(string trunName)//eventTrigger用
    {
        CurrentTurn = (Trun)Enum.Parse(typeof(Trun), trunName);
        switch (CurrentTurn)
        {
            case Trun.PlayerAttack:
                break;
            case Trun.EnemyAttack:
                ChangeTrunEffect("敵の攻撃", null);
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