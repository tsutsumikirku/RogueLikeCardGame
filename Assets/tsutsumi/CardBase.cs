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
    CharacterBase[]_enemy;
    [SerializeField] Buff _buff;
    [SerializeField] float _buffcount;
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
            GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
            _player = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
            enemy = new GameObject[enemy.Length];
            for (int i = 0; i < enemy.Length; i++)
            {
                _enemy[i] = enemy[i].GetComponent<CharacterBase>();
            }
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
    public virtual void CardUsing()
    {

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
