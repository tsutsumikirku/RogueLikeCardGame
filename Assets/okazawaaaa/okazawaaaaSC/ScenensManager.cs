using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenensManager : MonoBehaviour
{
    [Tooltip("�{�^���ɃA�^�b�`���ēǂݍ��݂����V�[���̖��O������")]
    [SerializeField]string _LoadScene = null;
    [Tooltip("�Z�[�u�f�[�^�������A�ǂݍ��݂V�[���ړ��B������")]
    [SerializeField] string _reStart = null;

    public void LordScene()
    {
        SceneManager.LoadScene(_LoadScene);
    }
    public void ReStart()
    {
        //��������{�^��
        SceneManager.LoadScene(_reStart);
        Debug.Log("���X�^�[�g�@�\������");
    }
}
