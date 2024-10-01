using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLayoutGroup : MonoBehaviour
{
    [SerializeField] float _maxSpace;
    [SerializeField] float _maxScal;
    [SerializeField] float _cardMoveSpeed;
    [SerializeField] public bool InCardSetActive;
    int _childCount;
    RectTransform _rectTransform;
    List<GameObject> _children;
    Vector2 _nextObjectPos;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _nextObjectPos = transform.position;
    }
    private void LateUpdate()
    {
        if (_childCount != transform.childCount)
        {
            float space = _maxScal / transform.childCount >= _maxSpace ? _maxSpace : _maxScal / transform.childCount;
            float startPoint = transform.position.x;
            float childCenterElement = transform.childCount / 2f;
            for (int i = 1; i <= transform.childCount; i++)
            {
                var child = transform.GetChild(i - 1);
                var movePos = new Vector2(startPoint + space * (i - (childCenterElement + 0.5f)), transform.position.y);
                //0.5は偶奇のポジションをずらす必要があるため
                if (child.TryGetComponent(out DragAndDrop dragAndDrop))
                {
                    dragAndDrop.enabled=InCardSetActive;
                }
                StartCoroutine(CallBackDoTween(child, movePos));
                //else child.DOMove(movePos, _cardMoveSpeed);
            }
            _childCount = transform.childCount;
        }
    }
    IEnumerator CallBackDoTween(Transform obj, Vector2 endPos)
    {
        yield return obj.DOMove(endPos, _cardMoveSpeed).WaitForCompletion();
    }
}
