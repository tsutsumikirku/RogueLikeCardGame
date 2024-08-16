using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    public EnemyData data;
   [SerializeField]  int num;
    // Start is called before the first frame update
    void Start()
    {
        SetStatus(data._enemyList[1]);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetBuff(Buff.DamageBuff, num);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetBuff(Buff.DamageDebuff,1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(3);
        }
    }
    public override void CardExecution(CardBase card, GameObject actionGameObject)
    {
        throw new System.NotImplementedException();
    }
}
