using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;
    [SerializeField] AudioSource Bgm;
    [SerializeField] AudioSource LoopSe;
    [SerializeField] AudioSource Se;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    public void PlayBgm(AudioClip aud)
    {
        Bgm.loop = true;
        Bgm.clip = aud;
        Bgm.Play();
    }
    public void StopBgm()
    {
        Bgm.Stop();
    }
    public void PlaySe(AudioClip aud)
    {
        Se.PlayOneShot(aud);
    }
    public void PlayLoopSe(AudioClip aud)
    {
        LoopSe.loop = true;
        LoopSe.clip = aud;
        LoopSe.Play();
    }
    public void StopLoopSe()
    {
        LoopSe.Stop();
    }
}
