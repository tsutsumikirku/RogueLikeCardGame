using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckCheck : MonoBehaviour
{
    [SerializeField] Transform _deckParent;
    [SerializeField] Transform _deckState;
    [SerializeField] float _yOffSet;
    [SerializeField] int _deckArrangeLength;
    [SerializeField] CharacterBase _player;
    [SerializeField] bool _test = false;
    [SerializeField] List<CardBase> _decki;
    int count = 0;
    Canvas _canvas;
    private void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    public void DeckSets()
    {
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

            for (int j = 0; j < _deckArrangeLength && count < deckToUse.Count; j++)
            {
                Instantiate(deckToUse[count]).transform.SetParent(deckParent[i]);
                count += 1;
            }
        }
    }
}