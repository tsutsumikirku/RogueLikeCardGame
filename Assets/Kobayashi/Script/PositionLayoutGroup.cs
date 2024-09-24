using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PositionLayoutGroup : MonoBehaviour, ICardDirective
{
    [SerializeField] float _maxSpace;
    [SerializeField] float _maxScal;
    [SerializeField] float _cardMoveSpeed;
    int _childCount;
    RectTransform _rectTransform;
    List<GameObject> _children;
    Vector2 _nextObjectPos;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _nextObjectPos = transform.position;
    }
    private void Update()
    {
        if (_childCount != transform.childCount)
        {
            float space = _maxScal / transform.childCount >= _maxSpace ? _maxSpace : _maxScal / transform.childCount;
            float startPoint = transform.position.x;
            float childCenterElement = transform.childCount / 2f;
            for (int i = 1; i <= transform.childCount; i++)
            {
                if (_childCount < transform.childCount && i == transform.childCount)
                {
                    _nextObjectPos = new Vector2(
                        startPoint + space * (i - (childCenterElement+0.5f)),
                        transform.position.y);
                    _childCount = transform.childCount;
                    return;
                }
                var child = transform.GetChild(i-1);
                Debug.Log($"{space}*{(i - (childCenterElement + 0.5f))}={ space * (i - (childCenterElement + 0.5f))}");
                child.DOMove(new Vector2(startPoint + space * (i - (childCenterElement + 0.5f)), transform.position.y), _cardMoveSpeed);
            }
            _childCount = transform.childCount;
        }
    }
    public Vector2 GetPos()
    {
        return _nextObjectPos;
    }

    public void InParent(GameObject obj)
    {

    }

    public void OutParent(GameObject obj)
    {

    }
}
