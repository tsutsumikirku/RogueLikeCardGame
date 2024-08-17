using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip _bgm;
    [SerializeField] AudioClip _se;
    [SerializeField] AudioClip _seLoop;
    [SerializeField] string _playBgm;
    [SerializeField] string _playSe;
    [SerializeField] string _playSeLoop;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_playBgm))
        {
            SoundsManager.Instance.PlayBgm(_bgm);
        } 
        if (Input.GetKeyDown(_playSe))
        {
            SoundsManager.Instance.PlaySe(_se);
        }
        if (Input.GetKeyDown(_playSeLoop))
        {
            SoundsManager.Instance.PlayLoopSe(_seLoop);
        }
    }
}
