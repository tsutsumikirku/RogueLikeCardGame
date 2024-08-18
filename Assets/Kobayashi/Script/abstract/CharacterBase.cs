using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField,Tooltip("�i���o�t�ł̐��l�̏㏸�l")] float _buffUp = 0.1f;
    [SerializeField,Tooltip("����o�t�ł̐��l�̏㏸�l")] float _oneTimeBuffUp = 0.1f;
    [SerializeField, Tooltip("�f�o�t�̐��l�̏㏸�l")] float _debuffUp = 0.1f;
    [HideInInspector]public AttackPattern _attackPattern = AttackPattern.Single;
    [HideInInspector] public int _attackCount = 1;
    public string _name;
    public float _hp;
    public float _attackpower;
    public List<CardBase> _deck;
    [HideInInspector]public List<Buff> _buff;//�i���o�t�̃��X�g
    [HideInInspector] List<Buff> _oneTimeBuff;//����o�t�̃��X�g
    [HideInInspector]public List<Buff> _debuff;//�f�o�t�̃��X�g
    public Buff _characterBuff;//�L�����N�^�[�̑����l���C���v���C���[�̑�����None�̑z��ł�
    public void Attack(CharacterBase enemy)
    {
        //for���[�v�ŃG�l�~�[�̑����l�ƑΏۂƂȂ�o�t���������Čv�Z���ɂ�蓖�Ă͂߂Ă��܂��B
        for(int i= 0; i< 3; i++)
        {
            if (enemy._characterBuff == (Buff)i)
            {
                float buff = 0;
                float onetimebuff = 0;
                float debuff = 0;
                for (int j = 0; j < _buff.Count; j++)
                {
                    if (_buff[j] == (Buff)i)
                    {
                        buff += _buffUp;
                    }
                }
                for(int h = 0; h < _oneTimeBuff.Count; h++)
                {
                    if (_buff[h] == (Buff)i)
                    {
                        onetimebuff += _oneTimeBuffUp;
                    }
                }
                debuff += _debuffUp * _debuff.Count;
                enemy._hp -= (_attackpower + buff) * (onetimebuff + 1) - debuff;
                Debug.Log($"�U�� {(_attackpower + buff) * (onetimebuff + 1) - debuff}");
                break;
            }
        }
    }
    public void BuffReset()
    {
        _buff.Clear();
    }
    public void OneTimeBuffReset()
    {
        _oneTimeBuff.Clear();
    }
   
}
public enum AttackPattern
{
    Single,
    All
}
public enum Buff
{
    None,
    Red,
    Green,
    Blue
}

