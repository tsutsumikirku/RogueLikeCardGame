using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ButtleTimeLineManager : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] PlayableDirector _director;
    public Text Text { get => _text; }
    public PlayableDirector Director { get => _director; }
    void StartTimeLine(string text)
    {
        _director.Play();
        _text.text = text;
    }
}
