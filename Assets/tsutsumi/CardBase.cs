using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour , IDragHandler , IBeginDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // Start is called before the first frame update
    RectTransform _desk;
    RectTransform _nulldesk = null;
    RectTransform _rectTransform;
    void Start()
    {

        _desk = GameObject.FindWithTag("Desk").GetComponent<RectTransform>();
        _nulldesk = GameObject.FindWithTag("DeskOut").GetComponent<RectTransform>();
       _rectTransform = GetComponent<RectTransform>();
        _rectTransform.SetParent(_desk);
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.SetParent(_nulldesk);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = eventData.position;
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
            _rectTransform.SetParent(desk.transform);
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
