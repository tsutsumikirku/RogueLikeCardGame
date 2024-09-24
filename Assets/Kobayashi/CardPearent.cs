using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObserver : MonoBehaviour
{
    int _childCount;
    [HideInInspector] public List<GameObject> _lateChild;
    // Start is called before the first frame update
    void Start()
    {
        _childCount=transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (_childCount != transform.childCount)
        {
            transform.GetChild(transform.childCount - 1);
        }
    }
}
