using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public static bool _isPlayer = true;
    GameObject _desk;
    GameObject _nulldesk;
    GameObject _selectdesk;
    CharacterBase _player;
    [SerializeField] BuffDebuff _isBuff;
    [SerializeField] Buff _buff;
    [SerializeField] int _attackCount = 1;
    [SerializeField] bool _test = true;
    [SerializeField] string _dictionary = "ê‡ñæÇ»Çµ";
    [SerializeField] int _price = 1;
    void Start()
    {
        _desk = GameObject.FindWithTag("Desk");
        _nulldesk = GameObject.FindWithTag("DeskOut");
        _selectdesk = GameObject.FindWithTag("SelectDesk");
        if (!_test)
        {
            _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        }
        transform.SetParent(_desk.transform);
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (_isPlayer)
        {
            transform.SetParent(_nulldesk.transform);
        }
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (_isPlayer)
        {
            transform.position = eventData.position;
        }
        else if(transform.parent.tag =="DeskOut")
        {
            transform.SetParent(_desk.transform);
        }
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (_isPlayer)
        {
            GameObject desk = GetCurrentDeck(eventData);
            if (desk)
            {
                transform.SetParent(desk.transform);
            }
            else
            {
                transform.SetParent(_selectdesk.transform);
            }
        }
    }

    GameObject GetCurrentDeck(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        RaycastResult result = default;
        foreach (var item in results)
        {
            if (item.gameObject.CompareTag("Desk"))
            {
                result = item;
                break;
            }
        }
        return result.gameObject;
    }
    public BuffDebuff CardUse(CharacterBase enemy)
    {
        CardUsing(enemy);
        CardUseEvent();
        return _isBuff;
    }
    public void CardUsing(CharacterBase enemy)
    {
        if (_isBuff == BuffDebuff.OverRide)
        {
            CardOverRide();
        }
        else if (_isBuff == BuffDebuff.Attack)
        {
            enemy.Attack(_player);
        }
        else if (_isBuff == BuffDebuff.Buff)
        {
            _player._buff.Add(_buff);
        }
        else if (_isBuff == BuffDebuff.OneTimeBuff)
        {
            _player._oneTimeBuff.Add(_buff);
        }
        else if (_isBuff == BuffDebuff.AllAttack)
        {
            _player._attackPattern = AttackPattern.All;
        }
        else if (_isBuff == BuffDebuff.AttackCount)
        {
            _player._attackCount = _attackCount;
        }
        else if (_isBuff == BuffDebuff.Debuff)
        {
        }

    }
    public virtual void CardOverRide()
    {

    }
    public virtual void CardUseEvent()
    {
        Destroy(gameObject);
    }


}
public enum BuffDebuff
{
    OverRide,
    Attack,
    Buff,
    OneTimeBuff,
    AllAttack,
    AttackCount,
    Debuff
}
public enum DebuffSelectState
{
    None,
    SelectBefore,
    SelectAfter
}
