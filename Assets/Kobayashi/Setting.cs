using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Setting",menuName ="ScriptableObject/Setting")]
public class Setting : ScriptableObject 
{
    private static Setting _instance;
    public int _minDeckRange=5;
    public static Setting Instans
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<Setting>(typeof(Setting).Name);
                if (_instance == null)
                {
                    Debug.LogError($"No {typeof(Setting)} found in Resources folder.");
                }
            }
            return _instance;
        }
    }
}
