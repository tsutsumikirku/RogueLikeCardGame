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
    [SerializeField] CharacterBase _player;
    List<CharacterBase> _playerAttackTarget = new List<CharacterBase>();
    [SerializeField] Canvas _resultCanvas;
    [SerializeField] Transform _handCard;
    [SerializeField] ButtleTimeLineManager _timeLine;
    //デバッグ用シリアライズ
    [SerializeField] List<CharacterBase> _enemyList = new List<CharacterBase>();
    List<Queue<CardBase>> _enemyDeck = new List<Queue<CardBase>>();
    Queue<CardBase> _playerDeck = new Queue<CardBase>();
    List<CardBase> _trashZone = new List<CardBase>();
    //
    Trun _trun;
    Dictionary<Action, Func<bool>> _cardPlayQueue = new Dictionary<Action, Func<bool>>();
    [SerializeField] GameObject _resultPanel;
    [SerializeField] bool Testmode;
    [SerializeField] GameObject _emptyCard;
    [SerializeField] CardBase[] _praiseCardList;

    [SerializeField] Transform _trashParent;
    [SerializeField] GameObject _stateEndButton;
    [SerializeField] Vector2 _stateEndButtonAnchor;

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
                PlayerAttack(_playerAttackTarget);
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
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
            SetData(_characterBasesTest);
        }
    }
    public void SetData(CharacterBase[] enemyArray)
    {
        var playerObj = GameObject.FindWithTag("Player");
        _player = playerObj.GetComponent<CharacterBase>();
        this.gameObject.SetActive(true);
        _playerAttackTarget.Clear();
        if (enemyArray.Length <= 0)
        {
            CurrentTurn = Trun.Result;
            return;
        }
        Debug.Log("ゲームスタート");
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
        Debug.Log("ターン開始");
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
            if (_playerDeck.Count <= 0)
            {
                Debug.Log("捨て札をデッキに戻しました");
                _playerDeck.Clear();
                _playerDeck = DeckShuffle(_trashZone);
                _trashZone.Clear();
            }
            Debug.Log("カードドロー");
            Instantiate(_playerDeck.Dequeue());
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
                Debug.Log("カード使用");
            }
        }
        if (debuffCard.Count > 0) StartCoroutine(UseDebuffCrad(debuffCard, _player));//コルーチン内でステートを管理する
        else NextTrun(Trun.PlayerAttackTargetSelection, 1);
        //StartCoroutine(CallBack(_cardPlayQueue));
    }
    IEnumerator UseDebuffCrad(List<CardBase> debufCardList, CharacterBase useCharacter)
    {
        int count = 0;
        Debug.Log("デバフ対象を選んでください");
        while (count < debufCardList.Count)
        {
            CharacterBase enemy = null;
            if (Input.GetMouseButton(0))
            {
                Debug.Log("ボタンが押された");
                enemy = GetMousePositionEnemy<CharacterBase>();
                if (enemy != null)
                {
                    Debug.Log($"{enemy}にデバフをかけた");
                    count++;
                    debufCardList[count].CardUse(useCharacter, enemy);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        //if (_stateEndButton == null)
        //{
        //    NextTrun(Trun.PlayerAttack, 0);
        //}
    }
    /// <summary>
    /// カード効果処理を直列で行うための関数
    /// </summary>
    /// <param name="taskList"></param>
    /// <returns></returns>
    IEnumerator CallBack(Dictionary<Action, Func<bool>> taskList)//カードの待機
    {
        foreach (var task in taskList)
        {
            Debug.Log("タスク開始");
            task.Key.Invoke();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(task.Value);
            Debug.Log("タスク終了");
        }
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
                foreach (var enemy in _enemyList)
                {
                    _playerAttackTarget.Add(enemy);
                }
                if (_enemyList.Count != 0)
                {
                    NextTrun(Trun.PlayerAttack, 3);
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
                            NextTrun(Trun.PlayerAttack, 1);
                            break;
                        }
                    }
                }
                break;
        }
    }
    void PlayerAttack(List<CharacterBase> enemys)
    {
        foreach (var enemy in enemys)
        {
            Debug.Log("playerの攻撃");
            _player.Attack(enemy);
            if (enemy._hp <= 0)
            {
                _enemyList.Remove(enemy);
                Debug.Log(enemy + "を倒した");
                if (_enemyList.Count == 0)
                {
                    Victory();
                    return;
                }
            }
        }
        _playerAttackTarget.Clear();
        NextTrun(Trun.EnemyAttack, 3);
    }
    void EnemyAttack()
    {
        Debug.Log("敵の攻撃");
        for (int i = 0; i < _enemyList.Count; i++)
        {
            CharacterBase attackObj = _player;
            foreach (var enemy in _enemyList[i]._debuff)
            {
                //混乱デバフの名前が決まり次第書き換えます
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
        var cards = RandomCard(_praiseCardList, count);
        var result = Instantiate(_resultPanel, _resultCanvas.transform);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("カード追加");
            var obj = Instantiate(_emptyCard, result.transform.GetChild(0));
            // EventTrigger コンポーネントを取得または追加
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.AddComponent<EventTrigger>();
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
            Debug.Log("Evenntotuika");
        }
    }
    void OnClick()//resultでの報酬選択用のメソッド
    {
        Debug.Log("clickされた " + this.gameObject.name);
        //アニメーション発火、プレイヤーのカードリストに登録、result画面を閉じる

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
    /// <summary>
    /// CardUseの処理を待機させるためのキュー
    /// </summary>
    /// <param name="action">実行したい処理</param>
    /// <param name="endConditions">処理の終了条件</param>
    public void SetCardUse(Action action, Func<bool> endConditions)
    {
        _cardPlayQueue.Add(action, endConditions);
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
            //case Trun.Result:
            //      コールバック
            //    break;
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