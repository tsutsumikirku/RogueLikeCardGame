using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] int _firstDraw;
    [SerializeField] Transform _waitingCardListParent;
    [SerializeField] CharacterBase[] _characterBases;//テスト用
    [SerializeField] CharacterBase _player;
    [SerializeField] List<CharacterBase> _enemyList = new List<CharacterBase>();
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
    private void Start()
    {
        if (Testmode)
        {
            SetData(_characterBases);
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
                Debug.Log("敵を選べ");
                ;
                break;
            case Trun.EnemyAttack:
                EnemyAttack();
                break;
            case Trun.EndTrun:
                EndTrun();
                break;
            case Trun.Result:
                //result処理
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
                        Debug.Log(enemy + "を倒した");
                    }
                }
                if (_enemyList.Count == 0)
                {
                    NextTrun(Trun.EnemyAttack, 3);
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
                            Debug.Log(enemy + "を倒した");
                        }
                        if (_enemyList.Count == 0)
                        {
                            NextTrun(Trun.EnemyAttack, 3);
                        }
                    }
                }
                break;
        }
        if (_enemyList.Count == 0)
        {
            Victory();
        }
    }
    public void SetData(CharacterBase[] enemyArray)
    {
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
        foreach(CharacterBase enemy in _enemyList)
        {
            if (enemy._hp <= 0)
            {
                _enemyList.Remove(enemy);
            }
        }
        if (_enemyList.Count <= 0)
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
        Debug.Log("カードドロー");
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
                Debug.Log("カード使用");
            }
        }
        StartCoroutine(UseDebuffCrad(debuffCard, _player));//コルーチン内でターンを管理する
    }
    void EnemyAttack()
    {
        Debug.Log("敵の攻撃");
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyDeck[i].Dequeue().CardUse(_enemyList[i], _player);
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
        Debug.Log("ターン終了");
        NextTrun(Trun.Start);
    }
    void Result()
    {
        gameObject.SetActive(false);
        GameManager.Instance.BattleEnd();

    }
    Queue<T> DeckShuffle<T>(List<T> DeckData)
    {
        List<T> cardsList = new List<T>();//playerのカードリスト（仮想）
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
        while (count < debufCardList.Count)
        {
            var enemy = GetMouseClickEnemy();
            if (enemy != null)
            {
                count++;
                debufCardList[count].CardUse(useCharacter, enemy);
            }
            yield return new WaitForEndOfFrame();
        }
        NextTrun(Trun.PlayerAttack);
    }
    void NextTrun(Trun trunName, float waiteTimer)
    {
        //StopCoroutine(NextTrunCoroutine(trunName, waiteTimer));
        StartCoroutine(NextTrunCoroutine(trunName, waiteTimer));
    }
    IEnumerator NextTrunCoroutine(Trun trunName, float waiteTimer)//デバッグ、アニメーションにも使うかもねくらい
    {
        Debug.Log($"TrunChange{trunName}");
        yield return new WaitForSeconds(waiteTimer);
        if (CurrentTurn != Trun.Result)
        {
            CurrentTurn = trunName;
        }
    }
    void NextTrun(Trun trunName)//アニメーションやイベントトリガーなどで呼ぶよう
    {
        Debug.Log($"TrunChange{trunName}");
        CurrentTurn = trunName;
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
