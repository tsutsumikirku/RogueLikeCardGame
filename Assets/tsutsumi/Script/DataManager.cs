using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Experimental.RestService;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance = null;
    public SaveData _data;
    public int _bustedEnemy;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void Save()
    {
        CharacterBase _playerdata = GameObject.Find("Player").GetComponent<CharacterBase>();
        _data.turnCount = GameManager.Instance._turnCount;
        _data.deck = _playerdata._deck;
        _data.hp = _playerdata._hp;
        string json = JsonUtility.ToJson(_data);
        string path = Application.persistentDataPath + "/saveData.json";
        File.WriteAllText(path, json);
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "/saveData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);
            CharacterBase _playerdata = GameObject.Find("Player").GetComponent<CharacterBase>();
            GameManager.Instance._turnCount = save.turnCount;
            _playerdata._deck = save.deck;
            _playerdata._hp = save.hp;
            return;
        }
            Debug.LogWarning("ÉZÅ[ÉuÇ™Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
    }
}


[System.Serializable]
public class SaveData
{
    public int turnCount;
    public List<CardBase> deck;
    public float hp;
}

