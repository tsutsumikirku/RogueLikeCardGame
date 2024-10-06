using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenensManager : MonoBehaviour
{
    [Tooltip("ボタンにアタッチして読み込みたいシーンの名前を書く")]
    [SerializeField]string _LoadScene = null;
    [Tooltip("セーブデータを検索、読み込みつつシーン移動。未実装")]
    [SerializeField] string _reStart = null;

    public void LordScene()
    {
        SceneManager.LoadScene(_LoadScene);
    }
    public void ReStart()
    {
        //続きからボタン
        SceneManager.LoadScene(_reStart);
        Debug.Log("リスタート機能未実装");
    }
}
