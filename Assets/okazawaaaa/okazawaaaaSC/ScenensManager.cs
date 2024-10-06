using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenensManager : MonoBehaviour
{
    public void LordScene(string _lordScene)
    {
        SceneManager.LoadScene(_lordScene);
    }
    public void ReStart(string _reStart)
    {
        //続きからボタン
        SceneManager.LoadScene(_reStart);
        Debug.Log("リスタート機能未実装");
    }
}
