using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData",menuName = "MyGame/Creat EnemyData")]
public class EnemyData : ScriptableObject
{
    public static EnemyData Instance;
    public List<EnemyList> Enemies;
    public List<CharacterBase> _TresureBox = new List<CharacterBase>();
    public List<CharacterBase> Boss = new List<CharacterBase>();
}
[System.Serializable]
public class EnemyList
{
    [SerializeField]public CharacterBase[] _character;
}