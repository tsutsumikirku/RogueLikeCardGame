using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScript : MonoBehaviour
{
    public static bool _isVictory;
    [SerializeField] string _resultSceneName;
    [SerializeField] Canvas _resultCanvas;
    [SerializeField,Tooltip("fadeout�A�j���[�V�����p��Image")] Image _fadeAnimationImage;
    [Header("�Q�[���I�����̃e�L�X�g�ݒ�")]
    [SerializeField] Text _isVictoryText;
    [SerializeField] string _victoryText;
    [SerializeField] string _defeatText;
    [Header("�������ɕ\����������")]
    [SerializeField] Image _victoryImage;
    [SerializeField, Tooltip("�N���A���̃p�l���ĕ\���{�^��")] GameObject _victoryPanelButton;
    [Header("��ɕ\����������")]
    [SerializeField,Tooltip("�o�߃^�[���\��Text")] Text _trunCountText;

    public static void LoadResultScene(string loadSceneName, bool isVictory)
    {
        _isVictory = isVictory;
        SceneManager.LoadScene(loadSceneName);
    }
    public static void LoadResultScene(bool isVictory)
    {
        _isVictory = isVictory;
        SceneManager.LoadScene("ResultScene");
    }
    private void Start()
    {
        _victoryImage.gameObject.SetActive(_isVictory);
        _victoryPanelButton.gameObject.SetActive(_isVictory);
        _isVictoryText.text = _isVictory ? _victoryText : _defeatText;
        _trunCountText.text = "�o�߃^�[�� : "+DataManager._instance._data.turnCount.ToString();
    }
    public void SceneLoad(string loadName)
    {
        SceneManager.LoadScene(loadName);
    }
}
