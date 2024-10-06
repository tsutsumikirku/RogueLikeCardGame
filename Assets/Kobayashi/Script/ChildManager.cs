using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildManager : MonoBehaviour
{
    List <Transform> _childTransform=new List<Transform>();
    // Start is called before the first frame update
    private void OnEnable()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            _childTransform.Add(transform.GetChild(i));
            Debug.Log(_childTransform[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
