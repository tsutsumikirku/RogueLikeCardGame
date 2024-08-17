using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardBase : MonoBehaviour , IDragHandler , IBeginDragHandler, IPointerUpHandler, IPointerDownHandler
{
  
    GameObject _desk;
    GameObject _nulldesk;
    GameObject _selectdesk;
    CharacterBase _player;
    [SerializeField] BuffDebuff _isBuff;
    [SerializeField] Buff _buff;
    [SerializeField] AttackPaturn _attackPaturn;
    [SerializeField] float _damage;
    [SerializeField] int _attackCount = 1;
    [SerializeField] bool _isPlayer = true;
    [SerializeField] bool _test = true;
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
    public void LateUpdate()
    {
        
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
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
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
    public void CardUse()
    {
        CardUsing();
        CardUseEvent();
    }
    public void CardUsing()
    {
        if(_isBuff == BuffDebuff.Buff)
        {
            _player._buff.Add(_buff);
        }
        else if(_isBuff == BuffDebuff.OneTimeBuff)
        {
            _player._oneTimeBuff.Add(_buff);
        }
        else if(_isBuff == BuffDebuff.Debuff)
        {

        }
        
    }
    public virtual void CardUseEvent()
    {
        Destroy(gameObject);
    }
}
public enum AttackPaturn
{
    Single,
    All,
    Mine
}
public enum BuffDebuff
{
    OverRide,
    Buff,
    OneTimeBuff,
    Debuff
}
public enum DebuffSelectState
{
    None,
    SelectBefore,
    SelectAfter
}
