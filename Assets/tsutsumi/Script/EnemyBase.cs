using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : CharacterBase
{
    Camera _mainCamera; 
    [SerializeField] Slider _healthBarSlider;
    [SerializeField] Vector3 _offset = new Vector3 { };
    Slider _healthBarSliderObj;
    float _maxhp;

    void Start()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _healthBarSliderObj = Instantiate(_healthBarSlider);
        _healthBarSliderObj.transform.SetParent(canvas.transform);
        _maxhp = _hp;

    }
    void Update()
    {
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position + _offset);
        _healthBarSliderObj.transform.position = screenPos;
        _healthBarSliderObj.value = _hp/_maxhp;
        Text _hptext = _healthBarSliderObj.GetComponentInChildren<Text>();
        _hptext.text = _hp.ToString(); 
    }
}
