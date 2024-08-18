using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;

public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool _isuse = true;
    [SerializeField,Tooltip("選択するカードの上限値を設定してくださいちなみに初期値は3です")] int _maxSelect = 3;
    GameObject _desk;
    GameObject _selectDesk;
    GameObject _nullDesk;
    private void Start()
    {
        _desk = GameObject.FindWithTag("Desk");
        _selectDesk = GameObject.FindWithTag("SelectDesk");
        _nullDesk = GameObject.FindWithTag("DeskOut");
        gameObject.transform.SetParent(_desk.transform);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isuse) 
        { 
            gameObject.transform.SetParent(_nullDesk.transform); 
        }
       
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (_isuse)
        {
            gameObject.GetComponent<RectTransform>().position = eventData.position;
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

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (_isuse)
        {
            GameObject _isdesk = GetCurrentDeck(eventData);
            if (!_isdesk)
            {
                if(_selectDesk.transform.childCount < _maxSelect)
                {
                    gameObject.transform.SetParent(_selectDesk.transform);
                }
                else
                {
                    gameObject.transform.SetParent(_desk.transform);
                }
            }
            else
            {
                gameObject.transform.SetParent(_isdesk.transform);
            }
        }
    }
}
