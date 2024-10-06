using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeckCheck : MonoBehaviour
{
    [SerializeField] Transform _deckParent;
    [SerializeField] Transform _deckState;
    [SerializeField] float _yOffSet;
    [SerializeField] int _deckArrangeLength;
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] RectTransform _buttonOfset;
    [SerializeField] CharacterBase _player;
    [SerializeField] bool _test = false;
    [SerializeField] List<CardBase> _decki;
    List<GameObject> _gameobj = new List<GameObject>();
    int count = 0;
    Canvas _canvas;
    GameObject _button;
    public UnityEvent _buttonEvent;

    private void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void DeckSets()
    {
        count = 0; // Reset count
        int line;
        List<CardBase> deckToUse = _test ? _decki : _player._deck;

        if (deckToUse.Count % _deckArrangeLength == 0)
        {
            line = deckToUse.Count / _deckArrangeLength;
        }
        else
        {
            line = deckToUse.Count / _deckArrangeLength + 1;
        }

        Transform[] deckParent = new Transform[line];

        for (int i = 0; i < line; i++)
        {
            deckParent[i] = Instantiate(_deckParent);
            deckParent[i].transform.SetParent(_canvas.transform);
            deckParent[i].position = new Vector3(_deckState.position.x, _deckState.position.y + (i * _yOffSet), _deckState.position.z);
            _gameobj.Add(deckParent[i].gameObject);

            for (int j = 0; j < _deckArrangeLength && count < deckToUse.Count; j++)
            {
                GameObject cardObject = Instantiate(deckToUse[count].gameObject);
                cardObject.transform.SetParent(deckParent[i]);
                count += 1;
            }
        }
        _button = Instantiate(_buttonPrefab, _buttonOfset.position, Quaternion.identity);
        _button.transform.SetParent(_canvas.transform);
        _button.GetComponent<Button>().onClick.AddListener(() =>
        {
            DeckClean();
            _buttonEvent.Invoke();
        });
    }

    public void DeckClean()
    {
        foreach (GameObject obj in _gameobj)
        {
            Destroy(obj);
        }
        _gameobj.Clear();
        Destroy(_button);
    }
}
