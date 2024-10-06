using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScript : MonoBehaviour
{
    public static bool _isVictory;
    [SerializeField] string _resultSceneName;
    [SerializeField] Canvas _resultCanvas;
    [SerializeField,Tooltip("fadeoutアニメーション用のImage")] Image _fadeAnimationImage;
    [Header("ゲーム終了時のテキスト設定")]
    [SerializeField] Text _isVictoryText;
    [SerializeField] string _victoryText;
    [SerializeField] string _defeatText;
    [Header("勝利時に表示されるもの")]
    [SerializeField] Image _victoryImage;
    [SerializeField, Tooltip("クリア時のパネル再表示ボタン")] GameObject _victoryPanelButton;
    [Header("常に表示されるもの")]
    [SerializeField,Tooltip("経過ターン表示Text")] Text _trunCountText;

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
        _trunCountText.text = "経過ターン : "+DataManager._instance._data.turnCount.ToString();
    }
    public void SceneLoad(string loadName)
    {
        SceneManager.LoadScene(loadName);
    }
}
