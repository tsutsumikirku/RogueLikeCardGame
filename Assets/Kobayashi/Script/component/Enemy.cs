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
        //SetStatus(data._enemyList[1]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
