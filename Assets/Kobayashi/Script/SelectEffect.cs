using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectEffect : MonoBehaviour
{
    public static SelectEffect Instance;
    [SerializeField] float _changeColorBrightness;
    Color _changeColor;
    List<Image> _images = new List<Image>();
    List<SpriteRenderer> _renderer = new List<SpriteRenderer>();
    List<Text> _text = new List<Text>();
    bool _colorChanged;
    Coroutine _coroutine;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
        Debug.Log(Instance.gameObject);
    }
    public void SelectModeStart()
    {
        _renderer = FindObjectsOfType<SpriteRenderer>().ToList();
        _images = FindObjectsOfType<Image>().ToList();
        _text = FindObjectsOfType<Text>().ToList();
        _coroutine = StartCoroutine(SelectMode<SpriteRenderer>());
    }
    IEnumerator SelectMode<T>() where T : Component
    {
        T changeColorObj = null;
        ColorChange();
        while (true)
        {
            var hitObj = GetMousePositionObject<T>();
            if (hitObj != null)
            {
                if (hitObj.TryGetComponent(out SpriteRenderer renderer) && changeColorObj != hitObj)
                {
                    renderer.color += new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0);
                    changeColorObj = hitObj;
                    _renderer.Remove(renderer);
                }
            }
            if (changeColorObj != hitObj)
            {
                var renderer = changeColorObj.GetComponent<SpriteRenderer>();
                renderer.color -= new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0);
                _renderer.Add(renderer);
                changeColorObj = hitObj;
            }

            //‰¹“ü‚ê‚é‚©‚à‚Ë
            yield return null;
        }
    }
    [return: MaybeNull]
    public T GetMousePositionObject<T>()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
        Debug.DrawRay(ray.origin, new Vector3(0, 0, 1) * 100, Color.red);
#nullable enable
        if (hit.transform != null && hit.transform.TryGetComponent(out T? hitGameObjectComponent))
        {
            return hitGameObjectComponent;
        }
#nullable disable
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
            _images.ForEach(image => image.color -= new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _renderer.ForEach(image => image.color -= new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _text.ForEach(text => text.color -= new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _colorChanged = true;
        }
    }
    public void ColorReset()
    {
        if (_colorChanged)
        {
            _images.ForEach(Image => Image.color += new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _renderer.ForEach(image => image.color += new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _text.ForEach(text => text.color += new Color(_changeColorBrightness, _changeColorBrightness, _changeColorBrightness, 0));
            _colorChanged = false;
        }
    }
}
