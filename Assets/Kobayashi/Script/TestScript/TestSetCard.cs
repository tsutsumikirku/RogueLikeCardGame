using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetCard : MonoBehaviour
{
    [SerializeField] CardBase card;
    [SerializeField]CardBase card2;
    // Start is called before the first frame update
    void Start()
    {
        card = card2;
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
