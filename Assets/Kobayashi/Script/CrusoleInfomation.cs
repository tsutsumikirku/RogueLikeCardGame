using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrusoleInfomation : MonoBehaviour
{
    [SerializeField] Text _name;
    [SerializeField] Text _information;
    [SerializeField] Text _prize;
    [SerializeField] float _displayInfoOnCursorTime;
    [SerializeField] float _animationTime;
    [SerializeField] bool _trackingMode;
    [SerializeField] bool _prizeDisplay;
    float _timer;
    EventSystem eventSystem;
    GraphicRaycaster _graphicRaycaster;
    /// <summary>
    /// float型のAnimationSpeedとbool型のFadeを持つアニメーションのみ動きます。
    /// </summary>
    [SerializeField] Animator[] _animators;
    private void OnEnable()
    {
        eventSystem = EventSystem.current;
        _graphicRaycaster = transform.parent.GetComponent<GraphicRaycaster>();
        foreach (var animator in _animators)
        {
            animator.SetFloat("AnimationSpeed", 1f / _animationTime);
        }
    }
    private void Update()
    {
        if (_trackingMode)
        {
            transform.position =
                Input.mousePosition;
            //マイフレームray飛ばしてる
            var data = GetUiRayChast<IHaveCardData>();
            if (data != null)
            {
                _timer += Time.deltaTime;
                if (_timer > _displayInfoOnCursorTime)
                {
                    FadeAnimationBool("FadeIn", true);
                    SetData(GetUiRayChast<IHaveCardData>().CardData);
                }
            }
            else
            {
                _timer = 0;
                FadeAnimationBool("FadeOut", false);
            }
        }

    }
    void FadeAnimationBool(string animationName, bool fade)
    {
        foreach (var animator in _animators)
        {
            animator.SetBool("Fade", fade);
        }
    }
    void SetData(CardData cardData)
    {
        _name.text = cardData._cardName;
        _information.text = cardData._information;
        if (_prizeDisplay) _prize.text = $"値段 : {cardData._price.ToString()}";
        else _prize.text = null;
    }
    T GetUiRayChast<T>()
    {
        PointerEventData eventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(eventData, results);
        List<T> result = new List<T>();
        results.ForEach(r =>
        {
            if (r.gameObject.TryGetComponent(out T data))
            {
                result.Add(data);
            }
        });
        if (result.Count > 0) return result[0];
        return default;
    }
}
interface IHaveCardData
{
    public CardData CardData { get; }
}