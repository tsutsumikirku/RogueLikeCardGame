using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Experimental.RestService;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance = null;
    public SaveData _data;
    public  bool _load = false;
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
        CharacterBase _playerdata = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
        _data.turnCount = GameManager.Instance._turnCount;
        _data.phaseCount = GameManager.Instance._phaseCount;
        _data.deck = _playerdata._deck;
        _data.hp = _playerdata._hp;
        string json = JsonUtility.ToJson(_data);
        string path = Application.persistentDataPath + "/saveData.json";
        File.WriteAllText(path, json);
    }
    public void Load()
    {
        _load = true;
        string path = Application.persistentDataPath + "/saveData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);
            CharacterBase _playerdata = GameObject.FindWithTag("Player").GetComponent<CharacterBase>();
            return;
        }
            Debug.LogWarning("ÉZÅ[ÉuÇ™Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
    }
}


[System.Serializable]
public class SaveData
{
    public int turnCount;
    public int phaseCount;
    public List<CardBase> deck;
    public float hp;
}

