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
        //��������{�^��
        SceneManager.LoadScene(_reStart);
        Debug.Log("���X�^�[�g�@�\������");
    }
}
