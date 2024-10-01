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
    /// float�^��AnimationSpeed��bool�^��Fade�����A�j���[�V�����̂ݓ����܂��B
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
            //�}�C�t���[��ray��΂��Ă�
            var data = GetUiRayChast<IHaveCardBase>();
            if (data != null)
            {
                _timer += Time.deltaTime;
                if (_timer > _displayInfoOnCursorTime)
                {
                    FadeAnimationBool("FadeIn", true);
                    SetData(GetUiRayChast<IHaveCardBase>().CardBase);
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
    void SetData(CardBase cardBase)
    {
        _name.text = cardBase.cardData._cardName;
        var card = cardBase;
        _information.text = cardBase.cardData._infometion;
        if (_prizeDisplay) _prize.text = $"�l�i : {cardBase.cardData._price.ToString()}";
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
interface IHaveCardBase
{
    public CardBase CardBase { get; }
}
