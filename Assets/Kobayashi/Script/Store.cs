using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] bool _test;
    private void Start()
    {
        if (_test)
        {
            StoreActive();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void StoreActive()
    {
        gameObject.SetActive(true);
    }
}
