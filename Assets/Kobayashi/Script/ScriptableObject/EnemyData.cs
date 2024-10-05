using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData",menuName = "MyGame/Creat EnemyData")]
public class EnemyData : ScriptableObject
{
    public List<EnemyList> _enemies;
    public List<CharacterBase> _tesureBox = new List<CharacterBase>();
    public List<CharacterBase> _boss = new List<CharacterBase>();
}
[System.Serializable]
public class EnemyList
{
    [SerializeField]public CharacterBase[] _character;
}