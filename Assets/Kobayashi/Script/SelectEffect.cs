using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SelectEffect : MonoBehaviour
{
    public static SelectEffect Instance;
    [SerializeField] float _changeColorBrightness;
    Color _changeColor;
    Dictionary<SpriteRenderer, Color> _defaultSpriteColorDic = new Dictionary<SpriteRenderer, Color>();
    Dictionary<Image, Color> _defaltImageColorDic = new Dictionary<Image, Color>();
    Dictionary<Text, Color> _defaultTextColorDic = new Dictionary<Text, Color>();
    bool _colorChanged;
    Coroutine _coroutine;
    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        _changeColor = new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0);
    }
    private void OnDisable()
    {
        Instance = null;
    }
    public void SelectModeStart()
    {
        _defaultSpriteColorDic = FindObjectsOfType<SpriteRenderer>().ToDictionary(sprite=>sprite, sprite=>sprite.color);
        _defaltImageColorDic = FindObjectsOfType<Image>().ToDictionary(sprite => sprite, sprite => sprite.color);
        _defaultTextColorDic = FindObjectsOfType<Text>().ToDictionary(text => text, text => text.color);
        _coroutine = StartCoroutine(SelectMode());
    }
    IEnumerator SelectMode()
    {
        ColorChange();
        SpriteRenderer _beforObj = null;
        while (true)
        {
            var hitObj = GetMousePositionObject<SpriteRenderer>(hit => hit.transform.tag == "Enemy");
            if (hitObj != _beforObj)
            {
                if (_beforObj)
                    _beforObj.color -= _changeColor;
                if (hitObj)
                    hitObj.color = _defaultSpriteColorDic[hitObj];
                _beforObj = hitObj;
            }
            //‰¹“ü‚ê‚é‚©‚à‚Ë
            yield return null;
        }
    }
    [return: MaybeNull]
    public T GetMousePositionObject<T>(Func<RaycastHit2D, bool> func) where T : Component
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
        if (hit.transform != null && hit.transform.TryGetComponent(out T hitGameObjectComponent) && func(hit))
        {
            return hitGameObjectComponent;
        }
        return default;
    }
    public void SelectModeEnd()
    {
        StopCoroutine(_coroutine);
        ColorReset();
    }
    public void ColorChange()
    {
        if (!_colorChanged)
        {
            foreach (var image in _defaltImageColorDic.Keys)
                image.color -= _changeColor;
            foreach (var renderer in _defaultSpriteColorDic.Keys)
                renderer.color -= _changeColor;
            foreach (var text in _defaultTextColorDic.Keys)
                text.color -= _changeColor;
            _colorChanged = true;
        }
    }
    public void ColorReset()
    {
        if (_colorChanged)
        {
            foreach (var image in _defaltImageColorDic)
                image.Key.color = image.Value;
            foreach (var renderer in _defaultSpriteColorDic)
                renderer.Key.color = renderer.Value;
            foreach (var text in _defaultTextColorDic)
                text.Key.color = text.Value;
            _colorChanged = false;
        }
    }
}
