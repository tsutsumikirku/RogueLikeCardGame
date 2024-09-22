using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData",menuName = "MyGame/Creat EnemyData")]
public class EnemyData : ScriptableObject
{
    public static EnemyData Instance;
    public List<CharacterBase> _FirstStep =new List<CharacterBase>();
    public List<CharacterBase> _SecondStep = new List<CharacterBase>();
    public List<CharacterBase> _TherdStep = new List<CharacterBase>();
    public List<CharacterBase> _TresureBox = new List<CharacterBase>();
    public List<CharacterBase> Boss = new List<CharacterBase>();
}