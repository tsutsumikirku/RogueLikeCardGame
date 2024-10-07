using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleCall : MonoBehaviour
{
    public void title(string _title)
    {
        SceneManager.LoadScene(_title);
    }
}
