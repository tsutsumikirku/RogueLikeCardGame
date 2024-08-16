using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour , IDragHandler , IBeginDragHandler, IPointerUpHandler, IPointerDownHandler
{
    GameObject _desk;
    GameObject _nulldesk;
    void Start()
    {
        _desk = GameObject.FindWithTag("Desk");
        _nulldesk = GameObject.FindWithTag("DeskOut");
        transform.SetParent(_desk.transform);
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(_nulldesk.transform);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
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
        GameObject desk = GetCurrentDeck(eventData);
        if (desk)
        {
            transform.SetParent(desk.transform);
        }
        else 
        {
            CardUse();
            CardUseEvent();
        }
    }
    public virtual void CardUse()
    {
        Debug.Log("CardUse");
    }
    public virtual void CardUseEvent()
    {
        Destroy(gameObject);
    }
}
