using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] CharacterBase[] _characterBasesTest;//テスト用
    [SerializeField] public CharacterBase _player;
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    List<CharacterBase> _playerAttackTarget = new List<CharacterBase>();
    [SerializeField] Canvas _resultCanvas;
    [SerializeField] Transform _handCard;
    [SerializeField] ButtleTimeLineManager _timeLine;
    //デバッグ用シリアライズ
    public Dictionary<CharacterBase, Queue<CardBase>> _enemyDictionary = new Dictionary<CharacterBase, Queue<CardBase>>();
    List<CardBase> _trashZone = new List<CardBase>();
    //
    Trun _trun;
    [SerializeField] GameObject _resultPanel;
    [SerializeField] bool Testmode;
    CardBase[] _praiseCardArray;

    public Transform _trashParent;
    [SerializeField] GameObject _stateEndButton;
    [SerializeField] Vector2 _stateEndButtonAnchor;
    //カード能力用フラグ
    public bool _enemyTrunSkip;

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
                int num = _handCard.childCount;
                int drawCount = _firstDraw - num;
                Debug.Log("手札を" + drawCount + "引きました");
                DrawCard(drawCount);
                break;
            case Trun.ChoiseUseCard:
                ChoiseUseCard();
                //ボタンで次のstateに
                break;
            case Trun.UseCard:
                UseCard();
                //ボタンで次のstateに
                break;
            case Trun.PlayerAttackTargetSelection:
                Debug.Log("敵を選べ");
                //ボタンで次のstateに
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
                //result処理
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
            BattleStart(GameObject.FindWithTag("Player").GetComponent<CharacterBase>(), _characterBasesTest, new CardBase[3]);
        }
    }
    /// <summary>
    /// バトル開始時に呼ぶ関数
    /// </summary>
    /// <param name="player">操作キャラ</param>
    /// <param name="enemyArray">敵の配列</param>
    /// <param name="prizeCards">報酬のテーブル</param>
    public void BattleStart(CharacterBase player, CharacterBase[] enemyArray, CardBase[] prizeCards)
    {
        //playerのカードベース取得
        _player = player.gameObject.GetComponent<CharacterBase>();
        gameObject.SetActive(true);//一応
        _praiseCardArray = prizeCards;//報酬の設定
        //エネミーがいない場合即座に報酬画面に進みメソッドを終了する
        if (enemyArray == null)
        {
            CurrentTurn = Trun.Result;
            return;
        }
        Debug.Log("ゲームスタート");
        _playerDeck = DeckShuffle(_player?._deck);
        foreach (CharacterBase enemy in enemyArray)
        {
            _enemyDictionary.Add(enemy, DeckShuffle(enemy._deck));
        }
        NextTrun(Trun.Start, 1);
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
    void DrawCard(int DrawCount)
    {
        for (int i = 0; i < DrawCount; i++)
        {
            if (_playerDeck.Count <= 0)
            {
                Debug.Log("捨て札をデッキに戻しました");
                _playerDeck.Clear();
                _playerDeck = DeckShuffle(_trashZone);
                _trashZone.Clear();
            }
            if (_playerDeck.TryDequeue(out CardBase card))
            {
                Instantiate(card).GetComponent<CardBase>();
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
        GameObject obj = Instantiate(_stateEndButton, _stateEndButtonAnchor, Quaternion.identity, _resultCanvas.transform);
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
        Debug.Log("カード使用");
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
    IEnumerator PlayerAttackTargetSelection()
    {
        switch (_player._attackPattern)
        {
            case AttackPattern.All:
                foreach (var enemy in _enemyDictionary.Keys)
                {
                    _playerAttackTarget.Add(enemy);
                }
                if (_enemyDictionary.Count != 0)
                {
                    break;
                }
                else
                {
                    Victory();
                }
                yield break;
            default:
                while (true)
                {
                    yield return new WaitForEndOfFrame();
                    if (Input.GetMouseButton(0))
                    {
                        var enemy = GetMousePositionEnemy<CharacterBase>();
                        if (enemy != null)
                        {
                            _playerAttackTarget.Add(enemy);
                            break;
                        }
                    }
                }
                break;
        }
        NextTrun(Trun.PlayerAttack, 1);
    }
    IEnumerator PlayerAttack(List<CharacterBase> enemys)
    {
        foreach (var enemy in enemys)
        {
            Debug.Log("playerの攻撃");
            _player.Attack(enemy);
            if (_player.TryGetComponent(out Animator animator))
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            }
            if (enemy._hp <= 0)
            {
                _enemyDictionary.Remove(enemy);
                Debug.Log(enemy + "を倒した");
                if (_enemyDictionary.Count == 0)
                {
                    Victory();
                    yield break;
                }
            }
        }
        _playerAttackTarget.Clear();
        if(_enemyTrunSkip)NextTrun(Trun.EndTrun, 1);
        NextTrun(Trun.EnemyAttack, 0);
    }
    IEnumerator EnemyAttack()
    {
        List<Action> actions = new List<Action>();//行動待機用リスト
        int count = 0;
        var endActions = false;
        Debug.Log("敵の攻撃");
        foreach (var enemy in _enemyDictionary)
        {
            CharacterBase attackObj = _player;
            enemy.Value.TryDequeue(out CardBase nextcard);
            if (nextcard == null)
            {
                enemy.Value.Clear();
                _enemyDictionary[enemy.Key] = DeckShuffle(enemy.Key._deck);
            }
            actions.Add(() => StartCoroutine(nextcard.CardUse(enemy.Key, () =>
            {
                count++;
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
            Defeat();
            yield break;
        }
        NextTrun(Trun.EndTrun, 3);
    }
    void EndTrun()
    {
        Debug.Log("次のターン");
        NextTrun(Trun.Start,0);
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
        var cards = RandomCard(_praiseCardArray, count);
        var result = Instantiate(_resultPanel, _resultCanvas.transform);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("カード追加");
            var obj = Instantiate(cards[i], result.transform.GetChild(0));
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
            entry.callback.AddListener((data) => { OnClick(); });

            // Entry を EventTrigger に追加
            trigger.triggers.Add(entry);
        }
    }
    void OnClick()//resultでの報酬選択用のメソッド
    {
        Debug.Log("clickされた " + this.gameObject.name);
        //アニメーション発火、プレイヤーのカードリストに登録、result画面を閉じる予定

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
        //_enemyDictionary.Add(enemy, DeckShuffle(enemy._deck));
    }
    Queue<T> DeckShuffle<T>(List<T> DeckData)
    {
        List<T> cardsList = new List<T>();//playerのカードリスト(コピー)
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
    public IEnumerator GetClickMousePositionObj<T>()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                var obj = GetMousePositionEnemy<T>();
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
    public T GetMousePositionEnemy<T>()
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
    T[] RandomCard<T>(T[] randomizeArray, int count)
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
                randomCopy[random] = randomCopy[randomCopy.Length - 1 - i];
            }
            return result;
        }
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