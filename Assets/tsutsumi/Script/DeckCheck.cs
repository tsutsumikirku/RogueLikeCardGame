using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckCheck : MonoBehaviour
{   
    // Start is called before the first frame update
    [SerializeField] Transform _deckParent;
    [SerializeField] int _deckArrangeLength;
    [SerializeField] CharacterBase _player;
    [SerializeField]bool _test = false;
    [SerializeField] List<CardBase> _decki;
    int count = 0;
    public void DeckSets()
    {
        if (!_test)
        {
            int line;
            if (_player._deck.Count % _deckArrangeLength == 0)
            {
                line = _player._deck.Count / _deckArrangeLength;
            }
            else
            {
                line = _player._deck.Count / _deckArrangeLength + 1;
            }
            Transform[] deckParent = new Transform[line];
            for (int i = 0; i < line; i++)
            {

                deckParent[i] = Instantiate(_deckParent);
                for (int j = 0; j < _deckArrangeLength; i++)
                {

                    Instantiate(_player._deck[count]).transform.SetParent(deckParent[i].transform);
                    count += 1;
                }
            }
        }
        else
        {
            int line;
            if (_decki.Count % _deckArrangeLength == 0)
            {
                line = _decki.Count / _deckArrangeLength;
            }
            else
            {
                line =_decki.Count / _deckArrangeLength + 1;
            }
            Transform[] deckParent = new Transform[line];
            for (int i = 0; i < line; i++)
            {

                deckParent[i] = Instantiate(_deckParent);
                for (int j = 0; j < _deckArrangeLength; i++)
                {

                    Instantiate(_player._deck[count]).transform.SetParent(deckParent[i].transform);
                    count += 1;
                }
            }
        }
       
        
    }
}
           

